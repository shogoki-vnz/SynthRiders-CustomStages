using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;
using System.IO;
using System;
using System.Linq;
using SRToolkit;
using UnityEngine.SceneManagement;

namespace SRToolkit
{
    public class WorkspaceManager : EditorWindow
    {
        static public WSPSessionInfo activeWorkspace;

        [MenuItem("SynthRiders/Workspace Manager", false, 2)]
        public static void Init()
        {
            WorkspaceManager wnd = GetWindow<WorkspaceManager>();
            wnd.titleContent = new GUIContent("WorkSpace Manager");
            Vector2 size = new Vector2(1000, 640);
            wnd.minSize = size;
            wnd.maxSize = size;

        }

        private VisualElement m_DetailSection;
        private WSPSessionInfo m_activeItem;

        private VisualElement m_stagesList;
        private VisualElement m_spacesContainer;

        private VisualElement normStageMeta;
        private VisualElement spinStageMeta;
        private VisualElement expMeta;

        private ToolbarButton currentSelectedTab;


        private Texture2D defaultStageIcon;
        private Texture2D defaultexpBanner;

        public void CreateGUI()
        {
			rootVisualElement.Clear();
			//rootVisualElement.styleSheets.Clear();

			try
			{
				BuildUI();
			}
			catch (Exception e)
			{
				Debug.LogError($"[WorkspaceManager] CreateGUI failed, rebuilding aborted: {e}");
				rootVisualElement.Clear(); // don't leave a half-built tree on screen
				return;
			}

			// Force layout/text recalculation only AFTER the tree is fully built
			rootVisualElement.schedule.Execute(() =>
			{
				rootVisualElement.style.display = DisplayStyle.None;
				rootVisualElement.style.display = DisplayStyle.Flex;
			});
		}
        private void BuildUI()
        {
            defaultStageIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SRStageWrench/Editor/Resources/CustomStages.png");
            defaultexpBanner = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SRStageWrench/Editor/Resources/Custom_Experience_Generic_Header.png");

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SRStageWrench/Editor/UI/WorkspaceManager.uxml");
            VisualElement loadedUXML = visualTree.Instantiate();
            visualTree.CloneTree(rootVisualElement);
            // Load StyleSheet
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SRStageWrench/Editor/UI/Workspace.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            //Metadata screens
            m_DetailSection = rootVisualElement.Q<VisualElement>("workspaceMeta");
            normStageMeta = rootVisualElement.Q<VisualElement>("normStageMeta");
            spinStageMeta = rootVisualElement.Q<VisualElement>("spinStageMeta");
            expMeta = rootVisualElement.Q<VisualElement>("expMeta");

            m_spacesContainer = rootVisualElement.Q<VisualElement>("spaceContainer");

            rootVisualElement.Q<Button>("newSpace").clicked += showCreateScreen;
            //rootVisualElement.Q<Button>("import").clicked += ImportPrompt;

            //Metadata Tab switching
            rootVisualElement.Q<ToolbarButton>("normData").RegisterCallback<ClickEvent>(ev => switchMETA(rootVisualElement.Q<ToolbarButton>("normData")));
            rootVisualElement.Q<ToolbarButton>("spinData").RegisterCallback<ClickEvent>(ev => switchMETA(rootVisualElement.Q<ToolbarButton>("spinData")));
            rootVisualElement.Q<ToolbarButton>("expData").RegisterCallback<ClickEvent>(ev => switchMETA(rootVisualElement.Q<ToolbarButton>("expData")));
            rootVisualElement.Q<ToolbarButton>("metaData").RegisterCallback<ClickEvent>(ev => switchMETA(rootVisualElement.Q<ToolbarButton>("metaData")));



            rootVisualElement.Q<Button>("switchtoSpace").clicked += SwitchWorkspace;
            rootVisualElement.Q<Button>("cloneSpace").RegisterCallback<ClickEvent>(ev =>
            {
                EditorUtility.DisplayDialog("Contained Scripts Warning", $"In order to prevent script errors caused by multiple(s) of the same script, any scripts contained in {m_activeItem.name}'s folder will be deleted after copying them to the Cloned Workspace. if you have any Editor scripts that you would like to use across all workspaces, consider putting them in the Root Assets folder to avoid this issue.", "Ok");
                SpaceFileManager.CloneWorkspace(m_activeItem);
                ReloadWorkspaceList();
            });
            rootVisualElement.Q<Button>("mergeSpaces").clicked += ShowMergePrompt;
            rootVisualElement.Q<Button>("deleteSpace").clicked += ShowdeleteWarning;

            tabList = rootVisualElement.Q<VisualElement>("spaceList");


            LoadWorkspaces();
            ElementReferences();
            rootVisualElement.Q<VisualElement>("spaceContainer").style.visibility = Visibility.Hidden;
            //if there is an activeWorkspace saved and loaded, Bind all values so it's the first thing the users see's when opening the panel
            // Auto-select the active workspace in the list
            if (activeWorkspace != null)
            {
                int activeIndex = workspaceList.IndexOf(activeWorkspace);
                if (activeIndex >= 0)
                {
                    itemListView.SetSelection(activeIndex);
                }
                else
                {
                    BindInfo(activeWorkspace);

                }
            }
        }

		private void ElementReferences()
		{
            //Register when strings change
            normStageMeta.RegisterCallback<ChangeEvent<string>>(evt => { UpdateCurrentList(); });
            spinStageMeta.RegisterCallback<ChangeEvent<string>>(evt => { UpdateCurrentList(); });
            expMeta.RegisterCallback<ChangeEvent<string>>(evt => { UpdateCurrentList(); });
            //Registers when Object refs change
            VisualElement scenesBox = m_DetailSection.Q<VisualElement>("Scenes");
            //Setup Warning box for Incorrect Tag;
            TextField tagField = expMeta.Q<TextField>("expTag");
            Label expCharCount = tagField.Q<Label>("charCount");
            //HelpBox incorrectTag = new HelpBox("The Tag is INCORRECT! you MUST add 'EXP-' to the beginning!", HelpBoxMessageType.Error);
            //incorrectTag.name = "TagIncorrectWarning";
            //tagField.Add(incorrectTag);

            scenesBox.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt => { SceneRefs(evt.target as ObjectField, evt); });
            scenesBox.RegisterCallback<ClickEvent>(evt => { SceneButtons(evt); });

            normStageMeta.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
            {
                Texture2D newTxt = evt.newValue as Texture2D;

                try { normStageMeta.Q<IMGUIContainer>("normIcon").style.backgroundImage = newTxt != null ? newTxt != m_activeItem.workspaceInfoScript.normStageIcon ? newTxt : m_activeItem.workspaceInfoScript.normStageIcon : m_activeItem.workspaceInfoScript.normStageIcon ? m_activeItem.workspaceInfoScript.normStageIcon : defaultStageIcon; }
				catch (Exception) { newTxt = defaultStageIcon; Debug.LogWarning("Reverted to Defualt Spin image"); }

                UpdateCurrentList();
            });
            spinStageMeta.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
            {
                Texture2D newTxt = evt.newValue as Texture2D;

                try { spinStageMeta.Q<IMGUIContainer>("spinIcon").style.backgroundImage = newTxt != null ? newTxt != m_activeItem.workspaceInfoScript.spinStageIcon ? newTxt : m_activeItem.workspaceInfoScript.spinStageIcon : m_activeItem.workspaceInfoScript.spinStageIcon ? m_activeItem.workspaceInfoScript.spinStageIcon : defaultStageIcon; }
                catch (Exception) { newTxt = defaultStageIcon; Debug.LogWarning("Reverted to Defualt Spin image"); }

				UpdateCurrentList();
            });
            expMeta.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
            {
                Texture2D newTxt = evt.newValue as Texture2D;

                try { expMeta.Q<IMGUIContainer>("expBanner").style.backgroundImage = newTxt != null ? newTxt != m_activeItem.workspaceInfoScript.expPromptBanner ? newTxt : m_activeItem.workspaceInfoScript.expPromptBanner : m_activeItem.workspaceInfoScript.expPromptBanner ? m_activeItem.workspaceInfoScript.expPromptBanner : defaultexpBanner; }
                catch (Exception) { newTxt = defaultexpBanner; Debug.LogWarning("Reverted to Defualt EXP image"); }

				UpdateCurrentList();
            });
            
            expMeta.Q<TextField>("expTag").RegisterCallback<ChangeEvent<String>>(evt => 
            {
                expCharCount.text = $"{tagField.value.Length}/12";
                if(tagField.value.Length <= 0) { expCharCount.style.display = DisplayStyle.None; }
				else { expCharCount.style.display = DisplayStyle.Flex; }
                if (tagField.value.Length < 12) { expCharCount.style.color = Color.red; }
				else { expCharCount.style.color = Color.white; }
            });
        }


        //Locate all Assets
        private static List<WSPSessionInfo> workspaceList = new List<WSPSessionInfo>();
        public void LoadWorkspaces()
        {
            workspaceList.Clear();

            string[] allPaths = Directory.GetFiles("Assets/SRStageWrench/Editor/WorkspaceInfo", "*.asset",
            SearchOption.AllDirectories);

            foreach (string path in allPaths)
            {
                string cleanedPath = path.Replace("\\", "/");
                workspaceList.Add((WSPSessionInfo)AssetDatabase.LoadAssetAtPath(cleanedPath,
                    typeof(WSPSessionInfo)));
            }

            if (activeWorkspace != null) { m_activeItem = activeWorkspace; }
            generateListItems();
        }

        public void ReloadWorkspaceList()
        {
            tabList.Remove(itemListView);
            LoadWorkspaces();
        }

		#region ListView
		//Add the items to the list
		private VisualElement tabList;
        [SerializeField]
        public VisualTreeAsset WorkspaceListItem;
        private ListView itemListView;
        private void generateListItems()
        {
            Func<VisualElement> makeItem = () => WorkspaceListItem.CloneTree();

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                e.Q<Label>("spaceName").text = workspaceList[i].workspaceName;

                if (workspaceList[i].workspaceStageInfo)
                {
                    var CSINFO = workspaceList[i].workspaceInfoScript;
                    if (CSINFO)
                    {
                        if (isValidNorm(CSINFO)) { e.Q<Label>("normMETA").style.display = DisplayStyle.Flex; } else { e.Q<Label>("normMETA").style.display = DisplayStyle.None; }
                        if (isValidSpin(CSINFO)) { e.Q<Label>("spinMETA").style.display = DisplayStyle.Flex; } else { e.Q<Label>("spinMETA").style.display = DisplayStyle.None; }
                        if (isValidExp(CSINFO)) { e.Q<Label>("expMETA").style.display = DisplayStyle.Flex; } else { e.Q<Label>("expMETA").style.display = DisplayStyle.None; }
					}
                }
                else 
                { 
                    Debug.LogError("Workspace: " + workspaceList[i].name + " has no CustomStageInfo Referenced!");
                    e.Q<Label>("normMETA").style.display = DisplayStyle.None;
                    e.Q<Label>("spinMETA").style.display = DisplayStyle.None;
                    e.Q<Label>("expMETA").style.display = DisplayStyle.None;
                }
            };

            itemListView = new ListView(workspaceList, 70, makeItem, bindItem);
            itemListView.selectionType = SelectionType.Single;
            itemListView.style.height = workspaceList.Count * 100;
            tabList.Add(itemListView);
            itemListView.selectionChanged += ListView_onSelectionChange;
        }

        bool isLoadingData = false;
        private void ListView_onSelectionChange(IEnumerable<object> selectedItems)
        {
            isLoadingData = true;
            m_activeItem = (WSPSessionInfo)selectedItems.First();
            BindInfo(m_activeItem);
            //Debug.Log(m_activeItem.workspaceStageInfo);
        }

        private void BindInfo(WSPSessionInfo toInfo)
		{
			m_DetailSection.Unbind();
			rootVisualElement.Q<VisualElement>("top").Unbind();
			normStageMeta.Unbind();
			spinStageMeta.Unbind();
			expMeta.Unbind();

			SerializedObject so = new SerializedObject(toInfo);
            m_DetailSection.Bind(so);
            rootVisualElement.Q<VisualElement>("top").Bind(so);
            if (toInfo.workspaceStageInfo)
            {
                SerializedObject info = new SerializedObject(toInfo.workspaceInfoScript);
                normStageMeta.Bind(info);
                spinStageMeta.Bind(info);
                expMeta.Bind(info);

                rootVisualElement.Q<IMGUIContainer>("normIcon").style.backgroundImage = toInfo.workspaceInfoScript.normStageIcon ? toInfo.workspaceInfoScript.normStageIcon : defaultStageIcon;
                rootVisualElement.Q<IMGUIContainer>("spinIcon").style.backgroundImage = toInfo.workspaceInfoScript.spinStageIcon ? toInfo.workspaceInfoScript.spinStageIcon : defaultStageIcon;
                rootVisualElement.Q<IMGUIContainer>("expBanner").style.backgroundImage = toInfo.workspaceInfoScript.expPromptBanner ? toInfo.workspaceInfoScript.expPromptBanner : defaultexpBanner;
            }
            m_spacesContainer.style.visibility = Visibility.Visible;
            m_DetailSection.style.display = DisplayStyle.Flex;
            normStageMeta.style.display = DisplayStyle.None;
            spinStageMeta.style.display = DisplayStyle.None;
            expMeta.style.display = DisplayStyle.None;

            // Set default selected tab to metaData
            if (currentSelectedTab != null)
            {
                currentSelectedTab.RemoveFromClassList("Tab-selected");
            }
            ToolbarButton metaDataTab = rootVisualElement.Q<ToolbarButton>("metaData");
            if (metaDataTab != null)
            {
                metaDataTab.AddToClassList("Tab-selected");
                currentSelectedTab = metaDataTab;
            }

            if (toInfo == activeWorkspace)
            {
                rootVisualElement.Q<Button>("switchtoSpace").style.display = DisplayStyle.None;
                rootVisualElement.Q<VisualElement>("activeIndicator").style.display = DisplayStyle.Flex;
            }
            else
            {
                rootVisualElement.Q<Button>("switchtoSpace").style.display = DisplayStyle.Flex;
                rootVisualElement.Q<VisualElement>("activeIndicator").style.display = DisplayStyle.None;
            }
            isLoadingData = false;
        }

        private void UpdateCurrentList()
		{
            //Debug.Log("REBUILD!");
            itemListView.Rebuild();
            if(m_activeItem.workspaceStageInfo != null && isLoadingData != true) 
            {
                Debug.Log("Saving Prefab!");
                EditorUtility.SetDirty(m_activeItem.workspaceStageInfo);
                PrefabUtility.RecordPrefabInstancePropertyModifications(m_activeItem.workspaceStageInfo);
                AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();
                Debug.Log("Saved!");
            }
		}
		#endregion

		private void switchMETA(ToolbarButton inButton)
		{
			//Debug.Log(inButton);
			m_DetailSection.style.display = DisplayStyle.None;
			normStageMeta.style.display = DisplayStyle.None;
			spinStageMeta.style.display = DisplayStyle.None;
			expMeta.style.display = DisplayStyle.None;

			// Remove selected class from previously selected tab
			if (currentSelectedTab != null)
			{
				currentSelectedTab.RemoveFromClassList("Tab-selected");
			}

			// Add selected class to newly clicked tab
			inButton.AddToClassList("Tab-selected");
			currentSelectedTab = inButton;

			if (inButton.name == "normData") { normStageMeta.style.display = DisplayStyle.Flex; }
			if (inButton.name == "spinData") { spinStageMeta.style.display = DisplayStyle.Flex; }
			if (inButton.name == "expData") { expMeta.style.display = DisplayStyle.Flex; }
			if (inButton.name == "metaData") { m_DetailSection.style.display = DisplayStyle.Flex; }
		}

        #region InfoScreen Scene Managing
        private void SceneRefs(ObjectField changedField, ChangeEvent<UnityEngine.Object> evt)
        {
            //Debug.Log("Field that changed: " + changedField);
            if (evt.newValue == null)
            {
                changedField.Q<Button>("createScene").style.display = DisplayStyle.Flex;
                changedField.Q<Button>("SwitchScene").style.display = DisplayStyle.None;
                changedField.Q<Button>("delScene").style.display = DisplayStyle.None;
            }
            else
            {
                changedField.Q<Button>("createScene").style.display = DisplayStyle.None;
                changedField.Q<Button>("SwitchScene").style.display = DisplayStyle.None;
                if (EditorSceneManager.GetActiveScene().name != changedField.value.name) { changedField.Q<Button>("SwitchScene").style.display = DisplayStyle.Flex; }
                changedField.Q<Button>("delScene").style.display = DisplayStyle.Flex;
            }
        }

        private void SceneButtons(ClickEvent evt)
		{
            if (evt.target.GetType() == rootVisualElement.Q<Button>().GetType())
			{
                Button pressedButton = evt.target as Button;
                if (pressedButton.name == "createScene")
                {
                    Debug.Log("Create " + pressedButton.parent + " 's Scene");
                    ObjectField parentField = pressedButton.parent as ObjectField;
                    if (parentField.name == "normSceneField") { StageSceneManager.CreateNormalScene(m_activeItem); parentField.value = m_activeItem.normStageScene; }
                    if (parentField.name == "spinSceneField") { StageSceneManager.CreateSpinScene(m_activeItem); parentField.value = m_activeItem.spinStageScene; }
                }
                if (pressedButton.name == "delScene")
                {
                    ObjectField parentField = pressedButton.parent as ObjectField;
                    Debug.Log("Delete Scene from parent's value: " + AssetDatabase.GetAssetPath(parentField.value));
                    StageSceneManager.DeleteScene(AssetDatabase.GetAssetPath(parentField.value));
                    parentField.value = null;
                }
                if (pressedButton.name == "SwitchScene")
                {
                    ObjectField parentField = pressedButton.parent as ObjectField;
                    Debug.Log("Switching Scenes to:" + AssetDatabase.GetAssetPath(parentField.value));
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(parentField.value));
                    //a little Crude to update all values but, it updates the buttons
                    //to show correctly
                    BindInfo(m_activeItem);
                    pressedButton.style.display = DisplayStyle.None;
                }
            }
            
        }
		#endregion
		#region CreateScreen
		private void showCreateScreen()
        {
            rootVisualElement.Q<VisualElement>("spaceList").style.display = DisplayStyle.None;
            rootVisualElement.Q<VisualElement>("spaceContainer").style.display = DisplayStyle.None;

            TextField pathField = rootVisualElement.Q<TextField>("importPath");
            Button clearPath = rootVisualElement.Q<Button>("clearPath");

            pathField.RegisterValueChangedCallback(evt => { if (pathField.value == null) { clearPath.style.display = DisplayStyle.None; } else { clearPath.style.display = DisplayStyle.Flex; } });
            clearPath.RegisterCallback<ClickEvent>(evt => { pathField.value = null; pathField.tooltip = "Path to a Assets folder that contains a '_CustomStageElements' subfolder"; rootVisualElement.Q<Button>("createnewSpace").text = "Create"; });
            rootVisualElement.Q<VisualElement>("createSpace").style.display = DisplayStyle.Flex;

            rootVisualElement.Q<Button>("choosePath").RegisterCallback<ClickEvent>(evt => { ImportPrompt(evt); });
            rootVisualElement.Q<Button>("createnewSpace").clicked += goNewWorkspace;
            rootVisualElement.Q<Button>("cancel").clicked += hideCreateScreen;
        }

        private void goNewWorkspace()
        {
            TextField newSpaceName = rootVisualElement.Q<TextField>("newSpaceName");
            TextField newSpaceAuthor = rootVisualElement.Q<TextField>("newSpaceAuthor");
            TextField importPathField = rootVisualElement.Q<TextField>("importPath");

            rootVisualElement.Q<Button>("createnewSpace").clicked -= goNewWorkspace;
            //Debug.Log("Import Field String length: " + importPathField.value.Length);

            Toggle isNormSetup = rootVisualElement.Q<Toggle>("normType");
            Toggle isSpinSetup = rootVisualElement.Q<Toggle>("spinType");
            Toggle isExpSetup = rootVisualElement.Q<Toggle>("expType");

            //if the Field isn't empty, assume Importing
            if (importPathField.value.Length > 0) 
            { 
                Debug.Log("Importing Stage!"); 
                SpaceFileManager.ImportAnotherProject(importPathField.value, newSpaceName.value, newSpaceAuthor.value);
                StageSceneManager.SetupScenes(SpaceFileManager.newlyCreated, false, isNormSetup.value, isSpinSetup.value, isExpSetup.value);
            }
			else 
            { 
                SpaceFileManager.createNewWorkspace(newSpaceName.value, newSpaceAuthor.value, true, false);
                StageSceneManager.SetupScenes(SpaceFileManager.newlyCreated, true, isNormSetup.value, isSpinSetup.value, isExpSetup.value);
            }
            hideCreateScreen();
            ReloadWorkspaceList();
        }

        private void ImportPrompt(ClickEvent evt)
        {
            string path = EditorUtility.OpenFolderPanel("select Assets folder of another Project that contains _CustomStageElements Folder", "", "");
            if (path.Length != 0)
            {
                Button rootButton = evt.target as Button;
                TextField buttonParent = rootButton.parent as TextField;
                buttonParent.value = path;
                buttonParent.tooltip = path;
                rootVisualElement.Q<Button>("createnewSpace").text = "Import";
            }
        }

        private void hideCreateScreen()
        {
            rootVisualElement.Q<Button>("createnewSpace").clicked -= SwitchWorkspace;
            rootVisualElement.Q<VisualElement>("createSpace").style.display = DisplayStyle.None;

            rootVisualElement.Q<VisualElement>("spaceList").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<VisualElement>("spaceContainer").style.display = DisplayStyle.Flex;
        }
		#endregion
		#region MergeScreen
		private WSPSessionInfo mergeFrom;
        private void ShowMergePrompt()
		{
            VisualElement mergespacePrompt = rootVisualElement.Q<VisualElement>("mergeSpace");
            mergespacePrompt.style.display = DisplayStyle.Flex;
            rootVisualElement.Q<VisualElement>("areYouSure").style.display = DisplayStyle.None;
            rootVisualElement.Q<Label>("selectLabel").style.display = DisplayStyle.Flex;
            mergeFrom = m_activeItem;
            itemListView.selectionChanged += ShowMergeAreYouSure;
            rootVisualElement.Q<Button>("cancelMerge").clicked += cancelMerge;
            rootVisualElement.Q<Button>("ConfirmMerge").clicked += ConfirmMerge;
        }

        private void ShowMergeAreYouSure(IEnumerable<object> selectedItems)
		{
            m_activeItem = (WSPSessionInfo)selectedItems.First();
            rootVisualElement.Q<Label>("selectLabel").style.display = DisplayStyle.None;
            Debug.Log("Active Item: " + m_activeItem + " Merge From: " + mergeFrom);

            VisualElement mergeFromVisual = rootVisualElement.Q<VisualElement>("MergeFromVisual");
            Label mergeFromNormal = mergeFromVisual.Q<Label>("normal");
            Label mergeFromSpin = mergeFromVisual.Q<Label>("spin");
            Label mergeFromExp = mergeFromVisual.Q<Label>("exp");

            VisualElement mergeToVisual = rootVisualElement.Q<VisualElement>("MergeToVisual");
            Label mergeToNormal = mergeToVisual.Q<Label>("normal");
            Label mergeToSpin = mergeToVisual.Q<Label>("spin");
            Label mergeToExp = mergeToVisual.Q<Label>("exp");
            //Set Metadata
            mergeToVisual.Q<Label>("name").text = mergeFrom.workspaceName;
            if (mergeFrom.workspaceInfoScript.isNormStage) { mergeToNormal.style.display = DisplayStyle.Flex; } else { mergeToNormal.style.display = DisplayStyle.None; }
            if (mergeFrom.workspaceInfoScript.isSpinStage) { mergeToSpin.style.display = DisplayStyle.Flex; } else { mergeToSpin.style.display = DisplayStyle.None; }
            if (mergeFrom.workspaceInfoScript.isExperience) { mergeToExp.style.display = DisplayStyle.Flex; } else { mergeToExp.style.display = DisplayStyle.None; }

            if (m_activeItem != mergeFrom)
			{
                rootVisualElement.Q<VisualElement>("areYouSure").style.display = DisplayStyle.Flex;
                //Set Metadata
                mergeFromVisual.Q<Label>("name").text = m_activeItem.workspaceName;
                if (m_activeItem.workspaceInfoScript.isNormStage) { mergeFromNormal.style.display = DisplayStyle.Flex; } else { mergeFromNormal.style.display = DisplayStyle.None; }
                if (m_activeItem.workspaceInfoScript.isSpinStage) { mergeFromSpin.style.display = DisplayStyle.Flex; } else { mergeFromSpin.style.display = DisplayStyle.None; }
                if (m_activeItem.workspaceInfoScript.isExperience) { mergeFromExp.style.display = DisplayStyle.Flex; } else { mergeFromExp.style.display = DisplayStyle.None; }
            }

        }

        private void ConfirmMerge()
		{
            itemListView.selectionChanged -= ShowMergeAreYouSure;
            rootVisualElement.Q<Button>("cancelMerge").clicked -= cancelMerge;
            rootVisualElement.Q<Button>("ConfirmMerge").clicked -= ConfirmMerge;
            rootVisualElement.Q<VisualElement>("mergeSpace").style.display = DisplayStyle.None;
            Debug.Log("Go Ahead to Merge!");
            SpaceFileManager.MergeWorkSpaces(m_activeItem, mergeFrom);
            //Delete the other Definition
            SpaceFileManager.DeleteWorkspace(m_activeItem, false);
            workspaceList.Remove(m_activeItem);
            itemListView.Rebuild();
        }

        private void cancelMerge()
		{
            itemListView.selectionChanged -= ShowMergeAreYouSure;
            rootVisualElement.Q<VisualElement>("mergeSpace").style.display = DisplayStyle.None;
            rootVisualElement.Q<Button>("cancelMerge").clicked -= cancelMerge;
            rootVisualElement.Q<Button>("ConfirmMerge").clicked -= ConfirmMerge;
        }
		#endregion
		#region DeleteScreen

		private void ShowdeleteWarning()
		{
            rootVisualElement.Q<VisualElement>("spaceList").style.display = DisplayStyle.None;
            rootVisualElement.Q<VisualElement>("spaceContainer").style.display = DisplayStyle.None;
            rootVisualElement.Q<VisualElement>("deleteWarning").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<Button>("cancelDel").clicked += CancelDelete;
            rootVisualElement.Q<Button>("confirmDel").clicked += ConfirmDelete;
        }

        private void CancelDelete()
		{
            rootVisualElement.Q<Button>("cancelDel").clicked -= CancelDelete;
            rootVisualElement.Q<Button>("confirmDel").clicked -= ConfirmDelete;
            rootVisualElement.Q<VisualElement>("spaceList").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<VisualElement>("spaceContainer").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<VisualElement>("deleteWarning").style.display = DisplayStyle.None;
        }


        private void ConfirmDelete()
		{
            rootVisualElement.Q<Button>("confirmDel").clicked -= ConfirmDelete;
			EditorSceneManager.CloseScene(SceneManager.GetActiveScene(), true);
			SpaceFileManager.DeleteWorkspace(m_activeItem, rootVisualElement.Q<Toggle>("deleteFilesToggle").value);
            rootVisualElement.Q<VisualElement>("deleteWarning").style.display = DisplayStyle.None;
			rootVisualElement.Q<VisualElement>("spaceList").style.display = DisplayStyle.Flex;
			rootVisualElement.Q<VisualElement>("spaceContainer").style.display = DisplayStyle.Flex;
            if(workspaceList.Count < 0)
            {
				m_spacesContainer.style.visibility = Visibility.Hidden;
			}
            HandleDeletionofWorkspace();
        }
		#endregion
        public void HandleDeletionofWorkspace()
        {
			workspaceList.Remove(m_activeItem);
			itemListView.Rebuild();
            if (workspaceList.Count >= 0)
            {
				activeWorkspace = workspaceList[0];
				BindInfo(workspaceList[0]);
            }		
        }
		public void SwitchWorkspace()
        {

            PlayerPrefs.SetString("CurrentSpace", AssetDatabase.GetAssetPath(m_activeItem));
            Debug.Log(m_activeItem);
            Debug.Log("CurrentSpaceValue: " + PlayerPrefs.GetString("CurrentSpace"));

            activeWorkspace = m_activeItem;
            SceneViewLabel.SceneLabelText = "Workspace: " + activeWorkspace.workspaceName;
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = activeWorkspace.workspaceStageInfo;

            rootVisualElement.Q<Button>("switchtoSpace").style.display = DisplayStyle.None;
            rootVisualElement.Q<VisualElement>("activeIndicator").style.display = DisplayStyle.Flex;
            BindInfo(m_activeItem);

            SceneAsset[] allScenes = { activeWorkspace.normStageScene, activeWorkspace.spinStageScene, activeWorkspace.expStageScene};
            foreach (var sceneFile in allScenes)
			{
                if(sceneFile != null) { EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(sceneFile));  return; }
			}
        }

        //Used to load Previously Saved(Active) Workspace since everytime a script
        //reload was triggered or the Editor was closed, it would forget what Workspace was Active.
        public static void LoadWorkspace()
		{
            if (PlayerPrefs.GetString("CurrentSpace") != string.Empty)
			{
				Debug.Log($"Attempting to Load previously active Workspace at: {PlayerPrefs.GetString("CurrentSpace")}");
				activeWorkspace = AssetDatabase.LoadAssetAtPath<WSPSessionInfo>(PlayerPrefs.GetString("CurrentSpace"));
				if (activeWorkspace != null)
                {
                    Debug.Log("Workspace: " + activeWorkspace.workspaceName + " Loaded!");
                    SceneViewLabel.SceneLabelText = "Workspace: " + activeWorkspace.workspaceName;
                }
                else
                {
                    Debug.LogError("Failed to load saved Workspace! You can ignore this warning unless this happens Multiple times. Clearing saved Value!");
                    PlayerPrefs.DeleteKey("CurrentSpace");
                }
            }   
        }

        public WSPSessionInfo getCurrentWKSP{ get { return activeWorkspace; } }

		#region isValid Checks
		private bool isValidNorm(Synth.Utils.CustomStageInfo inputInfo)
		{
            bool isNorm = false;

            if (inputInfo.normTile01 || inputInfo.normTile02 || inputInfo.normTile03 || inputInfo.normSkybox || inputInfo.normSpecialFX || inputInfo.normStageIcon || inputInfo.normStageName != string.Empty || inputInfo.normStageAuthor != string.Empty)
			{
                isNorm = true;
			}
            inputInfo.isNormStage = isNorm;
            return isNorm;
		}
        private bool isValidSpin(Synth.Utils.CustomStageInfo inputInfo)
        {
            bool isSpin = false;

            if (inputInfo.spinEmitter || inputInfo.spinPlatform || inputInfo.spinSpecialFX || inputInfo.spinSkybox || inputInfo.spinStageIcon ||inputInfo.spinStageName != string.Empty || inputInfo.spinStageAuthor != string.Empty)
            {
                isSpin = true;
            }
            inputInfo.isSpinStage = isSpin;
            return isSpin;
        }
        private bool isValidExp(Synth.Utils.CustomStageInfo inputInfo)
        {
            bool isExp = false;
            if (inputInfo.expPrefab || inputInfo.expSpecialsFX || inputInfo.expSkybox || inputInfo.expName != string.Empty || inputInfo.expAuthor != string.Empty || inputInfo.expTag != string.Empty || inputInfo.expPromptText != string.Empty || inputInfo.expPromptBanner)
            {
                isExp = true;
            }
            inputInfo.isExperience = isExp;
            return isExp;
        }
		#endregion
	}
}

[InitializeOnLoad]
public class Startup
{
    //In order to load a Non-static field, we have to do this odd workaround
    //in order to access it. after we do, we are going to load the saved WorkSpace
    static Startup()
	{
        //Debug.Log("Editor has been Opened!");
        Startup myObject = new Startup();
        myObject.Start();
	}
    private void Start()
	{
        WorkspaceManager.LoadWorkspace();
    }
}

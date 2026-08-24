using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using SRToolkit;

public class ExportStage : EditorWindow
{
    public WorkspaceManager workspaceManager;

    [MenuItem("SynthRiders/Export Stage", false, 1)]
    public static void ShowWindow()
    {
        ExportStage wnd = GetWindow<ExportStage>();
        wnd.titleContent = new GUIContent("ExportStage");
        Vector2 size = new Vector2(530, 250);
        wnd.minSize = size;
        wnd.maxSize = size;
    }

    public HelpBox expTagWarning;
    public WSPSessionInfo currentlyShownSpace;
    public void CreateGUI()
    {
        //Load UI and Clone
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SRStageWrench/Editor/UI/ExportStage.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        visualTree.CloneTree(rootVisualElement);
        //Load StyleSheet and add to current UI
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SRStageWrench/Editor/UI/Workspace.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        HelpBox incorrectTag = new HelpBox("Invalid Experience Tag! tag MUST be EXACTLY 12 characters. (EXP- is Automatically added during export)", HelpBoxMessageType.Error);
        incorrectTag.name = "TagIncorrectWarning";
        expTagWarning = incorrectTag;
        rootVisualElement.Q<VisualElement>("helpBoxWrap").Add(incorrectTag);

        LoadWorkspaceInfo();

        var pcvrCheckbox = rootVisualElement.Q<Toggle>("exportPCVR");
        var androidCheckbox = rootVisualElement.Q<Toggle>("exportANDROID");

        rootVisualElement.Q<Button>("exportBundled").RegisterCallback<ClickEvent>(ev => ExportStageManager.ExportStage(WorkspaceManager.activeWorkspace, pcvrCheckbox.value, androidCheckbox.value, true));
        rootVisualElement.Q<Button>("exportSeperate").RegisterCallback<ClickEvent>(ev => ExportStageManager.ExportStage(WorkspaceManager.activeWorkspace, pcvrCheckbox.value, androidCheckbox.value, false));
        rootVisualElement.Q<Button>("cancelExport").clicked += this.Close;
    }
    
    private void LoadWorkspaceInfo()
	{
        WSPSessionInfo activeWorkspace = WorkspaceManager.activeWorkspace;
        if(WorkspaceManager.activeWorkspace != null)
		{
            currentlyShownSpace = WorkspaceManager.activeWorkspace;

            SerializedObject so = new SerializedObject(activeWorkspace);
            rootVisualElement.Q<VisualElement>("currentSpace").Bind(so);

            if (activeWorkspace.workspaceInfoScript.isNormStage) { rootVisualElement.Q<Label>("normal").style.display = DisplayStyle.Flex; } else { rootVisualElement.Q<Label>("normal").style.display = DisplayStyle.None; }
            if (activeWorkspace.workspaceInfoScript.isSpinStage) { rootVisualElement.Q<Label>("spin").style.display = DisplayStyle.Flex; } else { rootVisualElement.Q<Label>("spin").style.display = DisplayStyle.None; }
            if (activeWorkspace.workspaceInfoScript.isExperience) { rootVisualElement.Q<Label>("exp").style.display = DisplayStyle.Flex; } else { rootVisualElement.Q<Label>("exp").style.display = DisplayStyle.None; }
		}
		else
		{
            return;
		}

    }

	private void OnGUI()
	{
        WSPSessionInfo activeWorkspace = WorkspaceManager.activeWorkspace;
        if (activeWorkspace == null)
		{
            rootVisualElement.Q<VisualElement>("activeWrap").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<VisualElement>("exportWrap").style.display = DisplayStyle.None;
        }
		else
		{
            rootVisualElement.Q<VisualElement>("activeWrap").style.display = DisplayStyle.None;
            rootVisualElement.Q<VisualElement>("exportWrap").style.display = DisplayStyle.Flex;
            if (currentlyShownSpace != activeWorkspace) { LoadWorkspaceInfo(); }

            if (currentlyShownSpace.workspaceInfoScript.isExperience) { WarningPreventExport(currentlyShownSpace.isValidEXPTag()); }
            else { WarningPreventExport(true); }
        }
    }

    private void WarningPreventExport(bool state)
	{
		if (state)
		{
            rootVisualElement.Q<Button>("exportBundled").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<Button>("exportSeperate").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<VisualElement>("format").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<VisualElement>("helpBoxWrap").style.display = DisplayStyle.None;
		}
		else
		{
            rootVisualElement.Q<Button>("exportBundled").style.display = DisplayStyle.None;
            rootVisualElement.Q<Button>("exportSeperate").style.display = DisplayStyle.None;
            rootVisualElement.Q<VisualElement>("format").style.display = DisplayStyle.None;
            rootVisualElement.Q<VisualElement>("helpBoxWrap").style.display = DisplayStyle.Flex;
        }
	}
}

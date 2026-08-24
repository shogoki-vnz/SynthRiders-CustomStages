using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Text;
using System.Linq;
using Synth.Utils;

namespace SRToolkit
{
    public class SpaceFileManager : MonoBehaviour
    {
        public static WSPSessionInfo newlyCreated { get; set; }
        public static void createNewWorkspace(string spaceName, string spaceCreator, bool createfromScratch, bool createRAW)
		{
            WSPSessionInfo newItem = WSPSessionInfo.CreateInstance<WSPSessionInfo>();
            if (createfromScratch)
			{
                if (AssetDatabase.IsValidFolder("Assets/SRStageWrench/SceneFiles" + "/" + spaceName))
                {
                    Debug.LogError("Cannot make new Workspace with the same name as an already existing Folder structure! Either Choose a Different Name or Delete the folder from Assets/SRStageWrench/SceneFiles!");
                }
                else
                {
                    Debug.Log("Creating New Workspace: " + spaceName);

                    string newSpacePath = "Assets/SRStageWrench/SceneFiles/" + spaceName;
                    string newCSElements = newSpacePath + "/_CustomStageElements";

                    newItem.workspaceName = spaceName;
                    newItem.workspaceCreator = spaceCreator;
                    newItem.workspacePath = newSpacePath;
                    newItem.dateCreated = DateTime.Now.ToString("yyyyMMd-hmmss");
                    AssetDatabase.CreateFolder("Assets/SRStageWrench/SceneFiles", spaceName);
                    AssetDatabase.CreateFolder(newSpacePath, "_CustomStageElements");
                    AssetDatabase.CreateFolder(newCSElements, "Prefabs");
                    AssetDatabase.Refresh();
                    createCSInfoPrefab(newCSElements + "/Prefabs/");
                    newItem.workspaceStageInfo = AssetDatabase.LoadAssetAtPath<GameObject>(newCSElements + "/Prefabs/CustomStageInfo.prefab");
                    AssetDatabase.CreateAsset(newItem, "Assets/SRStageWrench/Editor/WorkspaceInfo/" + $"{newItem.workspaceName}.asset");
                }
			}
            else if (createRAW)
			{
                Debug.Log("Creating new RAW Workspace: " + spaceName);

                string newSpacePath = "Assets/SRStageWrench/SceneFiles/" + spaceName;
                string newCSElements = newSpacePath + "/_CustomStageElements";

                newItem.workspaceName = spaceName;
                newItem.workspacePath = newSpacePath;
                newItem.workspaceCreator = "Andromeda";
                newItem.dateCreated = DateTime.Now.ToString("yyyyMMd-hmmss");
                AssetDatabase.CreateAsset(newItem, "Assets/SRStageWrench/Editor/WorkspaceInfo/" + $"{newItem.workspaceName}.asset");
            }
			else if(!createfromScratch && !createRAW)
			{
                if (AssetDatabase.IsValidFolder("Assets/SRStageWrench/SceneFiles" + "/" + spaceName))
                {
                    Debug.LogError("Cannot make new Workspace with the same name as an already existing Folder structure! Either Choose a Different Name  or Delete the folder from Assets/SRStageWrench/SceneFiles!");
				}
				else
				{
                    Debug.Log("Creating New Workspace: " + spaceName);

                    string newSpacePath = "Assets/SRStageWrench/SceneFiles/" + spaceName;
                    string newCSElements = newSpacePath + "/_CustomStageElements";

                    newItem.workspaceName = spaceName;
                    newItem.workspaceCreator = spaceCreator;
                    newItem.workspacePath = newSpacePath;
                    newItem.dateCreated = System.DateTime.Now.ToString("yyyyMMd-hmmss");
                    AssetDatabase.CreateFolder("Assets/SRStageWrench/SceneFiles", spaceName);

                    AssetDatabase.Refresh();
                    //createCSInfoPrefab(newCSElements + "/Prefabs/");
                    //newItem.workspaceStageInfo = AssetDatabase.LoadAssetAtPath<GameObject>(newCSElements + "/Prefabs/CustomStageInfo.prefab");
                    AssetDatabase.CreateAsset(newItem, "Assets/SRStageWrench/Editor/WorkspaceInfo/" + $"{newItem.workspaceName}.asset");
                }
            }
            newlyCreated = newItem;
		}
        public static void ImportAnotherProject(string path, string workspaceName, string workspaceCreator)
		{
            EditorSceneManager.NewPreviewScene();
            if (!AssetDatabase.IsValidFolder("Assets/SRStageWrench/tempImport")) { AssetDatabase.CreateFolder("Assets/SRStageWrench/", "tempImport"); }
			if (Directory.Exists(path + "/_CustomStageElements"))
			{
                string tempImport = "Assets/SRStageWrench/tempImport";
                string builtDependsPath = "Assets/SRStageWrench/tempImport/_CustomStageElements/Depends";
                FileUtil.CopyFileOrDirectory(path, "Assets/SRStageWrench/tempImport");
                if (Directory.Exists(tempImport + "/_CustomStageElements/Scripts")) { AssetDatabase.DeleteAsset(tempImport + "/_CustomStageElements/Scripts"); }
                if (Directory.Exists(tempImport + "/DemoScene/Scripts")) { AssetDatabase.DeleteAsset(tempImport + "/DemoScene/Scripts"); }
                if (Directory.Exists(tempImport + "/DOTween")) { AssetDatabase.DeleteAsset(tempImport + "/DOTween"); }
                if (Directory.Exists(tempImport + "/Scripts")) { AssetDatabase.DeleteAsset(tempImport + "/Scripts"); }
                if (Directory.Exists(tempImport + "/_TerrainAutoUpgrade")) { AssetDatabase.DeleteAsset(tempImport + "/_TerrainAutoUpgrade"); }
                if (Directory.Exists(tempImport + "/VRTK")) { AssetDatabase.DeleteAsset(tempImport + "/VRTK"); }
                AssetDatabase.Refresh();

                string[] prefabs = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/SRStageWrench/tempImport/_CustomStageElements/Prefabs" });

                //Rename the Prefabs to Hopefully cut down on same name prefabs
                //getting their references messed up
                Debug.Log("Renaming Prefabs!");
                foreach (string prefab in prefabs)
                {
                    string toRename = $"{workspaceName}.{prefab.ToString()}";
                    string pathtoAsset = AssetDatabase.GUIDToAssetPath(prefab);
                    Debug.Log("GetFileName: " + Path.GetFileName(pathtoAsset));
                    Debug.Log("Renaming prefabs to: " + toRename);
                    AssetDatabase.RenameAsset(pathtoAsset, $"{workspaceName}.{Path.GetFileName(pathtoAsset)}");
                }

                string[] scenes = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets/SRStageWrench/tempImport" });
                Debug.Log("Renaming Scenes!");
                foreach(string scene in scenes)
				{
                    string pathtoAsset = AssetDatabase.GUIDToAssetPath(scene);
                    AssetDatabase.MoveAsset(pathtoAsset, tempImport + $"/{Path.GetFileName(pathtoAsset)}");
                    Debug.Log("Get Full Path: " + pathtoAsset);
                    string toRename = $"{workspaceName}.{Path.GetFileName(pathtoAsset)}";
                    Debug.Log("Renaming Scene to: " + toRename);
                    AssetDatabase.RenameAsset(pathtoAsset, toRename);
                }
                //Rename the Skybox to prevent incorrect References
                string[] skyboxs = AssetDatabase.FindAssets("CustomSkybox t:Material", new string[] { "Assets/SRStageWrench/tempImport" });
                foreach (string skybox in skyboxs)
                {
                    string toRename = $"{workspaceName}.{skybox.ToString()}";
                    Debug.Log("Renaming Skybox to: " + toRename);
                    string pathtoAsset = AssetDatabase.GUIDToAssetPath(skybox);
                    Debug.Log("GetFileName: " + Path.GetFileName(pathtoAsset));
                    AssetDatabase.RenameAsset(pathtoAsset, $"{workspaceName}.{Path.GetFileName(pathtoAsset)}");
                }
                if (!Directory.Exists(builtDependsPath)) { AssetDatabase.CreateFolder("Assets/SRStageWrench/tempImport/_CustomStageElements", "Depends"); }
                foreach (string getDependents in GatherPrefabDependencies(prefabs))
				{
                    AssetDatabase.MoveAsset(getDependents, builtDependsPath);
                }
                //Create the New Workspace
                createNewWorkspace(workspaceName, workspaceCreator, false, false);
                AssetDatabase.Refresh();

                //Move all Directories from Tempimport folder to the new Workspace's folder structure
                foreach (string folder in Directory.GetDirectories(tempImport + "/"))
				{
                    Debug.Log(folder);
                    string dirName = new DirectoryInfo(folder).Name;
                    Debug.Log("Directory name: " + dirName);
                    string moveTo = "Assets/SRStageWrench/SceneFiles/" + workspaceName + "/" + dirName;
                    Debug.Log("FullPath: " + moveTo);
                    FileUtil.MoveFileOrDirectory(folder, moveTo);
                }
                //Move all other file Types from Tempimport to the New Workspace's folder structure
                foreach (string asset in Directory.GetFiles(tempImport + "/"))
                {
                    string pathtoAsset = AssetDatabase.GUIDToAssetPath(asset);
                    //if (asset.Contains(".meta")) { Debug.LogError("Skipping meta files!"); }
                    if (asset.Contains(".unity")) { Debug.Log("SceneFile! Renaming it!"); AssetDatabase.RenameAsset(pathtoAsset, $"{workspaceName}.{Path.GetFileName(pathtoAsset)}"); }
					else
					{
                        Debug.Log(asset);
                        string assetName = Path.GetFileName(asset);
                        Debug.Log("Filename: " + assetName);
                        string moveTo = "Assets/SRStageWrench/SceneFiles/" + workspaceName + "/" + assetName;
                        Debug.Log("FullPath: " + moveTo);
                        FileUtil.MoveFileOrDirectory(asset, moveTo);
                    }
                }
                //Create a New CustomStageInfo prefab. TODO: detect if imported CSINFO has missing scripts. if the script is missing,
                //it means it is a legacy stage and a new CSINFO should be created. but if the script loads fine, assume it's the latest one
                createCSInfoPrefab($"{newlyCreated.workspacePath}/_CustomStageElements/Prefabs/");
                newlyCreated.workspaceStageInfo = AssetDatabase.LoadMainAssetAtPath($"{newlyCreated.workspacePath}/_CustomStageElements/Prefabs/CustomStageInfo.prefab") as GameObject;
                AssetDatabase.Refresh();
                //Kill the TempImport folder. TODO: maybe make this a debug toggle?
                AssetDatabase.DeleteAsset(tempImport);
            } else { Debug.LogError("The Selected path doesn't have the _CustomStageElements Folder!"); }
            
		}

        public static void MergeWorkSpaces(WSPSessionInfo mergeFrom, WSPSessionInfo mergeTo)
		{
            Debug.Log("Merging " + mergeFrom.workspaceName + " into " + mergeTo.workspaceName);
			if (Directory.Exists(mergeFrom.workspacePath))
			{
                Debug.Log("Directory Exists!");
                FileUtil.MoveFileOrDirectory(mergeFrom.workspacePath, mergeTo.workspacePath + "/Merged");
                AssetDatabase.DeleteAsset(mergeFrom.workspacePath);
                AssetDatabase.Refresh();
                DeleteWorkspace(mergeFrom, false);
			}
		}

        public static void CloneWorkspace(WSPSessionInfo toClone)
		{
            Debug.Log("Cloning Workspace: " + toClone.workspaceName);
            string cloneName = null;
            if (Directory.Exists(toClone.workspacePath))
			{
                string[] spaces = AssetDatabase.FindAssets("t:ScriptableObject.WSPSessionInfo", new string[] { "Assets/SRStageWrench/Editor/WorkspaceInfo/" });
                int duplicates = 0;
                foreach(string space in spaces)
				{
                    string Init = AssetDatabase.GUIDToAssetPath(space);
					if (Init.Contains(toClone.workspaceName)) { duplicates++; }
				}
                cloneName = $"{toClone.workspaceName}({duplicates})";
                Debug.Log("Cloned Workspace name is: " + cloneName);

                createNewWorkspace(cloneName, toClone.workspaceCreator, false, true);
                WSPSessionInfo clonedSpace = FindWorkspace(cloneName);
                FileUtil.CopyFileOrDirectory(toClone.workspacePath, clonedSpace.workspacePath);
                AssetDatabase.Refresh();

                string[] containedScripts = AssetDatabase.FindAssets("t:Script", new string[] { clonedSpace.workspacePath });
                foreach(string script in containedScripts) { AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(script)); }

                string[] prefabs = AssetDatabase.FindAssets("t:prefab", new string[] { clonedSpace.workspacePath + "/_CustomStageElements/Prefabs" });
                string foundCSInfo = null;
                foreach(string prefab in prefabs)
				{
                    GameObject currentPrefab = (GameObject)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(prefab));
                    Debug.Log("Current: " + currentPrefab.name);
                    if (currentPrefab.name.Contains(toClone.name)) { AssetDatabase.RenameAsset(AssetDatabase.GUIDToAssetPath(prefab), currentPrefab.name.Replace(toClone.name, cloneName)); }
					else { }

                    if (currentPrefab.GetComponent<CustomStageInfo>() != null) { foundCSInfo = AssetDatabase.GetAssetPath(currentPrefab); }
				}
                clonedSpace.workspaceStageInfo = (GameObject)AssetDatabase.LoadMainAssetAtPath(foundCSInfo);
                EditorUtility.SetDirty(clonedSpace);
            }
		}

        public static void DeleteWorkspace(WSPSessionInfo spaceToDelete, bool deleteFolder)
		{
            Debug.Log("Deleting Workspace: " + spaceToDelete.ToString() + "!");
            string path = AssetDatabase.GetAssetPath(spaceToDelete);
            string workspaceDIR = spaceToDelete.workspacePath;
            AssetDatabase.DeleteAsset(path);
            if (deleteFolder)
			{
                Debug.Log("Deleting Workspace's Files");
                AssetDatabase.DeleteAsset(workspaceDIR);
			}else { Debug.Log("Not going to delete Workspace's Files"); }

		}
        public static void createCSInfoPrefab(string savePath)
		{
            Debug.Log("Creating CustomStageInfo Prefab at: " + savePath);
            GameObject go = new GameObject();
            go.AddComponent<CustomStageInfo>();
            PrefabUtility.SaveAsPrefabAsset(go, savePath + "CustomStageInfo.Prefab");
            GameObject.DestroyImmediate(go);
        }


        public static WSPSessionInfo FindWorkspace(string workspaceName)
		{
            WSPSessionInfo foundSpace = null;
            string[] spaces = AssetDatabase.FindAssets("t:ScriptableObject.WSPSessionInfo", new string[] { "Assets/SRStageWrench/Editor/WorkspaceInfo/" });
            foreach(string space in spaces)
			{
                WSPSessionInfo currentPointer = (WSPSessionInfo)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(space));
                if (currentPointer.workspaceName == workspaceName) { foundSpace = currentPointer; }
			}
            return foundSpace;
		}
        public static List<string> GatherPrefabDependencies(IList<string> prefabs)
		{
            List<string> dependencies = new List<string>();
            foreach (string prefabObject in prefabs)
		    {
                Debug.Log($"Working on: {AssetDatabase.GUIDToAssetPath(prefabObject)}");
                string[] prefabDepends = AssetDatabase.GetDependencies(prefabObject);
                foreach(string dependObjects in prefabDepends)
				{
                    //Debug.Log("Dependency: " + dependObjects);
                    if (dependObjects == prefabObject) { /*Debug.LogError("Skipping Native Prefab!");*/}
					else
					{
                        dependencies.Add(dependObjects);
                    }
                }
            }
            List<StringBuilder> listedDepends = dependencies.Select(s => new StringBuilder(s)).ToList();
			Debug.Log("prefabs depend on: " + string.Join(", ", listedDepends.Select(sb => $"{sb.ToString()}/n")));
            return dependencies;
		}

        private static int missingCount;
        public static void CheckforMissingScripts(GameObject go, string prefabName = "")
        {
            Component[] components = go.GetComponents<Component>();
            missingCount = 0;
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    missingCount++;
                    Transform t = go.transform;

                    string componentPath = go.name;
                    while (t.parent != null)
                    {
                        componentPath = t.parent.name + "/" + componentPath;
                        t = t.parent;
                    }
                    Debug.LogWarning("Prefab " + prefabName + " has an empty script attached:\n" + componentPath, go);
                }
            }

            foreach (Transform child in go.transform)
            {
                CheckforMissingScripts(child.gameObject, prefabName);
            }
        }
    }
}


using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneTemplate;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using SRToolkit;

namespace SRToolkit
{
    public class StageSceneManager : MonoBehaviour
    {
        public static SceneTemplateAsset normTemplate;
        public static SceneTemplateAsset spinTemplate;
        public static SceneTemplateAsset expTemplate;
        public static void CreateNormalScene(WSPSessionInfo forWorkspace)
		{
            //Load SceneTemplate Asset
            normTemplate = AssetDatabase.LoadMainAssetAtPath("Assets/SRStageWrench/Editor/SceneTemplates/NormalStage.scenetemplate") as SceneTemplateAsset;

            //Workspace Scene folder
            string workspaceScenes = $"{forWorkspace.workspacePath}/WorkspaceScenes";
            if (!AssetDatabase.IsValidFolder(workspaceScenes)) { AssetDatabase.CreateFolder(forWorkspace.workspacePath, "WorkspaceScenes"); }
            SceneTemplateService.Instantiate(normTemplate, false, $"{forWorkspace.workspacePath}/WorkspaceScenes/{forWorkspace.workspaceName}.NormStage.unity");
            forWorkspace.normStageScene = AssetDatabase.LoadMainAssetAtPath($"{forWorkspace.workspacePath}/WorkspaceScenes/{forWorkspace.workspaceName}.NormStage.unity") as SceneAsset;
        }

        public static void CreateSpinScene(WSPSessionInfo forWorkspace)
        {
            //Load SceneTemplate Asset
            spinTemplate = AssetDatabase.LoadMainAssetAtPath("Assets/SRStageWrench/Editor/SceneTemplates/SpinStage.scenetemplate") as SceneTemplateAsset;

            //Workspace Scene folder
            string workspaceScenes = $"{forWorkspace.workspacePath}/WorkspaceScenes";
            if (!AssetDatabase.IsValidFolder(workspaceScenes)) { AssetDatabase.CreateFolder(forWorkspace.workspacePath, "WorkspaceScenes"); }
            SceneTemplateService.Instantiate(spinTemplate, false, $"{forWorkspace.workspacePath}/WorkspaceScenes/{forWorkspace.workspaceName}.SpinStage.unity");
            forWorkspace.spinStageScene = AssetDatabase.LoadMainAssetAtPath($"{forWorkspace.workspacePath}/WorkspaceScenes/{forWorkspace.workspaceName}.SpinStage.unity") as SceneAsset;
        }

        public static void CreateExperienceScene(WSPSessionInfo forWorkspace)
        {
            //Load SceneTemplate Asset
            expTemplate = AssetDatabase.LoadMainAssetAtPath("Assets/SRStageWrench/Editor/SceneTemplates/ExpStage.scenetemplate") as SceneTemplateAsset;

            //Workspace Scene folder
            string workspaceScenes = $"{forWorkspace.workspacePath}/WorkspaceScenes";
            if (!AssetDatabase.IsValidFolder(workspaceScenes)) { AssetDatabase.CreateFolder(forWorkspace.workspacePath, "WorkspaceScenes"); }
            SceneTemplateService.Instantiate(expTemplate, false, $"{forWorkspace.workspacePath}/WorkspaceScenes/{forWorkspace.workspaceName}.ExpStage.unity");
            forWorkspace.expStageScene = AssetDatabase.LoadMainAssetAtPath($"{forWorkspace.workspacePath}/WorkspaceScenes/{forWorkspace.workspaceName}.ExpStage.unity") as SceneAsset;
        }

        public static void DeleteScene(string scenePath)
		{
            Debug.Log("DELETING SCENE AT: " + scenePath);
            AssetDatabase.DeleteAsset(scenePath);
		}

        public static void SetupScenes(WSPSessionInfo toInfo, bool createObjects, bool normalStage, bool spinStage, bool expStage)
		{
            if (normalStage)
			{
                CreateNormalScene(toInfo);
                if (createObjects)
				{
                    CreateStageObjects.CreateAllStageTilesandParent(toInfo, GameObject.Find("TileWrap1"), GameObject.Find("TileWrap2"), GameObject.Find("TileWrap3"));
                    CreateStageObjects.CreateNormPlatform(toInfo);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
				}
			}
            if (spinStage)
			{
                CreateSpinScene(toInfo);
                if (createObjects)
				{
                    CreateStageObjects.CreateSpinEmitter(toInfo);
                    CreateStageObjects.CreateSpinPlatform(toInfo);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }
			}
            if (expStage)
			{
                CreateExperienceScene(toInfo);
                if (createObjects)
				{
                    CreateStageObjects.CreateExpTimeline(toInfo);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }
			}
		}
    }
}


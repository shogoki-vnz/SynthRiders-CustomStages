using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SRToolkit
{
    public class CreateStageObjects : MonoBehaviour
    {

        public static void CreateAllStageTiles(WSPSessionInfo workspace)
		{
            CreateStageTile01(workspace, null);
            CreateStageTile02(workspace, null);
            CreateStageTile03(workspace, null);
		}
        public static void CreateAllStageTilesandParent(WSPSessionInfo workspace, GameObject tile01Parent, GameObject tile02Parent, GameObject tile03Parent)
        {
            CreateStageTile01(workspace, tile01Parent);
            CreateStageTile02(workspace, tile02Parent);
            CreateStageTile03(workspace, tile03Parent);
        }
        public static void CreateStageTile01(WSPSessionInfo workspace, GameObject setParent)
		{
            // Create a custom game object
            GameObject go = new GameObject("StageTile01");
            GameObject StageElements = new GameObject("StageTileElements");
            GameObjectUtility.SetParentAndAlign(StageElements, go);
            if (setParent != null)
            {
                GameObjectUtility.SetParentAndAlign(go, setParent);
                PrefabUtility.SaveAsPrefabAssetAndConnect(go, $"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.StageTile01.prefab", InteractionMode.UserAction);
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(go, $"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.StageTile01.prefab");
            }
            workspace.workspaceInfoScript.normTile01 = AssetDatabase.LoadMainAssetAtPath($"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.StageTile01.prefab") as GameObject;
        }
        public static void CreateStageTile02(WSPSessionInfo workspace, GameObject setParent)
        {
            // Create a custom game object
            GameObject go = new GameObject("StageTile02");
            GameObject StageElements = new GameObject("StageTileElements");
            GameObjectUtility.SetParentAndAlign(StageElements, go);
            if (setParent != null)
            {
                GameObjectUtility.SetParentAndAlign(go, setParent);
                PrefabUtility.SaveAsPrefabAssetAndConnect(go, $"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.StageTile02.prefab", InteractionMode.UserAction);
			}
			else
			{
                PrefabUtility.SaveAsPrefabAsset(go, $"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.StageTile02.prefab");
            }
            workspace.workspaceInfoScript.normTile02 = AssetDatabase.LoadMainAssetAtPath($"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.StageTile02.prefab") as GameObject;
        }
        public static void CreateStageTile03(WSPSessionInfo workspace, GameObject setParent)
        {
            // Create a custom game object
            GameObject go = new GameObject("StageTile03");
            GameObject StageElements = new GameObject("StageTileElements");
            GameObjectUtility.SetParentAndAlign(StageElements, go);
            if (setParent != null)
            {
                GameObjectUtility.SetParentAndAlign(go, setParent);
                PrefabUtility.SaveAsPrefabAssetAndConnect(go, $"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.StageTile03.prefab", InteractionMode.UserAction);
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(go, $"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.StageTile03.prefab");
            }
            workspace.workspaceInfoScript.normTile03 = AssetDatabase.LoadMainAssetAtPath($"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.StageTile03.prefab") as GameObject;
        }

        public static void CreateNormPlatform(WSPSessionInfo workspace)
        {
            // Create a Platform Gameobject using a prefab as base
            Object ColorObjectPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/SRStageWrench/Editor/Resources/Objects/Platform.prefab");
            GameObject newPlatform = Instantiate(ColorObjectPrefab) as GameObject;
            newPlatform.name = "Platform";
            PrefabUtility.SaveAsPrefabAssetAndConnect(newPlatform, $"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.NormPlatform.prefab", InteractionMode.UserAction);
            workspace.workspaceInfoScript.normplatform = AssetDatabase.LoadMainAssetAtPath($"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.NormPlatform.prefab") as GameObject;
        }
        public static void CreateSpinPlatform(WSPSessionInfo workspace)
        {
            // Create a Platform Gameobject using a prefab as base
            Object ColorObjectPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/SRStageWrench/Editor/Resources/Objects/Platform.prefab");
            GameObject newPlatform = Instantiate(ColorObjectPrefab) as GameObject;
            newPlatform.name = "Platform";
            PrefabUtility.SaveAsPrefabAssetAndConnect(newPlatform, $"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.SpinPlatform.prefab", InteractionMode.UserAction);
            workspace.workspaceInfoScript.spinPlatform = AssetDatabase.LoadMainAssetAtPath($"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.SpinPlatform.prefab") as GameObject;
        }

        public static void CreateSpinEmitter(WSPSessionInfo workspace)
        {
            // Create a Platform Gameobject using a prefab as base
            Object ColorObjectPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/SRStageWrench/Editor/Resources/Objects/SpinEmitter.prefab");
            GameObject newPlatform = Instantiate(ColorObjectPrefab) as GameObject;
            newPlatform.name = "SpinEmitter";
            PrefabUtility.SaveAsPrefabAssetAndConnect(newPlatform, $"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.SpinEmitter.prefab", InteractionMode.UserAction);
            workspace.workspaceInfoScript.spinEmitter = AssetDatabase.LoadMainAssetAtPath($"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.SpinEmitter.prefab") as GameObject;
        }

        public static void CreateExpTimeline(WSPSessionInfo workspace)
        {
            // Create a Platform Gameobject using a prefab as base
            GameObject directorGO = new GameObject("Timeline");
            PlayableDirector director = directorGO.AddComponent<PlayableDirector>();
            TimelineAsset timeline = new TimelineAsset();
            director.playableAsset = timeline;
            AssetDatabase.CreateAsset(timeline, $"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.EXP.playable");
            PrefabUtility.SaveAsPrefabAssetAndConnect(directorGO, $"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.Timeline.prefab", InteractionMode.UserAction);
            workspace.workspaceInfoScript.expPrefab = AssetDatabase.LoadMainAssetAtPath($"{workspace.workspacePath}/_CustomStageElements/Prefabs/{workspace.name}.Timeline.prefab") as GameObject;
        }
    }
}

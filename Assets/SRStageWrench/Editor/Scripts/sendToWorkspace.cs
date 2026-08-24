using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SRToolkit;

public class sendToWorkspace : EditorWindow
{

	[MenuItem("Assets/Workspace/Assign CustomStageInfo")]
	public static void AssigntoCSInfo()
	{
		var selected = Selection.activeObject;
		Debug.Log("Selected asset: " + selected.ToString());

		bool isPrefab = PrefabUtility.IsPartOfPrefabAsset(Selection.activeGameObject);

		if (isPrefab) { WorkspaceManager.activeWorkspace.workspaceStageInfo = Selection.activeGameObject; EditorUtility.SetDirty(WorkspaceManager.activeWorkspace.workspaceStageInfo);  }
		else { Debug.LogError("Cannot send Object to StageInfo bc it is not prefab!"); }
	}


	[MenuItem("GameObject/Workspace/Assign to/NormalStage/StageTile01", false, 10)]
	[MenuItem("Assets/Workspace/Assign to/NormalStage/StageTile01")]
	public static void AssigntoStageTile1()
	{
		var selected = Selection.activeObject;
		Debug.Log("Selected asset: " + selected.ToString());
		bool isPrefab = PrefabUtility.IsPartOfPrefabAsset(Selection.activeGameObject);
		var workspaceStageInfo = WorkspaceManager.activeWorkspace.workspaceStageInfo;
		var stageInfoScript = WorkspaceManager.activeWorkspace.workspaceInfoScript;

		if (isPrefab)
		{
			if (workspaceStageInfo)
			{
				if (!stageInfoScript.isNormStage) { stageInfoScript.isNormStage = true; }
				stageInfoScript.normTile01 = (GameObject)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(Selection.activeGameObject));
				EditorUtility.SetDirty(WorkspaceManager.activeWorkspace.workspaceStageInfo);
			}
			else { Debug.LogError("There is no CustomStageInfo Object Assigned to Workspace!"); }
		}
		else { Debug.LogError("Cannot send Object to StageInfo bc it is not prefab!"); }
	}

	[MenuItem("GameObject/Workspace/Assign to/NormalStage/StageTile02", false, 10)]
	[MenuItem("Assets/Workspace/Assign to/NormalStage/StageTile02")]
	public static void AssigntoStageTile2()
	{
		var selected = Selection.activeObject;
		Debug.Log("Selected asset: " + selected.ToString());
		bool isPrefab = PrefabUtility.IsPartOfPrefabAsset(Selection.activeGameObject);
		var workspaceStageInfo = WorkspaceManager.activeWorkspace.workspaceStageInfo;
		var stageInfoScript = WorkspaceManager.activeWorkspace.workspaceInfoScript;

		if (isPrefab)
		{
			if (workspaceStageInfo)
			{
				if (stageInfoScript)
				{
					if (!stageInfoScript.isNormStage) { stageInfoScript.isNormStage = true; }
					stageInfoScript.normTile02 = (GameObject)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(Selection.activeGameObject));
					EditorUtility.SetDirty(WorkspaceManager.activeWorkspace.workspaceStageInfo);
				}
			}
			else { Debug.LogError("There is no CustomStageInfo Object Assigned to Workspace!"); }
		}
		else { Debug.LogError("Cannot send Object to StageInfo bc it is not prefab!"); }
	}

	[MenuItem("GameObject/Workspace/Assign to/NormalStage/StageTile03", false, 10)]
	[MenuItem("Assets/Workspace/Assign to/NormalStage/StageTile03")]
	public static void AssigntoStageTile3()
	{
		var selected = Selection.activeObject;
		Debug.Log("Selected asset: " + selected.ToString());
		bool isPrefab = PrefabUtility.IsPartOfPrefabAsset(Selection.activeGameObject);
		var workspaceStageInfo = WorkspaceManager.activeWorkspace.workspaceStageInfo;
		var stageInfoScript = WorkspaceManager.activeWorkspace.workspaceInfoScript;

		if (isPrefab)
		{
			if (workspaceStageInfo)
			{
				if (!stageInfoScript.isNormStage) { stageInfoScript.isNormStage = true; }
				stageInfoScript.normTile03 = (GameObject)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(Selection.activeGameObject));
				EditorUtility.SetDirty(WorkspaceManager.activeWorkspace.workspaceStageInfo);
			}
			else { Debug.LogError("There is no CustomStageInfo Object Assigned to Workspace!"); }
		}
		else { Debug.LogError("Cannot send Object to StageInfo bc it is not prefab!"); }
	}

	[MenuItem("GameObject/Workspace/Assign to/NormalStage/Platform", false, 10)]
	[MenuItem("Assets/Workspace/Assign to/NormalStage/Platform")]
	public static void AssigntoNormPlatform()
	{
		var selected = Selection.activeObject;
		Debug.Log("Selected asset: " + selected.ToString());
		bool isPrefab = PrefabUtility.IsPartOfPrefabAsset(Selection.activeGameObject);
		var workspaceStageInfo = WorkspaceManager.activeWorkspace.workspaceStageInfo;
		var stageInfoScript = WorkspaceManager.activeWorkspace.workspaceInfoScript;

		if (isPrefab)
		{
			if (workspaceStageInfo)
			{
				if (!stageInfoScript.isNormStage) { stageInfoScript.isNormStage = true; }
				stageInfoScript.normplatform = (GameObject)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(Selection.activeGameObject));
				EditorUtility.SetDirty(WorkspaceManager.activeWorkspace.workspaceStageInfo);
			}
			else { Debug.LogError("There is no CustomStageInfo Object Assigned to Workspace!"); }
		}
		else { Debug.LogError("Cannot send Object to StageInfo bc it is not prefab!"); }
	}

	[MenuItem("GameObject/Workspace/Assign to/NormalStage/SpecialFX", false, 10)]
	[MenuItem("Assets/Workspace/Assign to/NormalStage/SpecialFX")]
	public static void AssigntoNormSpecial()
	{
		var selected = Selection.activeObject;
		bool isPrefab = PrefabUtility.IsPartOfPrefabAsset(Selection.activeGameObject);
		var workspaceStageInfo = WorkspaceManager.activeWorkspace.workspaceStageInfo;
		var stageInfoScript = WorkspaceManager.activeWorkspace.workspaceInfoScript;

		if (isPrefab)
		{
			if (Selection.activeGameObject.GetComponent<SynthSpecialsFX>())
			{
				if (workspaceStageInfo)
				{
					if (!stageInfoScript.isNormStage) { stageInfoScript.isNormStage = true; }
					stageInfoScript.normSpecialFX = (GameObject)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(Selection.activeGameObject));
					EditorUtility.SetDirty(WorkspaceManager.activeWorkspace.workspaceStageInfo);
				}
			}
			else { Debug.LogError("There is no CustomStageInfo Object Assigned to Workspace!"); }
		}
		else { Debug.LogError("Cannot send Object to StageInfo bc it is not prefab!"); }
	}

	[MenuItem("GameObject/Workspace/Assign to/NormalStage/StageIcon", false, 10)]
	[MenuItem("Assets/Workspace/Assign to/NormalStage/StageIcon")]
	public static void AssigntoNormIcon()
	{
		var selected = Selection.activeObject;
		var workspaceStageInfo = WorkspaceManager.activeWorkspace.workspaceStageInfo;
		var stageInfoScript = WorkspaceManager.activeWorkspace.workspaceInfoScript;

		if (Selection.activeObject is Texture2D)
		{
			if (workspaceStageInfo)
			{
				if (!stageInfoScript.isNormStage) { stageInfoScript.isNormStage = true; }
				stageInfoScript.normStageIcon = Selection.activeObject as Texture2D;
				EditorUtility.SetDirty(WorkspaceManager.activeWorkspace.workspaceStageInfo);
			}
			else { Debug.LogError("There is no CustomStageInfo Object Assigned to Workspace!"); }
		}
		else { Debug.LogError("Asset is not a Texture2D!"); }
	}

	[MenuItem("GameObject/Workspace/Assign to/SpinStage/Platform", false, 10)]
	[MenuItem("Assets/Workspace/Assign to/SpinStage/Platform")]
	public static void AssigntoSpinPlatform()
	{
		bool isPrefab = PrefabUtility.IsPartOfPrefabAsset(Selection.activeGameObject);
		var workspaceStageInfo = WorkspaceManager.activeWorkspace.workspaceStageInfo;
		var stageInfoScript = WorkspaceManager.activeWorkspace.workspaceInfoScript;

		if (isPrefab)
		{
			if (workspaceStageInfo)
			{
				if (!stageInfoScript.isSpinStage) { stageInfoScript.isSpinStage = true; }
				stageInfoScript.spinPlatform = (GameObject)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(Selection.activeGameObject));
				EditorUtility.SetDirty(WorkspaceManager.activeWorkspace.workspaceStageInfo);
			}
			else { Debug.LogError("There is no CustomStageInfo Object Assigned to Workspace!"); }
		}
		else { Debug.LogError("Cannot send Object to StageInfo bc it is not prefab!"); }
	}

	[MenuItem("GameObject/Workspace/Assign to/SpinStage/Spin Emitter", false, 10)]
	[MenuItem("Assets/Workspace/Assign to/SpinStage/Spin Emitter")]
	public static void AssigntoSpinEmitter()
	{
		bool isPrefab = PrefabUtility.IsPartOfPrefabAsset(Selection.activeGameObject);
		var workspaceStageInfo = WorkspaceManager.activeWorkspace.workspaceStageInfo;
		var stageInfoScript = WorkspaceManager.activeWorkspace.workspaceInfoScript;

		if (isPrefab)
		{
			if (workspaceStageInfo)
			{
				if (!stageInfoScript.isSpinStage) { stageInfoScript.isSpinStage = true; }
				stageInfoScript.spinEmitter = (GameObject)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(Selection.activeGameObject));
				EditorUtility.SetDirty(WorkspaceManager.activeWorkspace.workspaceStageInfo);
			}
			else { Debug.LogError("There is no CustomStageInfo Object Assigned to Workspace!"); }
		}
		else { Debug.LogError("Cannot send Object to StageInfo bc it is not prefab!"); }
	}

	[MenuItem("GameObject/Workspace/Assign to/SpinStage/SpecialFX", false, 10)]
	[MenuItem("Assets/Workspace/Assign to/SpinStage/SpecialFX")]
	public static void AssigntoSpinSpecial()
	{
		Object parentObject = PrefabUtility.GetCorrespondingObjectFromOriginalSource(Selection.activeGameObject); 
		string path = PrefabUtility.GetCorrespondingObjectFromOriginalSource(Selection.activeGameObject).ToString();
		Debug.LogError("prefab path:" + path);
		bool isPrefab = PrefabUtility.IsPartOfPrefabAsset(Selection.activeGameObject);
		var workspaceStageInfo = WorkspaceManager.activeWorkspace.workspaceStageInfo;
		var stageInfoScript = WorkspaceManager.activeWorkspace.workspaceInfoScript;

		if (isPrefab)
		{
			if (Selection.activeGameObject.GetComponent<SynthSpecialsFX>())
			{
				if (workspaceStageInfo)
				{
					if (!stageInfoScript.isSpinStage) { stageInfoScript.isSpinStage = true; }
					stageInfoScript.spinSpecialFX = (GameObject)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(Selection.activeGameObject));
					EditorUtility.SetDirty(WorkspaceManager.activeWorkspace.workspaceStageInfo);
				}
			}
			else { Debug.LogError("There is no SynthSpecialFX script Assigned!"); }
		}
		else { Debug.LogError("Cannot send Object to StageInfo bc it is not prefab!"); }
	}

	[MenuItem("GameObject/Workspace/Assign to/SpinStage/StageIcon", false, 10)]
	[MenuItem("Assets/Workspace/Assign to/SpinStage/StageIcon")]
	public static void AssigntoSpinIcon()
	{
		var selected = Selection.activeObject;
		var workspaceStageInfo = WorkspaceManager.activeWorkspace.workspaceStageInfo;
		var stageInfoScript = WorkspaceManager.activeWorkspace.workspaceInfoScript;

		if (Selection.activeObject is Texture2D)
		{
			if (workspaceStageInfo)
			{
				if (!stageInfoScript.isSpinStage) { stageInfoScript.isSpinStage = true; }
				stageInfoScript.spinStageIcon = Selection.activeObject as Texture2D;
				EditorUtility.SetDirty(WorkspaceManager.activeWorkspace.workspaceStageInfo);
			}
			else { Debug.LogError("There is no CustomStageInfo Object Assigned to Workspace!"); }
		}
		else { Debug.LogError("Asset is not a Texture2D!"); }
	}
}

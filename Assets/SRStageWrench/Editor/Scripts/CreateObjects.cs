using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SRToolkit;

public class CreateObjects : EditorWindow
{
	[MenuItem("GameObject/SR Stage Objects/StageTile", false, 10)]
	static void CreateCustomGameObject(MenuCommand menuCommand)
	{
		// Create a custom game object
		GameObject go = new GameObject("StageTile");
		// Ensure it gets reparented if this was a context click (otherwise does nothing)
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		PrefabUtility.SaveAsPrefabAssetAndConnect(go, WorkspaceManager.activeWorkspace + "/_CustomStageElements/Prefabs", InteractionMode.UserAction);
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

	[MenuItem("GameObject/SR Stage Objects/Custom Stage Info", false, 10)]
	static void CreateCustomStageInfo(MenuCommand menuCommand)
	{
		// Create a custom game object
		GameObject go = new GameObject("CustomStageInfo");
		// Ensure it gets reparented if this was a context click (otherwise does nothing)
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		go.AddComponent<Synth.Utils.CustomStageInfo>();
		PrefabUtility.SaveAsPrefabAssetAndConnect(go, WorkspaceManager.activeWorkspace.workspacePath + "/_CustomStageElements/Prefabs/CustomStageInfo.prefab", InteractionMode.UserAction);
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
		SRToolkit.WorkspaceManager.activeWorkspace.workspaceStageInfo = (GameObject)AssetDatabase.LoadMainAssetAtPath(SRToolkit.WorkspaceManager.activeWorkspace.workspacePath + "/_CustomStageElements/Prefabs/CustomStageInfo.prefab");
		EditorUtility.SetDirty(SRToolkit.WorkspaceManager.activeWorkspace);
	}

	[MenuItem("GameObject/SR Stage Objects/SpecialFX", false, 10)]
	static void CreateSpecialFX(MenuCommand menuCommand)
	{
		// Create a custom game object
		GameObject go = new GameObject("SynthSpecial");
		// Ensure it gets reparented if this was a context click (otherwise does nothing)
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		go.AddComponent<SynthSpecialsFX>();
		PrefabUtility.SaveAsPrefabAssetAndConnect(go, WorkspaceManager.activeWorkspace.workspacePath + "/_CustomStageElements/Prefabs/SpecialFX.prefab", InteractionMode.UserAction);
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}


	[MenuItem("GameObject/SR Stage Objects/StageEvents/StageEvents", false, 10)]
	static void CreateStageEvents(MenuCommand menuCommand)
	{
		// Create a custom game object
		GameObject go = new GameObject("NormalStageEvents");
		go.AddComponent<Synth.Utils.StageEvents>();

		GameObject find = GameObject.FindGameObjectWithTag("StageEvents");
		//Debug.Log(find);
		if (find) { GameObjectUtility.SetParentAndAlign(go, find); }
		else
		{
			GameObject parent = new GameObject("StageEvents");
			parent.tag = "StageEvents";
			Undo.RegisterCreatedObjectUndo(parent, "Create Parent" + go.name);
			GameObjectUtility.SetParentAndAlign(go, parent);
		}
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

	[MenuItem("GameObject/SR Stage Objects/StageEvents/ComboEvents", false, 10)]
	static void CreateComboEvents(MenuCommand menuCommand)
	{
		// Create a custom game object
		GameObject go = new GameObject("ComboEvents");
		go.AddComponent<Synth.Utils.StageComboEvents>();

		GameObject find = GameObject.FindGameObjectWithTag("StageEvents");
		//Debug.Log(find);
		if (find){ GameObjectUtility.SetParentAndAlign(go, find); }
		else
		{
			GameObject parent = new GameObject("StageEvents");
			parent.tag = "StageEvents";
			Undo.RegisterCreatedObjectUndo(parent, "Create Parent" + go.name);
			GameObjectUtility.SetParentAndAlign(go, parent);
		}
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

	[MenuItem("GameObject/SR Stage Objects/StageEvents/ScoreEvents", false, 10)]
	static void CreateScoreEvents(MenuCommand menuCommand)
	{
		// Create a custom game object
		GameObject go = new GameObject("ScoreEvents");
		go.AddComponent<Synth.Utils.StageScoreEvents>();

		GameObject find = GameObject.FindGameObjectWithTag("StageEvents");
		//Debug.Log(find);
		if (find) { GameObjectUtility.SetParentAndAlign(go, find); }
		else
		{
			GameObject parent = new GameObject("StageEvents");
			parent.tag = "StageEvents";
			Undo.RegisterCreatedObjectUndo(parent, "Create Parent" + go.name);
			GameObjectUtility.SetParentAndAlign(go, parent);
		}
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

	[MenuItem("GameObject/SR Stage Objects/StageEvents/TimeEvents", false, 10)]
	static void CreateTimeEvents(MenuCommand menuCommand)
	{
		// Create a custom game object
		GameObject go = new GameObject("TimeEvents");
		go.AddComponent<Synth.Utils.StageTimeEvents>();

		GameObject find = GameObject.FindGameObjectWithTag("StageEvents");
		//Debug.Log(find);
		if (find) { GameObjectUtility.SetParentAndAlign(go, find); }
		else
		{
			GameObject parent = new GameObject("StageEvents");
			parent.tag = "StageEvents";
			Undo.RegisterCreatedObjectUndo(parent, "Create Parent" + go.name);
			GameObjectUtility.SetParentAndAlign(go, parent);
		}
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

	[MenuItem("Assets/Create/SRToolkit/CustomStageInfo.prefab", false, 0)]
	static void CreateCSINFOPrefab(MenuCommand context)
	{
		string filePath;
		if (Selection.assetGUIDs.Length == 0)
			filePath = "Assets/";
		else
			filePath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]) + "/";
		SpaceFileManager.createCSInfoPrefab(filePath);
	}
}

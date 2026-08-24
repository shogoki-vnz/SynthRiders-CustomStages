using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

[CreateAssetMenu(fileName = "New Workspace", menuName = "Debug/Manually Create Workspace", order = 1)]
public class WSPSessionInfo : ScriptableObject
{
	public string workspaceName;
	public string workspaceCreator;
	public string workspacePath;

	public string dateCreated;
	public GameObject workspaceStageInfo;

	public SceneAsset normStageScene;
	public SceneAsset spinStageScene;
	public SceneAsset expStageScene;
	public Synth.Utils.CustomStageInfo workspaceInfoScript { get { return workspaceStageInfo.GetComponent<Synth.Utils.CustomStageInfo>(); } }

	public bool isValidEXPTag()
	{
		bool isValid = false;
		if (this.workspaceInfoScript.isExperience && !string.IsNullOrEmpty(this.workspaceInfoScript.expTag))
		{
			if (this.workspaceInfoScript.expTag.Length >= 12)
			{
				isValid = true;
			}
		}
		else
		{
			isValid = false;
		}
		return isValid;
	}
}

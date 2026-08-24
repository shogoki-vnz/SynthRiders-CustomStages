using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createJSON : MonoBehaviour
{

}

[System.Serializable]
public class jsonStageData
{
	public string creatorName;
	public string exportedOn;
	public platformValues pc = new platformValues();
	public platformValues android = new platformValues();
}

[System.Serializable]
public class platformValues
{
	public bool available = false;
	public string fileName;
	public bool isNormalStage;
	public string normStageIcon;
	public bool isSpinStage;
	public string spinStageIcon;
	public bool isEXP;
	public string expTag;
	public string expbannerIcon;
	public string expDesc;
}

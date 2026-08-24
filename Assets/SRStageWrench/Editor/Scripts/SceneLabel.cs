using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SceneViewLabel : MonoBehaviour
{
    static GUIStyle labelStyle;

    public static string sceneLabel = "SR Toolkit";
    public static string SceneLabelText { get => sceneLabel; set => sceneLabel = value; }
    static SceneViewLabel()
    {
        labelStyle = new GUIStyle();
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.fontSize = 20;
        labelStyle.normal.textColor = Color.white;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();
        GUI.Label(new Rect(50, 10, 150, 20), sceneLabel, labelStyle);
        Handles.EndGUI();
    }
}
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine;

public class CustomStageBundle : EditorWindow
{
    private const string AssetsPath = "Assets/_CustomStageElements";
    private string ExportPath = "Build";
    private static string BundleName { get; set; }

    [MenuItem("SynthRiders/Export Stage Bundle")]
    static void ExportCustomStageBundle()
    {
        // AssetImporter.GetAtPath(AssetsPath).;
        CustomStageBundle window = ScriptableObject.CreateInstance<CustomStageBundle>();
        window.titleContent = new GUIContent("Export Settings");
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 200);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Space(20);
        ExportPath = EditorGUILayout.TextField("Export Path: ", ExportPath);
        GUILayout.Space(20);
        BundleName = EditorGUILayout.TextField("Custom Stage Name: ", BundleName);
        
        GUILayout.Space(70);
        if (GUILayout.Button("Export PC")) {
            if(BundleName == null || BundleName.Equals(string.Empty)) {
                Debug.LogError("Please set the bundle name");
            } else {
                Debug.Log($"Bundle Name set to {BundleName}");

                AssetImporter.GetAtPath(AssetsPath).SetAssetBundleNameAndVariant($"{BundleName}.stage", "");
                string bundleExportPath = $"{Application.dataPath}/../{ExportPath}/{BundleName}/";

                if(!Directory.Exists(bundleExportPath))
                {
                    Directory.CreateDirectory(bundleExportPath);
                } 

                BuildPipeline.BuildAssetBundles(bundleExportPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
                EditorUtility.RevealInFinder($"{bundleExportPath}");
                Debug.Log($"Export complete, to directory {bundleExportPath}");
                this.Close();
            }             
        }

        if (GUILayout.Button("Export Quest")) {
            if(BundleName == null || BundleName.Equals(string.Empty)) {
                Debug.LogError("Please set the bundle name");
            } else {
                Debug.Log($"Bundle Name set to {BundleName}");

                AssetImporter.GetAtPath(AssetsPath).SetAssetBundleNameAndVariant($"{BundleName}.stagequest", "");
                string bundleExportPath = $"{Application.dataPath}/../{ExportPath}/{BundleName}/";

                if(!Directory.Exists(bundleExportPath))
                {
                    Directory.CreateDirectory(bundleExportPath);
                } 

                BuildPipeline.BuildAssetBundles(bundleExportPath, BuildAssetBundleOptions.None, BuildTarget.Android);
                EditorUtility.RevealInFinder($"{bundleExportPath}");
                Debug.Log($"Export complete, to directory {bundleExportPath}");
                this.Close();
            }             
        }

        if (GUILayout.Button("Cancel")) {
            this.Close();           
        }
        this.Repaint();
    }
}

using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.XR;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CustomStageBundle : EditorWindow
{
    private string AssetsPath = "Assets/_CustomStageElements";
    private string ExportPath = "Build";
    private static string BundleName { get; set; }
    private bool IsForSpinMode { get; set; }

    [MenuItem("SynthRiders/Legacy/Export Stage Bundle")]
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
        GUILayout.Space(10);
        BundleName = EditorGUILayout.TextField("Custom Stage Name: ", BundleName);
        GUILayout.Space(10);
        IsForSpinMode = EditorGUILayout.Toggle("For spin mode?", IsForSpinMode);
        
        GUILayout.Space(70);
        if (GUILayout.Button("Export PC")) {
            if(BundleName == null || BundleName.Equals(string.Empty)) {
                Debug.LogError("Please set the bundle name");
                EditorUtility.DisplayDialog("Custom Stage Name Empty", "The Custom Stage name must not be empty", "Ok");

            } else {
                Debug.Log($"Bundle Name set to {BundleName}");

                string extention = "stage";
				/*if(IsForSpinMode) {
                    extention = "spinstage";
                } */
				//AssetsPath = SRWrench.WrenchSceneSession.sessionCSElements;
                string sanitizeBundleName = SanitizeString(BundleName);

                //move everything from working directory over to root
                FileUtil.MoveFileOrDirectory(SRToolkit.WorkspaceManager.activeWorkspace.workspacePath, "Assets/_CustomStageElements");
                AssetDatabase.Refresh();

                var dependencies = AssetDatabase.GetDependencies("Assets/_CustomStageElements/Prefabs/Platform.prefab");

                StringBuilder dependenciesString = new StringBuilder();
                dependenciesString.AppendLine();

                GameObject CustomStageInfoPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_CustomStageElements/Prefabs/CustomStageInfo.prefab");
                var InfoScript = CustomStageInfoPrefab.GetComponent<Synth.Utils.CustomStageInfo>();

                bool[] stageMode = { InfoScript.isNormStage, InfoScript.isSpinStage, InfoScript.isExperience };
                string[] normprefabs = { AssetDatabase.GetAssetPath(InfoScript.normTile01), AssetDatabase.GetAssetPath(InfoScript.normTile02), AssetDatabase.GetAssetPath(InfoScript.normTile03), AssetDatabase.GetAssetPath(InfoScript.normplatform)};
                string[] spinprefabs = { AssetDatabase.GetAssetPath(InfoScript.spinEmitter), AssetDatabase.GetAssetPath(InfoScript.spinPlatform) };
                string[] expprefabs = { AssetDatabase.GetAssetPath(InfoScript.expPrefab)};

                List<string[]> mergedPrefabs = new List<string[]>();

                List<string> ogDependencyPaths = new List<string>();
                List<string> movedDependPaths = new List<string>();

                if (stageMode[0]) { mergedPrefabs.Add(normprefabs); Debug.Log("added Normal Prefabs"); }
                if (stageMode[1]) { mergedPrefabs.Add(spinprefabs); Debug.Log("added Spin Prefabs"); }
                if (stageMode[2]) { mergedPrefabs.Add(expprefabs); Debug.Log("added Experience Prefabs"); }

                StringBuilder listedOriginalDepends = new StringBuilder();

                //Disable AutoRefreshing Scripts to Prevent Code Editors from popping
                //up due to scripts being moved
                EditorPrefs.SetInt("kAutoRefresh", 0);

                foreach (var masterList in mergedPrefabs) { Debug.Log(masterList.ToString()); }
                //for each string in merged prefabs string
                foreach (var listMerg in mergedPrefabs)
				{
                    Debug.Log(listMerg);
                    //for each item within current string
                    foreach(var combinedList in listMerg)
					{
                        Debug.Log("Working on: " + combinedList);
                        var prefabDepends = AssetDatabase.GetDependencies(combinedList);
                        //for each prefab
                        foreach (var dependeds in prefabDepends)
                        {
                            //Debug.Log(dependeds);
                            if (dependeds.Contains("Assets/_CustomStageElements") || dependeds.Contains("Packages/") || ogDependencyPaths.Contains(dependeds) ) { }
                            else
                            {
                                //Debug.Log(dependeds);
                                ogDependencyPaths.Add(dependeds);
                                listedOriginalDepends.Append(dependeds);
                                listedOriginalDepends.AppendLine();
                            }
                        }
                    }
				}

                Debug.Log("Paths ready to migration!");
                
                //foreach (var depends in ogDependencyPaths) { Debug.Log(depends.ToString()); }

                //Check if Depends folder exists if not, Create it
                if (!AssetDatabase.IsValidFolder("Assets/_CustomStageElements/Depends")) { AssetDatabase.CreateFolder("Assets/_CustomStageElements", "Depends"); }

                //Move all dependensies to the Depends folder
                StringBuilder newDependsLocations = new StringBuilder();

                foreach (var depends in ogDependencyPaths)
                {
                    Debug.Log("FileName: " + Path.GetFileName(depends));
                    string dataPath = Application.dataPath;
                    dataPath = dataPath.Replace("Assets", "");
                    string moveTo = "Assets/_CustomStageElements/Depends/" + Path.GetFileName(depends);
                    //Debug.Log(moveTo);
                    movedDependPaths.Add(moveTo);

                    newDependsLocations.Append(moveTo);
                    newDependsLocations.AppendLine();

                    AssetDatabase.MoveAsset(depends, moveTo);
                }

                Debug.Log(listedOriginalDepends);
                Debug.Log(newDependsLocations);

                //Refresh the Databasses so Unity can detect that the assets moved
                //and change the path definitions accordingly
                Debug.Log("Refreshing Database!");
                AssetDatabase.Refresh();


                //configure menuitem thumbnail import settings to work in game
                TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(AssetsPath + "/MenuItems/Thumb.png");
                if (importer.isReadable != true) { importer.isReadable = true; }
                if (importer.textureCompression != TextureImporterCompression.Uncompressed) { importer.textureCompression = TextureImporterCompression.Uncompressed; }
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();

                AssetImporter.GetAtPath(AssetsPath).SetAssetBundleNameAndVariant($"{sanitizeBundleName}.{extention}", "");
                string bundleExportPath = $"{Application.dataPath}/../{ExportPath}/{sanitizeBundleName}/";

				
                if(!Directory.Exists(bundleExportPath))
                {
                    Directory.CreateDirectory(bundleExportPath);
                } 

                BuildPipeline.BuildAssetBundles(bundleExportPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows); 
                //move all dependencies back to their original folders
                for (var i = 0; i < movedDependPaths.Count; i++)
                {
                    Debug.Log("Current i value: " + i + " Current Pointer: " + movedDependPaths[i]);

                    string dataPath = Application.dataPath;
                    dataPath = dataPath.Replace("Assets", "");
                    string moveTo = "Assets/_CustomStageElements/Depends/" + Path.GetFileName(movedDependPaths[i]);
                    Debug.Log("Moved Path: " + movedDependPaths[i] + "\nAssets Original Path: " + ogDependencyPaths[i]);
                    AssetDatabase.MoveAsset(moveTo, ogDependencyPaths[i]);
                }

                //Re-Enable AutoRefreshing Scripts
                EditorPrefs.SetInt("kAutoRefresh", 1);

                //move everything back
                FileUtil.ReplaceDirectory("Assets/_CustomStageElements", SRToolkit.WorkspaceManager.activeWorkspace.workspacePath);
                //delete the leftover _CustomStageElements folder
                //so that we can do this again without having to manually delete it again
                FileUtil.DeleteFileOrDirectory("Assets/_CustomStageElements");
                File.Delete(Application.dataPath + "/" + "_CustomStageElements");
                File.Delete(Application.dataPath + "/" + "_CustomStageElements.meta");
                AssetDatabase.Refresh();
                EditorUtility.RevealInFinder($"{bundleExportPath}");
                Debug.Log($"Export complete, to directory {bundleExportPath}");
                this.Close();
            }             
        }

        if (GUILayout.Button("Export Quest")) {
            if(BundleName == null || BundleName.Equals(string.Empty)) {
                Debug.LogError("Please set the bundle name");
                EditorUtility.DisplayDialog("Custom Stage Name Empty", "The Custom Stage name must not be empty", "Ok");
            } else {
                Debug.Log($"Bundle Name set to {BundleName}");

                string extention = "stagequest";
                if(IsForSpinMode) {
                    extention = "spinstagequest";
                }
                string sanitizeBundleName = SanitizeString(BundleName);

                AssetImporter.GetAtPath(AssetsPath).SetAssetBundleNameAndVariant($"{sanitizeBundleName}.{extention}", "");
                string bundleExportPath = $"{Application.dataPath}/../{ExportPath}/{sanitizeBundleName}/";

                //Check if Android Build support is install before exporting so that people
                //do not get confused when it doesn't export when Android SDK isn't installed
                //Debug.Log(BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android));
                if (BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android) == true)
                {
                    Debug.Log("Andriod is Supported meaning Android SDK is INSTALLED!");
                    if (!Directory.Exists(bundleExportPath))
                    {
                        Directory.CreateDirectory(bundleExportPath);
                    }
                    try
                    {
                        BuildPipeline.BuildAssetBundles(bundleExportPath, BuildAssetBundleOptions.None, BuildTarget.Android);
                        EditorUtility.RevealInFinder($"{bundleExportPath}");
                        Debug.Log($"Export complete, to directory {bundleExportPath}");
                    }
                    catch (Exception E)
                    {
                        Debug.LogError("FAILED TO EXPORT! Caused by: " + E);
                    }
                }
                else
                {
                    Debug.LogError("ANDROID SDK NOT INSTALLED!");
                    EditorUtility.DisplayDialog("EXPORT FAILED!", "Exporting to Quest failed because the Android SDK is not installed! Install it from Unity Hub and restart the editor to continue", "Ok");
                }
                
                
                this.Close();
            }             
        }

        if (GUILayout.Button("Cancel")) {
            this.Close();           
        }
        this.Repaint();
    }

    private static string SanitizeString(string dirtyString)
	{
		return new String(dirtyString.Where(Char.IsLetterOrDigit).ToArray());
	}
}

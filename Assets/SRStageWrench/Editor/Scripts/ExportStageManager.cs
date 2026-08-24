using Synth.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SRToolkit
{
	public class ExportStageManager
	{
		//Gather all dependencies from all the prefabs assigned to the current Workspace,
		//change any image cover's import preferences so that i can be loaded in game (Must have No compression, read write enabled)
		//gather all the stage's dependencies and create assetbundles for them without moving them around in the project (so that we don't break anything)

		//packaging for upload
		//take the freshly created stage files for every platform
		//create and fill out a json containing all the info about the stage
		//create a zip
		//package json, both stage files, and any images used for the stage
		private static string pcFileExtension = ".stage";
		private static string androidFileExtension = ".stagedroid";
		public static void ExportStage(WSPSessionInfo workspaceToExport, bool isPCVR, bool isAndroid, bool exportBundled)
		{

			//string bundleExportPath = $"{Application.dataPath}/../Build/{workspaceToExport.name}/{workspaceToExport.name}{timeNow().Replace(":", "-")}/";
			string bundleExportPath = Path.Combine(Application.dataPath, "..", "Build", workspaceToExport.name, $"{workspaceToExport.name}{timeNow().Replace(":", "-")}{Path.DirectorySeparatorChar}");
			List<string> workingPrefabs = new List<string>();


			//Check if Android Build Tools are installed
			if (isAndroid && BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android) == true) { Debug.Log("Android Build Tools are Installed!"); }
			else { Debug.LogError("Android Build Tools are MISSING! You cannot export for Android platforms unless this is installed!"); isAndroid = false; }

			CustomStageInfo InfoScript = workspaceToExport.workspaceInfoScript;

			if (InfoScript.isNormStage)
			{
				GameObject[] prefabs = { InfoScript.normTile01, InfoScript.normTile02, InfoScript.normTile03, InfoScript.normplatform, InfoScript.normSpecialFX };
				foreach (GameObject stageObject in prefabs) { if (stageObject != null) { workingPrefabs.Add(AssetDatabase.GetAssetPath(stageObject)); } }
				Debug.Log("added Normal Prefabs");
			}
			if (InfoScript.isSpinStage)
			{
				GameObject[] prefabs = { InfoScript.spinEmitter, InfoScript.spinPlatform, InfoScript.spinSpecialFX };
				foreach (GameObject stageObject in prefabs) { if (stageObject != null) { workingPrefabs.Add(AssetDatabase.GetAssetPath(stageObject)); } }
				Debug.Log("added Spin Prefabs");
			}
			if (InfoScript.isExperience)
			{
				GameObject[] prefabs = { InfoScript.expPrefab, InfoScript.expSpecialsFX };
				foreach (GameObject stageObject in prefabs) { if (stageObject != null) { workingPrefabs.Add(AssetDatabase.GetAssetPath(stageObject)); } }
				Debug.Log("added Experience Prefabs");
			}

			workingPrefabs.Add(AssetDatabase.GetAssetPath(workspaceToExport.workspaceStageInfo));

			//Handle Texture import settings so it appears correctly in game
			Texture2D[] textures = { InfoScript.normStageIcon, InfoScript.spinStageIcon, InfoScript.expPromptBanner };
			foreach (Texture2D icon in textures) { if(icon) CorrectTextureImportSettings(icon); }

			// add "EXP-" to the front of the tag
			if (workspaceToExport.workspaceInfoScript.isExperience && workspaceToExport.workspaceInfoScript.expTag != null && !workspaceToExport.workspaceInfoScript.expTag.Contains("EXP-"))
			{
				workspaceToExport.workspaceInfoScript.expTag = workspaceToExport.workspaceInfoScript.expTag.Insert(0, "EXP-");
			}

			//new way of grabbing files and sending them to the assetbundle. 
			//this way we don't need to move things around and possibly cause problems
			if (isPCVR)
			{
				BuildAssetBundle($"{workspaceToExport.name}{pcFileExtension}", workingPrefabs.Concat(GatherDependencies(workingPrefabs.ToArray())).Distinct().ToArray(), BuildTarget.StandaloneWindows64, bundleExportPath);
			}
			if (isAndroid)
			{
				BuildAssetBundle($"{workspaceToExport.name}{androidFileExtension}", workingPrefabs.Concat(GatherDependencies(workingPrefabs.ToArray())).Distinct().ToArray(), BuildTarget.Android, bundleExportPath);
			}

			Debug.Log("Cleaning up excess manifets files");
			DirectoryInfo dirInfo = new DirectoryInfo(bundleExportPath);
			var folderInfo = dirInfo.GetFiles().Where(x => x.Extension != pcFileExtension && x.Extension != androidFileExtension);
			foreach (var file in folderInfo) { file.Delete(); }

			if (exportBundled) { CreateStageBundle(workspaceToExport, bundleExportPath, isPCVR, isAndroid); }

			//Restore the EXP tag to remove the "EXP-" in front so that it doesn't cause issues with the UI
			string expTag = workspaceToExport.workspaceInfoScript.expTag;
			if (expTag.Contains("EXP-")) { workspaceToExport.workspaceInfoScript.expTag = expTag.Replace("EXP-", ""); }

			EditorUtility.RevealInFinder(bundleExportPath);
			Debug.Log($"Export complete! Directory: {bundleExportPath}");
		}

		//Texture settings must be exposed a certain way so that code can hot-load them.
		private static void CorrectTextureImportSettings(Texture2D texture)
		{
			if(texture == null) { Debug.LogError("Cannot correct import settings for a null texture!"); return; }

			TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));
			if (importer.isReadable != true) { importer.isReadable = true; }
			if (importer.textureCompression != TextureImporterCompression.Uncompressed) { importer.textureCompression = TextureImporterCompression.Uncompressed; }
			EditorUtility.SetDirty(importer);
			importer.SaveAndReimport();
		}

		private static void BuildAssetBundle(string name, string[] assetList, BuildTarget platform, string buildPath)
		{
			if (assetList == null) { Debug.LogError("Cannot create Assetbundle with an invalid assetlist!"); return; }
			AssetBundleBuild build = new AssetBundleBuild
			{
				assetBundleName = name,
				assetNames = assetList,
			};
			if (!Directory.Exists(buildPath)) { Directory.CreateDirectory(buildPath); }
			BuildPipeline.BuildAssetBundles(buildPath, new[] { build }, BuildAssetBundleOptions.None, platform);
		}

		private static void CreateStageBundle(WSPSessionInfo workingWorkspace, string pathforFiles, bool PCVR, bool Android)
		{
			Debug.Log("Assembling .stagepkg");
			//Create tempZipFolder so we can turn it into a zip later
			string dir = $"{pathforFiles}/{workingWorkspace.workspaceName}";
			string tempZipFolder = $"{dir}-tempZIP";

			if (!Directory.Exists(tempZipFolder)) { Directory.CreateDirectory(tempZipFolder); }

			jsonStageData stageMETA = new jsonStageData();
			CustomStageInfo csInfo = workingWorkspace.workspaceInfoScript;

			stageMETA.creatorName = workingWorkspace.workspaceCreator;
			stageMETA.exportedOn = timeNow();

			//Copy finished StageFiles to temp ZIP folder
			//var dirInfo = new DirectoryInfo(bundleExportPath);
			DirectoryInfo info = new DirectoryInfo(pathforFiles);
			var stageFiles = info.GetFiles().Where(x => x.Extension == pcFileExtension || x.Extension == androidFileExtension);
			foreach (FileInfo file in stageFiles)
			{
				Debug.Log($"Copying File: {file.Name}");
				FileUtil.CopyFileOrDirectory(file.ToString(), $"{tempZipFolder}/{file.Name}");
			}

			//Copy Icons/Banner to folder
			if (csInfo.normStageIcon != null) { FileInfo iconMeta = new FileInfo(AssetDatabase.GetAssetPath(csInfo.normStageIcon)); FileUtil.CopyFileOrDirectory(iconMeta.ToString(), tempZipFolder + "/" + iconMeta.Name); }
			if (csInfo.spinStageIcon != null) { FileInfo iconMeta = new FileInfo(AssetDatabase.GetAssetPath(csInfo.spinStageIcon)); FileUtil.CopyFileOrDirectory(iconMeta.ToString(), $"{tempZipFolder}/{csInfo.spinStageName}.SPN{iconMeta.Extension}"); }
			if (csInfo.expPromptBanner != null) { FileInfo iconMeta = new FileInfo(AssetDatabase.GetAssetPath(csInfo.expPromptBanner)); FileUtil.CopyFileOrDirectory(iconMeta.ToString(), $"{tempZipFolder}/{csInfo.expName}.EXP{iconMeta.Extension}"); }

			if (PCVR)
			{
				stageMETA.pc = GatherInfoOnStage(workingWorkspace);
				stageMETA.pc.available = true;
				stageMETA.pc.fileName = $"{workingWorkspace.name}{pcFileExtension}";
			}
			if (Android)
			{
				stageMETA.android = GatherInfoOnStage(workingWorkspace);
				stageMETA.android.available = true;
				stageMETA.android.fileName = $"{workingWorkspace.name}{androidFileExtension}";
			}

			string sanitizedJSON = JsonUtility.ToJson(stageMETA, true);
			File.WriteAllText($"{tempZipFolder}/stagepkg.json", sanitizedJSON);
			ZipFile.CreateFromDirectory(tempZipFolder, $"{dir}.stagepkg");
			FileUtil.DeleteFileOrDirectory(tempZipFolder);
		}
		//Gather a list of dependencies (scripts, shaders, objects, etc) and return a list of them
		private static List<string> GatherDependencies(string[] prefabPaths)
		{
			List<string> gatheredDependencies = new List<string>();
			foreach (string dependencies in SpaceFileManager.GatherPrefabDependencies(prefabPaths.ToArray()))
			{
				//Debug.Log("Current: " + dependencies);
				if (dependencies.Contains("Assets/_CustomStageElements") || dependencies.Contains("Packages/") || gatheredDependencies.Contains(dependencies) || dependencies.Contains(".cs")) { /*Debug.LogError("Skipping: " + dependencies); */}
				else
				{
					gatheredDependencies.Add(dependencies);
				}
			}
			Debug.Log("Gathered all Dependencies! Dependencies: \n" + gatheredDependencies);
			return gatheredDependencies;
		}

		//obtain the more common values as a new instance so we don't
		//accidentally change other platform's values
		private static platformValues GatherInfoOnStage(WSPSessionInfo workspace)
		{
			CustomStageInfo csInfo = workspace.workspaceInfoScript;
			platformValues commonValues = new platformValues();
			commonValues.isNormalStage = csInfo.isNormStage;
			commonValues.isSpinStage = csInfo.isSpinStage;
			commonValues.isEXP = csInfo.isExperience;
			commonValues.expTag = csInfo.expTag;
			commonValues.expDesc = csInfo.expPromptText;
			if (csInfo.normStageIcon != null) { FileInfo iconMeta = new FileInfo(AssetDatabase.GetAssetPath(csInfo.normStageIcon)); commonValues.normStageIcon = iconMeta.Name; }
			if (csInfo.spinStageIcon != null) { FileInfo iconMeta = new FileInfo(AssetDatabase.GetAssetPath(csInfo.spinStageIcon)); commonValues.spinStageIcon = $"{csInfo.spinStageName}.SPN{iconMeta.Extension}"; }
			if (csInfo.expPromptBanner != null) { FileInfo iconMeta = new FileInfo(AssetDatabase.GetAssetPath(csInfo.expPromptBanner)); commonValues.expbannerIcon = $"{csInfo.expName}.EXP{iconMeta.Extension}"; ; }

			return commonValues;
		}

		private static string timeNow() => DateTime.Now.ToString("yyyyMMd-hmmss");
	}
}

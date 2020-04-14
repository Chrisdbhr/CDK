using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CDK {
	public static class BuildProcessMethods {

		[PostProcessBuild(1)]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltExe) {

			switch (target) {
				default:
					// get root build path
					var rootProjectBuildPath = pathToBuiltExe.Replace($"{Application.productName}.exe", string.Empty);

					// delete unity crash handler
					var unityCrashHandlerPath = rootProjectBuildPath + "/UnityCrashHandler32.exe";
					Debug.Log($"Deleting 'UnityCrashHandler32' {unityCrashHandlerPath}");
					File.Delete(unityCrashHandlerPath);

					// publish build
					PublishWin32();
					break;
			}

			// raise version number
			var splitVersion = PlayerSettings.bundleVersion.Split('.');
			var lastIndex = splitVersion.Length - 1;
			var intVersion = int.Parse(splitVersion[lastIndex]);
			intVersion += 1;
			splitVersion[lastIndex] = intVersion.ToString();
			PlayerSettings.bundleVersion = string.Join(".", splitVersion);

			AssetDatabase.Refresh();
		}

		[MenuItem("Build/DevelopmentBuild/" + "Publish: " + NAME_WIN32)]
		public static void PublishWin32() {
			ExecuteCommand($"{Application.dataPath}/../../BuildTools/publish-{NAME_WIN32}.bat {Application.productName}-{NAME_WIN32} {Application.version}");
		}
		

		public const string NAME_WIN32 = "win32";
		[MenuItem("Build/DevelopmentBuild/" + NAME_WIN32)]
		public static void BuildWindows32DevBuild() {

			Debug.Log($"Starting build '{NAME_WIN32}' version: {Application.version}");

			// get scenes to build
			var scenes = EditorBuildSettings.scenes;

			// get target directory
			var folderName = $"{Application.productName}-{NAME_WIN32}";
			var path = $"T:/Builds/{Application.productName}-{NAME_WIN32}/{folderName}";
			Directory.CreateDirectory(path);
			if (!Directory.Exists(path)) {
				path = EditorUtility.SaveFolderPanel("Choose location to built", path, folderName);
			}
			
			// delete already build files
			string[] files = { $"{Application.productName}.exe", "UnityPlayer.dll" };
			string[] folders = { $"{Application.productName}_Data", "MonoBleedingEdge" };
			foreach (var file in files) {
				File.Delete(file);
			}
			foreach (var folder in folders) {
				if(Directory.Exists(folder)) Directory.Delete(folder, true);
			}
			
			// build player
			BuildPipeline.BuildPlayer(scenes, $"{path}/{Application.productName}.exe", BuildTarget.StandaloneWindows, BuildOptions.Development | BuildOptions.ShowBuiltPlayer);
		}
		
		
		static void ExecuteCommand(string command)
		{
			var processInfo = new ProcessStartInfo("cmd.exe", "/C " + command);
			processInfo.CreateNoWindow = false;
			processInfo.UseShellExecute = true;

			var process = Process.Start(processInfo);
			if (process == null) {
				Debug.LogError($"Could not initialize process with command: {command}");
				return;
			}
			process.WaitForExit();

			Debug.Log($"ExitCode: {process.ExitCode}");
			process.Close();
		}
	}
}
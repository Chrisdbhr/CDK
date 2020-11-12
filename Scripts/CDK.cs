// CDK (Chris Development Kit) is developed and frequently updated by @Chrisdbhr
// Those are part of Source Code of all my games developed with Unity
// Don't forget to Star and check https://github.com/Chrisdbhr/CDK for updates. 
#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

namespace CDK {
	[InitializeOnLoad]
	static class CDK {

		static CDK() {
			CheckForUpdatedVersion();
			CGameSettings.EditorCreateGameSettingsResourceIfNeeded();
		} 
		
		
		
		private const string VERSION_FILE_PATH = "CDK/Resources/CDKVersion.txt";
		private const string VERSION_UPDATE_URL = "https://raw.githubusercontent.com/Chrisdbhr/CDK/master/Resources/CDKVersion.txt";


		
		
		private static string GetCurrentVersion() {
			string fileContent = null;
			try {
				fileContent = File.ReadAllText($"{Application.dataPath}/{VERSION_FILE_PATH}");
			} catch (Exception e) {
				Debug.LogError($"Could not read version file at {VERSION_FILE_PATH}.\nAre you sure that CDK is inside Assets/CDK folder?{e.Message}");
			}
			return fileContent;
		}

		/// <summary>
		/// Notify if a update is available for download.
		/// </summary>
		[MenuItem("CDK/Check for update")]
		private static void CheckForUpdatedVersion() {
			var request = UnityWebRequest.Get(VERSION_UPDATE_URL);
			request.SendWebRequest().completed += asyncOp => {
				if (request.isHttpError || request.isNetworkError) {
					Debug.LogWarning($"CDK was unable to check for updates: {request.error}");
					return;
				}
				try {
					var currentVersion = new Version(GetCurrentVersion());
					var releasedVersion = new Version(request.downloadHandler.text);
					if (currentVersion < releasedVersion) {
						Debug.Log($"There is a newer version of CDK available to updated (you have version {currentVersion} and latest version is {releasedVersion}. Please visit https://github.com/Chrisdbhr/CDK for more information.");
					}
				} catch (Exception e) {
					Debug.LogWarning($"CDK checked for update but it was unable to validate version because: {e.Message}");
				}
			};
		}

	}
}
#endif

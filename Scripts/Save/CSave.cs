using System;
using System.IO;
using System.Threading.Tasks;
using CDK.Json;
using UniRx;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;

namespace CDK {
	public partial class CSave {

		#region <<---------- Singleton ---------->>

		public static CSave get => getRx.Value;
		public static ReadOnlyReactiveProperty<CSave> getRx {
			get {
				if (_rx == null) {
					// load game
					_rx = new ReactiveProperty<CSave>();
					_rx.Value = new CSave();
				}
				return _rx.ToReadOnlyReactiveProperty();
			}
		}
		public static bool IsGameLoaded => getRx.Value != null;
		private static ReactiveProperty<CSave> _rx;

		#endregion <<---------- Singleton ---------->>

		
		
		
		#region <<---------- Properties ---------->>
		
		[JsonProperty("cameraSensitivity")]
		public Vector2 CameraSensitivity = new Vector2(7.5f, 0.15f);

		[JsonProperty("language")]
		public string Language;

		#endregion <<---------- Properties ---------->>


		

		#region <<---------- Saving ---------->>

		public static async Task SaveGame() {
			try {
				Debug.Log($"Starting SaveGame process.");
				
				var json = JsonConvert.SerializeObject(getRx.Value, CJsonExtensions.DefaultSettings);
			
				var folderPath = GetSaveFileDirectory();
				if (!Directory.Exists(folderPath)) {
					Directory.CreateDirectory(folderPath);
				}
				var filePath = GetSaveFilePath();
			
				Debug.Log($"Trying to SaveGame in file '{filePath}' with content: '{json}'");

				using var streamWriter = File.CreateText(filePath);
				await streamWriter.WriteAsync(json);
			
				Debug.Log($"Game saved.");
			}
			catch (Exception e) {
				Debug.LogError(e);
			}
		}
		
		#endregion <<---------- Saving ---------->>



		
		#region <<---------- Loading ---------->>

		public static bool LoadGame() {
			try {
				var filePath = GetSaveFilePath();
				
				Debug.Log($"Trying to LoadGame with file '{filePath}'");

				if (!File.Exists(filePath)) {
					Debug.LogWarning($"Save file at path '{filePath}' doesnt exist!");
					return false;
				}

				var fileContent = File.ReadAllText(filePath);
				
				Debug.Log($"Save file content: {fileContent}");

				var save = JsonConvert.DeserializeObject<CSave>(fileContent, CJsonExtensions.DefaultSettings);

				if (save == null) {
					Debug.LogError($"Could not deserialize Save at path '{filePath}'!");
					return false;
				}
				
				Debug.Log($"Game Loaded.");
				(_rx ??= new ReactiveProperty<CSave>()).Value = save;
				return true;
			}
			catch (Exception e) {
				Debug.LogError(e);
			}
			return false;
		}

		#endregion <<---------- Loading ---------->>
		
		
		
		
		#region <<---------- Path ---------->>

		private static string GetSaveFileDirectory() {
			return Path.Combine(Application.persistentDataPath, "SaveGame").Replace('\\','/');
		}
		private static string GetSaveFilePath() {
			return GetSaveFileDirectory() + "/gamesave.json";
		}

		#endregion <<---------- Path ---------->>


		#region <<---------- Editor ---------->>
	
		#if UNITY_EDITOR

		[MenuItem("Game/Open save folder")]
		public static void OpenSaveFolder() {
			EditorUtility.RevealInFinder(GetSaveFileDirectory());
		}
		
		#endif
		
		#endregion <<---------- Editor ---------->>
		
	}
}

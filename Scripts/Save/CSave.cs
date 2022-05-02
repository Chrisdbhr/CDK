using System;
using System.IO;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if Newtonsoft_Json_for_Unity
using Newtonsoft.Json;
#endif


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
		
		#if Newtonsoft_Json_for_Unity
		[JsonProperty("cameraSensitivity")]
		#endif
		public Vector2 CameraSensitivity = DefaultMaxCameraSensitivity * 0.5f;
        
		#if Newtonsoft_Json_for_Unity
        [JsonIgnore]
		#endif
        public static readonly Vector2 DefaultMaxCameraSensitivity = new Vector2(5f * 2f, 0.04f * 2f);

		#if Newtonsoft_Json_for_Unity
		[JsonProperty("language")]
		#endif
		public string Language;

		#endregion <<---------- Properties ---------->>


		

		#region <<---------- Saving ---------->>

		public static async Task SaveGame() {
			try {
				#if Newtonsoft_Json_for_Unity
				Debug.Log($"Starting SaveGame process.");
				
				var json = JsonConvert.SerializeObject(getRx.Value, CJsonExtensions.DefaultSettings);
			
				var folderPath = GetSaveFileDirectory();
				if (!Directory.Exists(folderPath)) {
					Directory.CreateDirectory(folderPath);
				}
				var filePath = GetSaveFilePath();
			
				Debug.Log($"Trying to SaveGame in file '{filePath}' with content: '{json}'");

				using (var streamWriter = File.CreateText(filePath)) {
					await streamWriter.WriteAsync(json);
				}
			
				Debug.Log($"Game saved.");

				#else
				throw new NotImplementedException();
				#endif
			}
			catch (Exception e) {
				Debug.LogError(e);
			}
		}
		
		#endregion <<---------- Saving ---------->>



		
		#region <<---------- Loading ---------->>

		public static bool LoadGame() {
			try {
				#if Newtonsoft_Json_for_Unity
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
				(_rx = _rx ?? new ReactiveProperty<CSave>()).Value = save;
				return true;
				
				#else
				throw new NotImplementedException();
				#endif
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

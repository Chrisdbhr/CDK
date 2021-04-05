using System;
using System.IO;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityJSON;

namespace CDK {
	[JSONObject(ObjectOptions.IgnoreUnknownKey | ObjectOptions.IgnoreProperties)]
	public partial class CSave {

		#region <<---------- Singleton ---------->>

		public static CSave get {
			get {
				if (_instance != null) return _instance;
				Debug.Log($"Creating new instance of {nameof(CSave)}");
				CApplication.IsQuitting += () => {
					_instance = null;
				};
				return _instance = new CSave();
			}
		}
		private static CSave _instance;
		
		#endregion <<---------- Singleton ---------->>


		#region <<---------- Properties ---------->>
		
		[JSONNode(NodeOptions.ReplaceDeserialized, key = KEY_CameraSensitivityHorizontal)]
		public readonly ReactiveProperty<float> CameraSensitivityHorizontal = new ReactiveProperty<float>(5f);
		public const string KEY_CameraSensitivityHorizontal = "cameraSensitivityHorizontal";
		
		[JSONNode(NodeOptions.ReplaceDeserialized, key = KEY_CameraSensitivityVertical)]
		public readonly ReactiveProperty<float> CameraSensitivityVertical = new ReactiveProperty<float>(0.1f);
		public const string KEY_CameraSensitivityVertical = "cameraSensitivityVertical";

		#endregion <<---------- Properties ---------->>


		#region <<---------- Saving ---------->>

		public static async Task SaveGame() {
			try {
				Debug.Log($"Starting SaveGame process.");

				var json = CJsonUtils.DefaultSerializer.Serialize(get);
			
				var folderPath = GetSaveFileDirectory();
				if (!Directory.Exists(folderPath)) {
					Directory.CreateDirectory(folderPath);
				}
				var filePath = GetSaveFilePath();
			
				Debug.Log($"Trying to SaveGame in file '{filePath}'");

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

		public static async Task LoadGame() {
			try {
				var filePath = GetSaveFilePath();
				
				Debug.Log($"Trying to LoadGame with file '{filePath}'");

				if (!File.Exists(filePath)) {
					Debug.LogWarning($"Save file at path '{filePath}' doesnt exist!");
					return;
				}

				var fileContent = File.ReadAllText(filePath);
				
				Debug.Log($"Save file content: {fileContent}");

				var save = CJsonUtils.DefaultDeserializer.Deserialize<CSave>(fileContent);

				if (save == null) {
					Debug.LogError($"Could not deserialize Save at path '{filePath}'!");
					return;
				}

				CSave._instance = save;
				
				Debug.Log($"Game Loaded.");
			}
			catch (Exception e) {
				Debug.LogError(e);
			}
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
		
	}
}

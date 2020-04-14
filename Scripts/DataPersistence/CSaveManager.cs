using System;
using System.IO;
using System.Threading.Tasks;
using SimpleJSON;
using UnityEngine;

namespace CDK.DataPersistence {
	public class CSaveManager {


		#region <<---------- Singleton ---------->>

		public static CSaveManager get {
			get {
				if (_instance == null) {
					_instance = new CSaveManager();
				}
				return _instance;
			}
		}
		private static CSaveManager _instance;

		#endregion <<---------- Singleton ---------->>
		
		
		
		
		#region <<---------- Properties and Fields ---------->>
		
		private SimpleJSON.JSONNode _saveDataJsonNode;
		private string _currentProfile = "Default";
		
		private const string SAVE_DIR_SUFIX = "/save";
		private const string SAVE_FILE_SUFIX = ".savedata.json";

		private readonly string _saveDirectory;

		private bool _loadedSave;

		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- Initializers ---------->>

		public CSaveManager() {
			_instance = this;
			Debug.Log($"[SaveManager] Initializing.");
			this._saveDirectory = Application.persistentDataPath + SAVE_DIR_SUFIX;
			this._saveDataJsonNode = new JSONObject();
			this.LoadFromDisk().Wait();
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		
		
		#region <<---------- Data Verification ---------->>
		
		public bool IsSaveLoaded() {
			return this._loadedSave;
		}
		
		#endregion <<---------- Data Verification ---------->>


		

		#region <<---------- Save and Load Data ---------->>

		private JSONNode LoadDataFromKey(string key) {
			JSONNode json = this._saveDataJsonNode[this._currentProfile];
			return json[key];
		}

		#endregion <<---------- Save and Load Data ---------->>
		
		
		

		#region <<---------- Saving and Loading on Disk ---------->>

		public async Task SaveToDisk() {
			// validade directory
			if (!this.IsSaveDirectoryCreated(this._saveDirectory)) return;
			
			// get file and dir name to save
			var fileNameWithDir = $"{this._saveDirectory}/{this._currentProfile}{SAVE_FILE_SUFIX}";

			// prepare data to save
			string jsonString = this._saveDataJsonNode.ToString();
			Debug.Log("Saving Json to disk: \n" + jsonString);
			
			using (var fileStream = new StreamWriter(fileNameWithDir, false)) {
				if(Debug.isDebugBuild) Debug.Log($"Saving '{this._currentProfile}' game data to disk: {jsonString}");
				await fileStream.WriteAsync(jsonString);
				Debug.Log($"Saved.");
			}
		}

		public async Task LoadFromDisk() {
			if (!this.IsSaveDirectoryCreated(this._saveDirectory)) return;
			
			var fileNameWithDir = $"{this._saveDirectory}/{this._currentProfile}{SAVE_FILE_SUFIX}";
			
			// load file from disk
			if (File.Exists(fileNameWithDir)) {
				using (var fileStream = File.OpenText(fileNameWithDir)) {
					this._saveDataJsonNode = JSON.Parse(await fileStream.ReadToEndAsync());
				}
			}
			
			// if is null create an empty save data
			if (this._saveDataJsonNode == null) {
				this._saveDataJsonNode = new JSONObject();
				this._saveDataJsonNode[this._currentProfile] = new JSONObject();
			}
			
			// file loaded
			if(Debug.isDebugBuild) Debug.Log("Loaded Json content: \n" + this._saveDataJsonNode.ToString());
			this._loadedSave = true;
		}

		private bool IsSaveDirectoryCreated(string saveDir) {
			var directoryInfo = Directory.CreateDirectory(saveDir);
			bool dirExists = directoryInfo.Exists;
			if (!dirExists) {
				Debug.LogError($"Could not save game. Directory could not be found: {saveDir}");
			}
			return dirExists;
		}
		
		#endregion <<---------- Saving and Loading on Disk ---------->>
		
		


		#region <<---------- Specific Save ---------->>
		
		public void SaveData_GameObject(CSaveGameObjectData value) {
			var serializedJson = UnityJSON.JSON.Serialize(value);
			Debug.Log($"Saving GameObject data ID {value.MyUid}:\n{serializedJson}");
			this._saveDataJsonNode[this._currentProfile].Add(value.MyUid, serializedJson);
		}
		
		#endregion <<---------- Specific Save ---------->>

		
		

		#region <<---------- Specific Load ---------->>

		public CSaveGameObjectData LoadData_GameObject(string key) {
			
			JSONNode loadedJson = this.LoadDataFromKey(key);
			if (loadedJson == null) return null;
			CSaveGameObjectData loadedObject;
			try {
				loadedObject = UnityJSON.JSON.Deserialize<CSaveGameObjectData>(loadedJson);
			}
			catch (Exception e) {
				Console.WriteLine(e);
				throw;
			}
			return loadedObject;
		}
		
		#endregion <<---------- Specific Load ---------->>

	}
}
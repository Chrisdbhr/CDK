using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if Newtonsoft_Json_for_Unity
using Newtonsoft.Json;
#endif

namespace CDK {
    public abstract class CGameProgressBase : CPersistentData {

        #region <<---------- Initializers ---------->>

        public CGameProgressBase(string name = null, bool isAutoInitialized = false) {
            this.WasLoadedAutomatically = isAutoInitialized;
            this.SaveIdentifier = Guid.NewGuid().ToString();
            if (!name.CIsNullOrEmpty()) this.SaveDescriptiveName = name;
            this.SaveDate = DateTime.UtcNow;
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        public static event Action OnNotifyForExternalModifiedSaveFile {
            add {
                _onNotifyForExternalModifiedSaveFile -= value;
                _onNotifyForExternalModifiedSaveFile += value;
            }
            remove {
                _onNotifyForExternalModifiedSaveFile -= value;
            }
        }
        [JsonIgnore]
        private static Action _onNotifyForExternalModifiedSaveFile;

        [JsonProperty("_appVersionWhenCreated")]
        public string AppVersionWhenCreated = Application.version;

        [JsonProperty("_appVersionOnLastSave")]
        public string AppVersionOnLastSave;

        [JsonIgnore]
        public bool WasLoadedAutomatically { get; }

        [JsonIgnore]
        public const string SavesDirectoryName = "SavesDir";

        [JsonProperty("_saveIdentifier")]
        public string SaveIdentifier;

        [JsonProperty("_saveDescriptiveName")]
        public string SaveDescriptiveName = "Save";

        [JsonProperty("_saveDateTime")]
        public DateTime SaveDate;

        [JsonProperty("_saveHash")]
        public string SaveHash;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Save ---------->>

        public virtual bool Save() {
            return this.SaveJson();
        }

        private bool SaveJson() {
            this.SaveDate = DateTime.UtcNow;
            this.SaveHash = String.Empty;
            this.AppVersionOnLastSave = Application.version;

            // serialized json without hash
            this.SaveHash = Animator.StringToHash(this.GetSerializedJson()).ToString();
            // then serialize again and save with hash
            return SaveJsonTextToFile(this.GetSerializedJson(), GetGameProgressFilePath(this.SaveIdentifier));
        }

        private string GetSerializedJson() {
            #if Newtonsoft_Json_for_Unity
            return JsonConvert.SerializeObject(this, CJsonExtensions.DefaultSettings);
            #else
            return JsonUtility.ToJson(this);
            #endif
        }

        #endregion <<---------- Save ---------->>

        
        

        #region <<---------- Loading ---------->>

        public static T LoadFromId<T>(string saveId) where T : CPersistentData {
            try {
                var filePath = GetGameProgressFilePath(saveId);
				
                Debug.Log($"Trying to LoadGameProgress with file '{filePath}'");

                if (!File.Exists(filePath)) {
                    Debug.LogWarning($"SaveGameProgress file at path '{filePath}' doesn't exist!");
                    return null;
                }
				
                return LoadFromPath<T>(filePath);
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
			
            return null;
        }
        
        public static T LoadFromPath<T>(string filePath) where T : CPersistentData {
            try {
                var fileContent = File.ReadAllText(filePath);
				
				var save = DeserializeFile<T>(fileContent);
                if (save == null) {
                    Debug.LogError($"Could not deserialize Save at path '{filePath}'!");
                    return null;
                }

                CheckForModifiedFile(save);

                Debug.Log($"GameProgress Loaded.");
                return save;
            }
            catch (Exception e) {
                Debug.LogError(e);
            }

            return null;
        }

        private static T DeserializeFile<T>(string fileContent) where T : CPersistentData {
            #if Newtonsoft_Json_for_Unity
            return JsonConvert.DeserializeObject<T>(fileContent, CJsonExtensions.DefaultSettings);
            #else
			return JsonUtility.FromJson<T>(fileContent);
            #endif
        }

        /// <summary>
        /// Never returns a null list.
        /// </summary>
        public static List<T> GetAllSaveFiles<T>() where T : CGameProgressBase {
            List<T> saves = new List<T>();
            try {
                var filesPaths = Directory.GetFiles(GetGameProgressFolder(), "*.json");
                foreach (var filePath in filesPaths) {
                    try {
                        var save = LoadFromPath<T>(filePath);
                        if (save == null) continue;
                        saves.Add(save);
                    }
                    catch (Exception e) {
                        Debug.LogError($"Error reading file from path '{filePath}'" + e);
                    }
                }
            }
            catch (Exception e) {
                Debug.LogError("Could not load all save files: " + e);
            }
            return saves;
        }

        #endregion <<---------- Loading ---------->>

        
        
        
        #region <<---------- Paths ---------->>

        public static string GetGameProgressFolder() {
            var folderPath = Path.Combine(GetApplicationPersistentDataFolder(), SavesDirectoryName).Replace('\\', '/');
            if (!Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
            }
            Debug.Log($"{nameof(GetGameProgressFolder)} returned: '{folderPath}'");
            return folderPath;
        }
        public static string GetGameProgressFilePath(string fileName) {
            return Path.Combine(GetGameProgressFolder(), $"{fileName}.json");
        }

        #endregion <<---------- Paths ---------->>




        #region <<---------- General ---------->>

        private static void CheckForModifiedFile<T>(T dataT) {
            if (!(dataT is CGameProgressBase data)) return;
            var originalHash = data.SaveHash;
            data.SaveHash = string.Empty;
            var newHash = Animator.StringToHash(data.GetSerializedJson()).ToString();
            if (originalHash != newHash) {
                Debug.Log($"Save file '{data.SaveIdentifier}' was modified externally!");
                _onNotifyForExternalModifiedSaveFile?.Invoke();
            }
            data.SaveHash = originalHash;
        }

        #endregion <<---------- General ---------->>

    }
}
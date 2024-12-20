#if NEWTONSOFT_JSON
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]
    public abstract class CGameStateBase : CPersistentData {

        #region <<---------- Initializers ---------->>

        protected CGameStateBase(string name = null, bool isAutoInitialized = false) {
            this.WasLoadedAutomatically = isAutoInitialized;
            this.SaveIdentifier = Guid.NewGuid().ToString();
            if (!name.CIsNullOrEmpty()) this.SaveDescriptiveName = name;
            this.SaveDate = DateTime.UtcNow;
            AppVersionWhenCreated = new Version(Application.version);
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        public static event Action OnNotifyForExternalModifiedSaveFile {
            add {
                _onNotifyForExternalModifiedSaveFile -= value;
                _onNotifyForExternalModifiedSaveFile += value;
            }
            remove => _onNotifyForExternalModifiedSaveFile -= value;
        }
        [NonSerialized] static Action _onNotifyForExternalModifiedSaveFile;

        [JsonProperty("_appVersionWhenCreated"), SerializeField]
        string _appVersionWhenCreated;
        public Version AppVersionWhenCreated {
            get => Version.TryParse(_appVersionWhenCreated, out var version) ? version : default;
            set => _appVersionWhenCreated = value.ToString();
        }

        [JsonProperty("_appVersionOnLastSave"), SerializeField]
        string _appVersionOnLastSave;
        public Version AppVersionOnLastSave {
            get => Version.TryParse(_appVersionOnLastSave, out var version) ? version : default;
            set => _appVersionOnLastSave = value.ToString();
        }

        public bool WasLoadedAutomatically { get; }

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

        bool SaveJson() {
            this.SaveDate = DateTime.UtcNow;
            this.SaveHash = String.Empty;
            if (Version.TryParse(Application.version, out var version)) {
                this.AppVersionOnLastSave = version;
            }
            // serialized json without hash
            this.SaveHash = Animator.StringToHash(this.GetSerializedJson()).ToString();
            // then serialize again and save with hash
            return SaveJsonTextToFile(this.GetSerializedJson(), GetGameStateFilePath(this.SaveIdentifier));
        }

        string GetSerializedJson() {
            #if NEWTONSOFT_JSON
            return JsonConvert.SerializeObject(this, CJsonExtensions.DefaultSettings);
            #else
            return JsonUtility.ToJson(this);
            #endif
        }

        #endregion <<---------- Save ---------->>

        
        

        #region <<---------- Loading ---------->>

        public static T LoadFromId<T>(string saveId) where T : CPersistentData {
            try {
                var filePath = GetGameStateFilePath(saveId);
				
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

        static T DeserializeFile<T>(string fileContent) where T : CPersistentData {
            #if NEWTONSOFT_JSON
            return JsonConvert.DeserializeObject<T>(fileContent, CJsonExtensions.DefaultSettings);
            #else
			return JsonUtility.FromJson<T>(fileContent);
            #endif
        }

        /// <summary>
        /// Never returns a null list.
        /// </summary>
        public static List<T> GetAllSaveFiles<T>() where T : CGameStateBase {
            List<T> saves = new List<T>();
            try {
                var filesPaths = Directory.GetFiles(GetGameStateFolder(), "*.json");
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




        #region Deleting

        public bool DeleteSave() {
            try {
                var filePath = GetGameStateFilePath(this.SaveIdentifier);
                if (!File.Exists(filePath)) return false;
                File.Delete(filePath);
                return true;
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
            return false;
        }

        #endregion Deleting

        


        #region <<---------- Paths ---------->>

        public static string GetGameStateFolder() {
            var folderPath = Path.Combine(GetApplicationPersistentDataFolder(), SavesDirectoryName).Replace('\\', '/');
            if (!Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
            }
            Debug.Log($"{nameof(GetGameStateFolder)} returned: '{folderPath}'");
            return folderPath;
        }
        public static string GetGameStateFilePath(string fileName) {
            return Path.Combine(GetGameStateFolder(), $"{fileName}.json");
        }

        #endregion <<---------- Paths ---------->>




        #region <<---------- General ---------->>

        public static bool CheckForModifiedFile<T>(T dataT) {
            if (!(dataT is CGameStateBase data)) return true;
            var originalHash = data.SaveHash;
            data.SaveHash = string.Empty;
            var newHash = Animator.StringToHash(data.GetSerializedJson()).ToString();
            if (originalHash != newHash) {
                Debug.Log($"Save file '{data.SaveIdentifier}' was modified externally!");
                _onNotifyForExternalModifiedSaveFile?.Invoke();
                return true;
            }
            data.SaveHash = originalHash;
            return false;
        }

        #endregion <<---------- General ---------->>

    }
}
#endif

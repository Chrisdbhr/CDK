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

        public CGameProgressBase(string name = null) {
            this.SaveIdentifier = Guid.NewGuid().ToString();
            if (!name.CIsNullOrEmpty()) this.SaveDescriptiveName = name;
            this.SaveDate = DateTime.UtcNow;
        }

        #endregion <<---------- Initializers ---------->>

        
        
        
        #region <<---------- Properties and Fields ---------->>

        [JsonIgnore]
        public const string SavesDirectoryName = "SavesDir";  
        
        #if Newtonsoft_Json_for_Unity
        [JsonProperty("_saveIdentifier")]
        #endif
        public string SaveIdentifier;

        #if Newtonsoft_Json_for_Unity
        [JsonProperty("_saveDescriptiveName")]
        #endif
        public string SaveDescriptiveName = "Save";
        
        #if Newtonsoft_Json_for_Unity
        [JsonProperty("_saveDateTime")]
        #endif
        public DateTime SaveDate;
        
        #endregion <<---------- Properties and Fields ---------->>
        
        
        
        
        #region <<---------- Save ---------->>

        public virtual bool Save() {
            this.SaveDate = DateTime.UtcNow;
            return this.SaveJson();
        }
        
        private bool SaveJson() {
            #if Newtonsoft_Json_for_Unity
            var json = JsonConvert.SerializeObject(this, CJsonExtensions.DefaultSettings);
            #else
            var json = JsonUtility.ToJson(data);
			#endif
            return SaveJsonTextToFile(json, GetGameProgressFilePath(this.SaveIdentifier));
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
				
                Debug.Log($"SaveGameProgress file content: {fileContent}");

				#if Newtonsoft_Json_for_Unity
                var save = JsonConvert.DeserializeObject<T>(fileContent, CJsonExtensions.DefaultSettings);
                #else
				var save = JsonUtility.FromJson<T>(fileContent);
				#endif
                if (save == null) {
                    Debug.LogError($"Could not deserialize Save at path '{filePath}'!");
                    return null;
                }

                Debug.Log($"GameProgress Loaded.");
                return save;
            }
            catch (Exception e) {
                Debug.LogError(e);
            }

            return null;
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

    }
}
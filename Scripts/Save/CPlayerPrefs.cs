#if Newtonsoft_Json_for_Unity
using Newtonsoft.Json;
#endif

using System;
using System.IO;
using UnityEngine;

namespace CDK {
    public class CPlayerPrefs : CPersistentData {
        
        #region <<---------- Singleton ---------->>
        
        public static CPlayerPrefs Current {
            get {
                return _current ?? new CPlayerPrefs();
            }
            set {
                if (value == null) {
                    Debug.LogError($"Will not set {nameof(CPlayerPrefs)} Current value to null, will create a new instance instead!");
                }
                _current = new CPlayerPrefs();
            }
        }
        private static CPlayerPrefs _current;
        
        #endregion <<---------- Singleton ---------->>
        
        
        
        
        #region <<---------- Properties and Fields ---------->>

        #region <<---------- Camera ---------->>

        #if Newtonsoft_Json_for_Unity
        [JsonProperty("cameraSensitivity")]
		#endif
        public Vector2 CameraSensitivity {
            get => this._cameraSensitivity;
            set {
                if (value.x == this._cameraSensitivity.x && value.y == this._cameraSensitivity.y) return;
                this._cameraSensitivity = value;
            }
        }
        [JsonIgnore]
        private Vector2 _cameraSensitivity = DefaultMaxCameraSensitivity * 0.5f;

        public void SetCameraSensitivityX(float value) {
            this._cameraSensitivity = new Vector2(this._cameraSensitivity.x, value);
        }
        public void SetCameraSensitivityY(float value) {
            this._cameraSensitivity = new Vector2(value, this._cameraSensitivity.y);
        }
        
		#if Newtonsoft_Json_for_Unity
        [JsonIgnore]
		#endif
        public static readonly Vector2 DefaultMaxCameraSensitivity = new Vector2(64f * 2f, 0.56f * 2f);

        #endregion <<---------- Camera ---------->>

		#if Newtonsoft_Json_for_Unity
        [JsonProperty("language")]
		#endif
        public string Language;

        #endregion <<---------- Properties and Fields ---------->>


        

        #region <<---------- Load and Save ---------->>

        public static CPlayerPrefs LoadPlayerPrefs() {
            try {
                var filePath = GetPlayerPrefsFilePath();
            
                if (!File.Exists(filePath)) {
                    Debug.LogWarning($"PlayerPrefs file at path '{filePath}' doesn't exist!");
                    return default;
                }
            
                var fileContent = File.ReadAllText(filePath);
                Debug.Log($"PlayerPrefs file content: {fileContent}");

                #if Newtonsoft_Json_for_Unity
                var prefs = JsonConvert.DeserializeObject<CPlayerPrefs>(fileContent, CJsonExtensions.DefaultSettings);
                #else
				var prefs = JsonUtility.FromJson<CPlayerPrefs>(fileContent);
				#endif
                
                if (prefs == null) {
                    Debug.LogError($"Could not deserialize PlayerPrefs at path '{filePath}'!");
                    return default;
                }

                Debug.Log($"PlayerPrefs Loaded.");
				
                return prefs;
            }
            catch (Exception e) {
                Debug.LogError(e);
            }

            return default;
        }

       
        public static void SaveCurrent() {
            Current.Save();
        }
        
        private void Save() {
            try {
                #if Newtonsoft_Json_for_Unity
                var json = JsonConvert.SerializeObject(this, CJsonExtensions.DefaultSettings);
                #else
                var json = JsonUtility.ToJson(prefs);
			    #endif

                SaveJsonTextToFile(json, GetPlayerPrefsFilePath());
                
                return;
            }
            catch (Exception e) {
                Debug.LogError(e);
                return;
            }
        }

        #endregion <<---------- Load and Save ---------->>


        

        #region <<---------- Paths ---------->>

        protected static string GetPlayerPrefsFilePath() {
            return GetRootSaveFolder() + "/playerPrefs.json";
        }

        #endregion <<---------- Paths ---------->>

    }
}
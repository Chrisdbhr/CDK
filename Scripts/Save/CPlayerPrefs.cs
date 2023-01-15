#if Newtonsoft_Json_for_Unity
using Newtonsoft.Json;
#endif

using System;
using System.IO;
using UniRx;
using UnityEngine;

namespace CDK {
    public class CPlayerPrefs : CPersistentData {
        
        #region <<---------- Singleton ---------->>
        
        public static CPlayerPrefs Current {
            get {
                return _current ??= LoadOrGetNewPlayerPrefs();
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
        
        [JsonIgnore]
        public Vector2 CameraSensitivityMultiplier {
            get => this._cameraSensitivityMultiplier;
        }
        #if Newtonsoft_Json_for_Unity
        [JsonProperty("cameraSensitivityMultiplier")]
		#endif
        private Vector2 _cameraSensitivityMultiplier = Vector3.one * 0.5f;

        public void SetCameraSensitivity(float value) {
            this._cameraSensitivityMultiplier.x = value;
            this._cameraSensitivityMultiplier.y = value;
        }
        public void SetCameraSensitivityX(float value) {
            this._cameraSensitivityMultiplier = new Vector2(value, this._cameraSensitivityMultiplier.y);
        }
        public void SetCameraSensitivityY(float value) {
            this._cameraSensitivityMultiplier = new Vector2(this._cameraSensitivityMultiplier.x, value);
        }

        #if Newtonsoft_Json_for_Unity
        [JsonProperty("resolutionScale")]
		#endif
        public float ResolutionScale {
            get {
                return this._resolutionScaleRx.Value;
            }
            set {
                if (value == this._resolutionScaleRx.Value) return;
                this._resolutionScaleRx.Value = value;
            }
        }
        #if Newtonsoft_Json_for_Unity
        [JsonIgnore]
		#endif
        private FloatReactiveProperty _resolutionScaleRx = new FloatReactiveProperty(1f);
        #if Newtonsoft_Json_for_Unity
        [JsonIgnore]
		#endif
        public IReadOnlyReactiveProperty<float> ResolutionScaleRx => this._resolutionScaleRx.ToReadOnlyReactiveProperty();

        #endregion <<---------- Camera ---------->>

		#if Newtonsoft_Json_for_Unity
        [JsonProperty("language")]
		#endif
        public string Language;

        #endregion <<---------- Properties and Fields ---------->>


        

        #region <<---------- Load and Save ---------->>

        private static CPlayerPrefs LoadOrGetNewPlayerPrefs() {
            try {
                var filePath = GetPlayerPrefsFilePath();
            
                if (!File.Exists(filePath)) {
                    Debug.LogWarning($"PlayerPrefs file at path '{filePath}' doesn't exist!");
                    return new CPlayerPrefs();
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
                    return new CPlayerPrefs();
                }

                Debug.Log($"PlayerPrefs Loaded.");
				
                return prefs;
            }
            catch (Exception e) {
                Debug.LogError(e);
            }

            return new CPlayerPrefs();
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
            return GetApplicationPersistentDataFolder() + "/playerPrefs.json";
        }

        #endregion <<---------- Paths ---------->>

    }
}
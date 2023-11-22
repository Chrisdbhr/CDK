using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CDK {
	[System.Serializable]
	public class CSceneField : ISerializationCallbackReceiver {
        
        #region <<---------- Properties and Fields ---------->>

		#if UNITY_EDITOR
        [Obsolete("Editor only variable, do not use on runtime scripts!")]
        public SceneAsset sceneAsset;
		#endif

#pragma warning disable 414
        [SerializeField, HideInInspector]
        private string sceneName = "";
#pragma warning restore 414

        #endregion <<---------- Properties and Fields ---------->>

        
        

        #region <<---------- Operator Overload ---------->>

        // Makes it work with the existing Unity methods (LoadLevel/LoadScene)
        public static implicit operator string(CSceneField sceneField) {
			#if UNITY_EDITOR
            return Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(sceneField.sceneAsset));
			#else
			return sceneField.sceneName;
			#endif
        }
        
        public static bool operator ==(CSceneField a, CSceneField b) {
            return a?.sceneName == b?.sceneName;
        }
        public static bool operator !=(CSceneField a, CSceneField b) => !(a == b);

        #endregion <<---------- Operator Overload ---------->>


        
        
        #region <<---------- Serialization ---------->>

        public void OnBeforeSerialize() {
			#if UNITY_EDITOR
            this.sceneName = this;
			#endif
        }

        public void OnAfterDeserialize() { }

        #endregion <<---------- Serialization ---------->>


        

        #region <<---------- Public Methods ---------->>
        
        public void LoadScene() {
            SceneManager.LoadScene(this.sceneName);
        }

        public void LoadSceneAdditive() {
            SceneManager.LoadScene(this.sceneName, LoadSceneMode.Additive);
        }

        #endregion <<---------- Public Methods ---------->>

	}
}
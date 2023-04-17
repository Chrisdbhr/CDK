using System;
using UnityEngine;

namespace CDK {
	[System.Serializable]
	public class CSceneField : ISerializationCallbackReceiver {
		#if UNITY_EDITOR
		[Obsolete("Do not use on runtime scripts! This is a editor only variable.")]
		public UnityEditor.SceneAsset sceneAsset;
		#endif

#pragma warning disable 414
		[SerializeField, HideInInspector]
		private string sceneName = "";
#pragma warning restore 414

		// Makes it work with the existing Unity methods (LoadLevel/LoadScene)
		public static implicit operator string(CSceneField sceneField) {
			#if UNITY_EDITOR
			return System.IO.Path.GetFileNameWithoutExtension(UnityEditor.AssetDatabase.GetAssetPath(sceneField.sceneAsset));
			#else
			return sceneField.sceneName;
			#endif
		}
        
        public static bool operator ==(CSceneField a, CSceneField b) {
            return a.sceneName == b.sceneName;
        }
        public static bool operator !=(CSceneField a, CSceneField b) => !(a == b);

		public void OnBeforeSerialize() {
			#if UNITY_EDITOR
			this.sceneName = this;
			#endif
		}

		public void OnAfterDeserialize() { }
	}
}

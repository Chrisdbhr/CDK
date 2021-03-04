using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	
	[ExecuteInEditMode]
	public class CVersionNumberGetter : MonoBehaviour{

		[SerializeField] private CUnityEventString versionStringEvent;

		private void Awake() {
			var path = "GameBundleVersion";
			var textAsset = Resources.Load<TextAsset>(path);
			try {
				this.versionStringEvent?.Invoke(textAsset.text ?? "");
			} catch (Exception e) {
				Debug.LogError($"Exception getting Version file, path was '{path}'\n{e}");
			}
		}

		#if UNITY_EDITOR
		private void OnValidate() {
			
			this.versionStringEvent?.Invoke(PlayerSettings.bundleVersion);
		}
		#endif
		
	}
}
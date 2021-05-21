using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	
	[ExecuteInEditMode]
	public class CVersionNumberGetter : MonoBehaviour {

		[SerializeField] private CUnityEventString versionStringEvent;
		[NonSerialized] private string _filePathOnResourcesFolder = "GameBundleVersion";

		private void Awake() {
			this.versionStringEvent?.Invoke(this.GetBundleVersion());
		}

		#if UNITY_EDITOR
		private void OnValidate() {
			
			this.versionStringEvent?.Invoke(this.GetBundleVersion());
		}
		#endif


		private string GetBundleVersion() {
			var version = string.Empty;
			#if UNITY_EDITOR
			version = PlayerSettings.bundleVersion;
			#endif

			try {
				var textAsset = Resources.Load<TextAsset>(this._filePathOnResourcesFolder);
				
				#if UNITY_EDITOR
				if (textAsset == null) {
					File.WriteAllText(this._filePathOnResourcesFolder + ".txt", version);
				}
				#endif

				if(textAsset != null) version = textAsset.text;
			}
			catch (Exception e) {
				Debug.LogError(e);
			}

			return version;
		}
	}
}
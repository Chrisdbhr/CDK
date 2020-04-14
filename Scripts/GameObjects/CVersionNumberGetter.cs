using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	
	[ExecuteInEditMode]
	public class CVersionNumberGetter : MonoBehaviour{

		[SerializeField] private CUnityEventString versionStringEvent;

		#if UNITY_EDITOR
		private void OnValidate() {
			this.GetVersion();
		}
		#endif
		
		private void Start() {
			this.GetVersion();
		}

		private void GetVersion() {
			this.versionStringEvent?.Invoke(((TextAsset)Resources.Load("GameVersion")).text);
		}
		
	}
}
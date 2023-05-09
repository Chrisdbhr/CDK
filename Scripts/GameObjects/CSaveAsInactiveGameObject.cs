using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace CDK {
	[ExecuteInEditMode]
	public class CSaveAsInactiveGameObject : MonoBehaviour {
		
#if UNITY_EDITOR
		
		private void Awake() {
			EditorSceneManager.sceneSaving += EditorSceneManagerOnSceneSaving; 
			EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
		}
		
		private void OnDestroy() {
			EditorSceneManager.sceneSaving -= EditorSceneManagerOnSceneSaving; 
			EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
		}
		
		private void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange s) {
			if (s != PlayModeStateChange.ExitingEditMode) return;
			SetSelfInactive();
		}
		
		private void EditorSceneManagerOnSceneSaving(Scene scene, string path) {
			SetSelfInactive();
		}
		
		private void SetSelfInactive() {
			if (!this.gameObject.activeSelf) return;
			this.gameObject.SetActive(false);
			Debug.Log($"Saving {this.name} as inactive game object in {this.gameObject.scene}");
		}
		
		#endif
	}
}

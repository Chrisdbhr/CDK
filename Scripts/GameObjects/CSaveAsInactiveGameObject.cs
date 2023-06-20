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
            PrefabStage.prefabSaving += PrefabStageSaving;
		}

        private void PrefabStageSaving(GameObject go) {
            if (go != this.gameObject) return;
            SetSelfInactive(go);
        }

        private void OnDestroy() {
			EditorSceneManager.sceneSaving -= EditorSceneManagerOnSceneSaving; 
			EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
            PrefabStage.prefabSaving -= PrefabStageSaving;
		}
		
		private void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange s) {
			if (s != PlayModeStateChange.ExitingEditMode) return;
			SetSelfInactive(this.gameObject);
		}
		
		private void EditorSceneManagerOnSceneSaving(Scene scene, string path) {
			SetSelfInactive(this.gameObject);
		}
		
		private void SetSelfInactive(GameObject go) {
            if (!go.activeSelf) return;
            go.SetActive(false);
			Debug.Log($"Saving {this.name} as inactive game object in {go.scene}");
		}
		
		#endif
	}
}

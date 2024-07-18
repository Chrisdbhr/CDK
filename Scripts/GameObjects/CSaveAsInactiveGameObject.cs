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
		void Awake() {
			this.SignToEvents();
		}

		void Reset() {
			this.SignToEvents();
		}

		void OnValidate() {
			this.SignToEvents();
		}

		void SignToEvents() {
			if (Application.isPlaying) return;
			EditorSceneManager.sceneSaving -= EditorSceneManagerOnSceneSaving;
			EditorSceneManager.sceneSaving += EditorSceneManagerOnSceneSaving;

			EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
			EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;

			PrefabStage.prefabSaving -= PrefabStageSaving;
			PrefabStage.prefabSaving += PrefabStageSaving;
		}

        private void PrefabStageSaving(GameObject go) {
            if (this == null) return;
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
            if (this == null) return;
			SetSelfInactive(this.gameObject);
		}
		
		private void EditorSceneManagerOnSceneSaving(Scene scene, string path) {
            if (this == null) return;
			SetSelfInactive(this.gameObject);
		}
		
		private void SetSelfInactive(GameObject go) {
            if (this == null) return;
            if (!go.activeSelf) return;
            go.SetActive(false);
			Debug.Log($"Saving '{this.name}' as inactive game object in '{go.scene.name}'");
		}
		#endif
	}
}
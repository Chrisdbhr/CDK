using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace CDK {
    [ExecuteInEditMode]
    public class CBakeSceneOnOpenEditor : MonoBehaviour {
        private void Awake() {
            if (Application.isPlaying) return;
            #if UNITY_EDITOR
            Selection.activeGameObject = this.gameObject;
            if (EditorSceneManager.sceneCount > 1) {
                Debug.LogWarning("Canceling auto bake because there is more than one scene open.", this);
                return;
            }
            Debug.Log("Auto baking scene.", this);
            Lightmapping.BakeAsync();
            #endif
        }
    }
}
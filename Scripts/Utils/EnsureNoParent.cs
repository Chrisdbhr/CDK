using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
    [ExecuteInEditMode, DisallowMultipleComponent]
    public class EnsureNoParent : MonoBehaviour {
        void OnValidate() {
            EnsureNoParentObject();
        }

        void Reset() {
            EnsureNoParentObject();
        }

        void EnsureNoParentObject() {
            if (transform.parent == null) return;
            transform.parent = null;
            Debug.LogWarning($"{name} need to be a root object. It parent has been removed.", this);
            MarkSceneAsDirty();
        }

        void MarkSceneAsDirty() {
            #if UNITY_EDITOR
            if (Application.isPlaying) return;
            EditorUtility.SetDirty(gameObject);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            #endif
        }
    }
}
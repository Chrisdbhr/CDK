using UnityEngine;

namespace CDK {
    public static class CComponentExtensions {

        public static T CGetComponentInChildrenOrInParent<T>(this Component comp, bool includeInactive = false) {
            return comp.gameObject.CGetComponentInChildrenOrInParent<T>(includeInactive);
        }
        
        public static T CGetComponentInParentOrInChildren<T>(this Component comp, bool includeInactive = false) {
            return comp.gameObject.CGetComponentInParentOrInChildren<T>(includeInactive);

        }
     
        public static void CDestroyGameObject(this Component value, float time = 0f) {
            if (value == null || value.gameObject == null) return;
            if (Application.isPlaying) {
                Object.Destroy(value.gameObject, time > 0f ? time : 0f);
            }
            else {
                Object.DestroyImmediate(value.gameObject);
            }
        }

        public static T CGetOrAddComponent<T>(this Component value) where T : Component {
            if (value == null) {
                Debug.LogError($"Cant add component to a null component!");
                return default;
            }

            var comp = value.GetComponent<T>();
            return (comp != null ? comp : value.gameObject.AddComponent<T>());
        }
        
    }
}
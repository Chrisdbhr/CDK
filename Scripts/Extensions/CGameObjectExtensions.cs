using Unity.Linq;
using UnityEngine;

namespace CDK {
    public static class CGameObjectExtensions {
        
        public static T CGetOrAddComponent<T>(this GameObject go) where T : Component {
            var comp = go.GetComponent<T>();
            return comp != null ? comp : go.AddComponent<T>();
        }

        public static GameObject CSetLayerFromLayerMask(this GameObject go, LayerMask layer) {
            if (go == null) return null;
            go.layer = (int)Mathf.Log(layer.value, 2);
            return go;
        }
        
        public static T CGetComponentInChildrenOrInParent<T>(this GameObject go, bool includeInactive = false) {
            if (go == null) return default;
            var target = go.GetComponentInChildren<T>(includeInactive);
            if (target != null) return target;
            return go.GetComponentInParent<T>(includeInactive);
        }

        public static T CGetComponentInParentOrInChildren<T>(this GameObject go, bool includeInactive = false) {
            if (go == null) return default;
            var target = go.GetComponentInParent<T>(includeInactive);
            if (target != null) return target;
            return go.GetComponentInChildren<T>(includeInactive);
        }
        
        public static T CGetComponentInChildrenRecursiveUntilRoot<T>(this GameObject go, bool includeInactive = false) {
            if (go == null) return default;
            T target = default;
            foreach (var ancestor in go.AncestorsAndSelf()) {
                target = ancestor.GetComponentInChildren<T>(includeInactive);
                if (target != null) {
                    return target;
                }
            }
            return target;
        }

        public static GameObject CDontDestroyOnLoad(this GameObject go) {
            GameObject.DontDestroyOnLoad(go);
            return go;
        }
        
    }
}
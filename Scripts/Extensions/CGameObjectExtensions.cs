using System.Linq;
using Unity.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
    public static class CGameObjectExtensions {
        
        public static T CGetOrAddComponent<T>(this GameObject go) where T : Component {
            if (go == null) {
                Debug.LogError("Cant get or add component from null GameObject!");
                return null;
            }
            return go.TryGetComponent<T>(out var comp) ? comp : go.AddComponent<T>();
        }

        public static T CGetInChildrenOrAddComponent<T>(this GameObject go) where T : Component {
            var comp = go.GetComponentInChildren<T>();
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
            Debug.Log($"Setting '{go.name}' to DontDestroyOnLoad");
            Object.DontDestroyOnLoad(go);
            return go;
        }
        
        public static void CUnparentAllChildren(this GameObject go, bool worldPositionStays = true) {
            if (go == null) return;
            foreach (var c in go.Children().ToArray()) {
                c.transform.SetParent(null, worldPositionStays);
                c.transform.SetAsLastSibling();
            }
        }

        public static void CDestroyAllChildren(this GameObject go)
        {
            if (go == null) return;
            var allChild = go.Children().Where(g=>g!= null);
            #if UNITY_EDITOR
            Undo.RecordObjects(allChild.ToArray(), go.name + "_child");
            #endif
            foreach (var child in allChild) {
                if(child == null) continue;
                child.gameObject.CDestroy();
            }
        }
        
    }
}
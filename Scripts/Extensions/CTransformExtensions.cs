using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public static class CTransformExtensions {

        public static void CRotateTowardsDirection(this Transform t, Vector3 dir, float maxDegreesDelta, float timeScale = 1f) {
            if (timeScale == 0f) return;
            dir.y = 0f;
            if (dir == Vector3.zero) return;

            // lerp rotation
            t.rotation = Quaternion.RotateTowards(
                t.rotation,
                Quaternion.LookRotation(dir),
                maxDegreesDelta * timeScale
            );
        }
        
		public static void CDeleteAllChildren(this Transform parentGameObject) {
			var allChild = parentGameObject.GetComponentsInChildren<Transform>();
            #if UNITY_EDITOR
            Undo.RecordObjects(allChild, parentGameObject.name + "_child");
            #endif
			foreach (var child in allChild) {
				if(child == null || child.transform == parentGameObject) continue;
				UnityEngine.Object.DestroyImmediate(child.gameObject);
			}
		}

        public static Transform CGetParentOrSelf(this Transform t) {
            var target = t.parent;
            return target != null ? target : t;
        }
        
        public static void CSetRotationToInverseNormal(this Transform t, Vector3 checkDirection, float checkDistance, LayerMask layerMask) {
            if (t == null) return;
            if (!Physics.Raycast(t.position, checkDirection, out var hit, checkDistance, layerMask)) return;
            t.rotation = Quaternion.FromToRotation(t.up, hit.normal);
        }

        public static void CAssertIfScaleIsNotOne(this Transform t) {
            if (t.localScale.CIsOne()) return;
            Debug.LogError($"Transform '{t.name}' is not one!");
        }
        
        public static bool CAssertIfNotRoot(this Transform t, string message = null) {
            var isRoot = t.parent == null;
            if (!isRoot) {
                Debug.LogAssertion($"Transform is not root ({message})");
            }
            return !isRoot;
        }
        
        public static bool CAssertLocalPositionIsNotZero(this Transform t, string message = null) {
            var isZero = t.localPosition.CIsZero();
            if (!isZero) {
                Debug.LogAssertion($"Transform local position is not zero ({message})");
            }
            return !isZero;
        }

        public static void CUnparentAllChildren(this Transform t) {
            if (t == null) return;
            t.gameObject.CUnparentAllChildren();
        }
        
    }
}
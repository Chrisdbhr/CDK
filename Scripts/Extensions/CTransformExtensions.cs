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

		public static void CDestroyAllChildren(this Transform t)
        {
            if (t == null) return;
			t.gameObject.CDestroyAllChildren();
		}

        public static Transform CGetParentOrSelf(this Transform t) {
            var target = t.parent;
            return target != null ? target : t;
        }
        
        public static void CSetRotationToInverseNormal(this Transform t, Vector3 checkDirection, float checkDistance, LayerMask layerMask) {
            if (t == null) return;
            if (!Physics.Raycast(t.position, checkDirection, out var hit, checkDistance, layerMask)) return;
            var target = Quaternion.FromToRotation(t.up, hit.normal);
            if(target == Quaternion.identity) return;
            t.rotation = target;
        }

        public static void CAssertIfScaleIsNotOne(this Transform t) {
            if (t.localScale.CIsOne()) return;
            Debug.LogError($"Transform '{t.name}' is not one!");
        }
        
        public static bool CAssertIfNotRoot(this Transform t, string message = null) {
            var isRoot = t.parent == null;
            if (!isRoot) {
                Debug.LogError($"Transform is not root ({message})");
            }
            return !isRoot;
        }
        
        public static bool CAssertLocalPositionIsNotZero(this Transform t, string message = null) {
            var isZero = t.localPosition.CIsZero();
            if (!isZero) {
                Debug.LogError($"Transform local position is not zero ({message})");
            }
            return !isZero;
        }

        public static void CUnparentAllChildren(this Transform t) {
            if (t == null) return;
            t.gameObject.CUnparentAllChildren();
        }

        public static void CResetTransform(this Transform t) {
            if (t == null) return;
            t.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            t.localScale = Vector3.one;
        }

        public static Vector3 CGetRelative3dPlanarDirection(this Transform t, Vector3 dir)
        {
            var forward = t.forward;
            var right = t.right;

            // project forward and right vectors on the horizontal plane (y = 0)
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            return (forward * dir.z) + (right * dir.x);
        }

    }
}
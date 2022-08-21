using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public static class CTransformExtensions {

		public static void DeleteAllChildren(this Transform parentGameObject) {
			var allChild = parentGameObject.GetComponentsInChildren<Transform>();
            #if UNITY_EDITOR
            Undo.RecordObjects(allChild, parentGameObject.name + "_child");
            #endif
			foreach (var child in allChild) {
				if(child == null || child.transform == parentGameObject) continue;
				UnityEngine.Object.DestroyImmediate(child.gameObject);
			}
		}

        public static Transform GetParentOrSelf(this Transform t) {
            var target = t.parent;
            return target != null ? target : t;
        }

	}
}
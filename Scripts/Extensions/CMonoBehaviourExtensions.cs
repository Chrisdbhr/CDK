using System.Collections;
using UnityEditor;
using UnityEngine;

namespace CDK {
	public static class CMonoBehaviourExtensions {
		
		public static Coroutine CStartCoroutine(this MonoBehaviour monoBehaviour, IEnumerator coroutine) {
            if (!monoBehaviour.isActiveAndEnabled) return null;
			if (coroutine == null) return null;
			return monoBehaviour.StartCoroutine(coroutine);
		}
		
		public static void CStopCoroutine(this MonoBehaviour monoBehaviour, Coroutine coroutine) {
			if (coroutine == null) return;
			monoBehaviour.StopCoroutine(coroutine);
		}
        
        public static void CResolveComponentFromChildrenIfNull<T>(this MonoBehaviour m, ref T c) where T : Component {
            if (c != null) return;
            c = m.GetComponentInChildren<T>();
        }
        
        public static void CResolveComponentFromChildrenOrParentIfNull<T>(this MonoBehaviour m, ref T c) where T : Component {
            if (c != null) return;
            c = m.CGetComponentInChildrenOrInParent<T>();
        }

        public static void CSetNameIfOnlyComponent(this MonoBehaviour m, string name) {
            if (m == null) return;
            var comps = m.GetComponents<Component>();
            if (comps == null) return;
            if (comps.Length > 2) return;
            m.name = name;
            #if UNITY_EDITOR
            EditorUtility.SetDirty(m);
            #endif
        }

    }
}
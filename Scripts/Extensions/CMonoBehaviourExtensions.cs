using System.Collections;
using UnityEngine;

namespace CDK {
	public static class CMonoBehaviourExtensions {
		
		public static Coroutine CStartCoroutine(this MonoBehaviour monoBehaviour, IEnumerator coroutine) {
			if (coroutine == null) return null;
			return monoBehaviour.StartCoroutine(coroutine);
		}
		
		public static void CStopCoroutine(this MonoBehaviour monoBehaviour, Coroutine coroutine) {
			if (coroutine == null) return;
			monoBehaviour.StopCoroutine(coroutine);
		}

		public static Coroutine CRestartCoroutine(this MonoBehaviour monoBehaviour, IEnumerator coroutine) {
			if (coroutine == null) return null;
			monoBehaviour.StopCoroutine(coroutine);
			return monoBehaviour.StartCoroutine(coroutine);
		}
		
	}
}

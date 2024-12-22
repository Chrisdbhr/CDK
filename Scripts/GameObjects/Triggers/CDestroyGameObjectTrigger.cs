using UnityEngine;

namespace CDK {
	public class CDestroyGameObjectTrigger : CAutoTriggerCompBase {

		[SerializeField] bool _destroyThis;

		
		
		
		public void DestroySelfGameObject() {
			DestroyGameObject(gameObject);
		}

		public void DestroyGameObject(GameObject goToDestroy) {
			if (goToDestroy == null) {
				Debug.Log($"{nameof(CDestroyGameObjectTrigger)} received a request to destroy an empty game object.", gameObject);
				return;
			}
			Debug.Log($"{nameof(CDestroyGameObjectTrigger)} <color=orange>destroying game object</color> '{goToDestroy.name}'.", goToDestroy);
			goToDestroy.CDestroy();
		}

		protected override void TriggerEvent() {
			if (!_destroyThis) return;
			Debug.Log($"[{nameof(CDestroyGameObjectTrigger)}] '{name}' <color=orange>self destroying</color>.");
			gameObject.CDestroy();
		}
	}
}

using UnityEngine;

namespace CDK {
	public class CDestroyGameObjectTrigger : CAutoTriggerCompBase {

		[SerializeField] private bool _destroyThis;

		
		
		
		public void DestroySelfGameObject() {
			DestroyGameObject(this.gameObject);
		}

		public void DestroyGameObject(GameObject goToDestroy) {
			if (goToDestroy == null) {
				Debug.Log($"{nameof(CDestroyGameObjectTrigger)} received a request to destroy an empty game object.", this.gameObject);
				return;
			}
			Debug.Log($"{nameof(CDestroyGameObjectTrigger)} <color=orange>destroying game object</color> '{goToDestroy.name}'.", goToDestroy);
			goToDestroy.CDestroy();
		}

		protected override void TriggerEvent() {
			if (!this._destroyThis) return;
			Debug.Log($"{nameof(CDestroyGameObjectTrigger)} <color=orange>self destroying</color>.");
			this.gameObject.CDestroy();
		}
	}
}

using UnityEngine;

namespace CDK {
	public class CPhysicsTrigger : CPhysicsMonoBehaviourTriggers {
		
		private void OnTriggerEnter(Collider other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Enter?.Invoke(other.transform);
		}

		private void OnTriggerEnter2D(Collider2D other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Enter?.Invoke(other.transform);
		}

		private void OnTriggerExit(Collider other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Exit?.Invoke(other.transform);
		}

		private void OnTriggerExit2D(Collider2D other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Exit?.Invoke(other.transform);
		}
	}
}
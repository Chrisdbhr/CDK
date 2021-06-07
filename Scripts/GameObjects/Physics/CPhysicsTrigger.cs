using UnityEngine;

namespace CDK {
	public class CPhysicsTrigger : CPhysicsMonoBehaviourTriggers {
		
		private void OnTriggerEnter(Collider other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Enter?.Invoke(other.transform);
			this.Entered?.Invoke(true);
		}

		private void OnTriggerEnter2D(Collider2D other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Enter?.Invoke(other.transform);
			this.Entered?.Invoke(true);
		}

		private void OnTriggerExit(Collider other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Exit?.Invoke(other.transform);
			this.Entered?.Invoke(false);
		}

		private void OnTriggerExit2D(Collider2D other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Exit?.Invoke(other.transform);
			this.Entered?.Invoke(false);
		}
	}
}
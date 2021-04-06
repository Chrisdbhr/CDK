using UnityEngine;

namespace CDK {
	public class CPhysicsCollision : CPhysicsMonoBehaviourTriggers {
		
		private void OnCollisionEnter(Collision other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Enter?.Invoke(other.transform);
		}

		private void OnCollisionExit(Collision other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Exit?.Invoke(other.transform);
		}

		private void OnCollisionEnter2D(Collision2D other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Enter?.Invoke(other.transform);
		}

		private void OnCollisionExit2D(Collision2D other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Exit?.Invoke(other.transform);
		}

	}
}

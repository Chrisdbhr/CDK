using UnityEngine;

namespace CDK {
	public class CPhysicsCollision : CPhysicsMonoBehaviourTriggers {
		
		private void OnCollisionEnter(Collision other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Enter?.Invoke(other.transform);
			this.Entered?.Invoke(true);
		}

		private void OnCollisionExit(Collision other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Exit?.Invoke(other.transform);
			this.Entered?.Invoke(true);
		}

		private void OnCollisionEnter2D(Collision2D other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Enter?.Invoke(other.transform);
			this.Entered?.Invoke(false);
		}

		private void OnCollisionExit2D(Collision2D other) {
			if (this.WillIgnoreCollision(other.transform)) return;
			this.Exit?.Invoke(other.transform);
			this.Entered?.Invoke(false);
		}

	}
}

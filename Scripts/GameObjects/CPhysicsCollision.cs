using UnityEngine;

namespace CDK {
	public class CPhysicsCollision : MonoBehaviour {

		[SerializeField] private CUnityEventTransform CollisionEnter;
		[SerializeField] private CUnityEventTransform CollisionExit;

		
		
		
		private void OnCollisionEnter(Collision other) {
			this.CollisionEnter?.Invoke(other.transform);
		}

		private void OnCollisionExit(Collision other) {
			this.CollisionExit?.Invoke(other.transform);
		}

		private void OnCollisionEnter2D(Collision2D other) {
			this.CollisionEnter?.Invoke(other.transform);
		}

		private void OnCollisionExit2D(Collision2D other) {
			this.CollisionExit?.Invoke(other.transform);
		}
	}
}

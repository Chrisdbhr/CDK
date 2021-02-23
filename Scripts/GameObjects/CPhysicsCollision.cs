using UnityEngine;

namespace CDK {
	public class CPhysicsCollision : MonoBehaviour {

		[SerializeField] private bool TriggeredByPlayerOnly = false;

		[SerializeField] private CUnityEventTransform CollisionEnter;
		[SerializeField] private CUnityEventTransform CollisionExit;

		
		
		
		private void OnCollisionEnter(Collision other) {
			if (this.TriggeredByPlayerOnly && !CGamePlayerManager.get.IsRootTransformFromAPlayerCharacter(other.transform)) return;
			this.CollisionEnter?.Invoke(other.transform);
		}

		private void OnCollisionExit(Collision other) {
			if (this.TriggeredByPlayerOnly && !CGamePlayerManager.get.IsRootTransformFromAPlayerCharacter(other.transform)) return;
			this.CollisionExit?.Invoke(other.transform);
		}

		private void OnCollisionEnter2D(Collision2D other) {
			if (this.TriggeredByPlayerOnly && !CGamePlayerManager.get.IsRootTransformFromAPlayerCharacter(other.transform)) return;
			this.CollisionEnter?.Invoke(other.transform);
		}

		private void OnCollisionExit2D(Collision2D other) {
			if (this.TriggeredByPlayerOnly && !CGamePlayerManager.get.IsRootTransformFromAPlayerCharacter(other.transform)) return;
			this.CollisionExit?.Invoke(other.transform);
		}
	}
}

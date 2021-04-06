using UnityEngine;

namespace CDK {
	public abstract class CPhysicsMonoBehaviourTriggers : MonoBehaviour {
	
		[SerializeField] protected bool TriggeredByPlayerOnly = false;

		[SerializeField] protected CUnityEventTransform Enter;
		[SerializeField] protected CUnityEventTransform Exit;

		
		protected bool WillIgnoreCollision(Transform t) {
			return this.TriggeredByPlayerOnly && !CGamePlayerManager.get.IsTransformFromAPlayerCharacter(t.root);
		}

	}
}
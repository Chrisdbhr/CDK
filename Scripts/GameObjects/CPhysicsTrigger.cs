using UnityEngine;

namespace CDK {
	public class CPhysicsTrigger : MonoBehaviour {

		[SerializeField] private bool TriggeredByPlayerOnly = false;

		[SerializeField] private CUnityEventTransform TriggerEnter;
		[SerializeField] private CUnityEventTransform TriggerExit;




		private void OnTriggerEnter(Collider other) {
			if (this.TriggeredByPlayerOnly && !CGamePlayerManager.get.IsRootTransformFromAPlayerCharacter(other.transform)) return;
			this.TriggerEnter?.Invoke(other.transform);
		}

		private void OnTriggerEnter2D(Collider2D other) {
			if (this.TriggeredByPlayerOnly && !CGamePlayerManager.get.IsRootTransformFromAPlayerCharacter(other.transform)) return;
			this.TriggerEnter?.Invoke(other.transform);
		}

		private void OnTriggerExit(Collider other) {
			if (this.TriggeredByPlayerOnly && !CGamePlayerManager.get.IsRootTransformFromAPlayerCharacter(other.transform)) return;
			this.TriggerExit?.Invoke(other.transform);
		}

		private void OnTriggerExit2D(Collider2D other) {
			if (this.TriggeredByPlayerOnly && !CGamePlayerManager.get.IsRootTransformFromAPlayerCharacter(other.transform)) return;
			this.TriggerExit?.Invoke(other.transform);
		}
	}
}
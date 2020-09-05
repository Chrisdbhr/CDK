using UnityEngine;

namespace CDK {
	public class CPhysicsTrigger : MonoBehaviour {

		[SerializeField] private CUnityEventTransform TriggerEnter;
		[SerializeField] private CUnityEventTransform TriggerExit;

		
		

		private void OnTriggerEnter(Collider other) {
			this.TriggerEnter?.Invoke(other.transform);
		}

		private void OnTriggerEnter2D(Collider2D other) {
			this.TriggerEnter?.Invoke(other.transform);
		}

		private void OnTriggerExit(Collider other) {
			this.TriggerExit?.Invoke(other.transform);
		}

		private void OnTriggerExit2D(Collider2D other) {
			this.TriggerExit?.Invoke(other.transform);
		}
	}
}
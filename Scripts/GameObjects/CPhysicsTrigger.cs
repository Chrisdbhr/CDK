using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CPhysicsTrigger : MonoBehaviour {

		[SerializeField] private UnityEvent TriggerEnter;
		[SerializeField] private UnityEvent TriggerExit;


		private void OnTriggerEnter(Collider other) {
			this.TriggerEnter?.Invoke();
		}

		private void OnTriggerExit(Collider other) {
			this.TriggerExit?.Invoke();
		}
	}
}
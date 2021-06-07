using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CDisableTrigger : MonoBehaviour {
		[SerializeField] private UnityEvent TriggerEvent;
		
		private void OnDisable() {
			this.TriggerEvent?.Invoke();
		}
		
	}
}
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CDisableTrigger : MonoBehaviour {
		[SerializeField] UnityEvent TriggerEvent;

		void OnDisable() {
			TriggerEvent?.Invoke();
		}
		
	}
}
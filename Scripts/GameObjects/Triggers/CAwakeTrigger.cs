using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CAwakeTrigger : MonoBehaviour {
		[SerializeField] private UnityEvent TriggerEvent;
		
		private void Awake() {
			this.TriggerEvent?.Invoke();
		}
		
	}
}
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CEnableTrigger : MonoBehaviour {
		[SerializeField] private UnityEvent TriggerEvent;
		
		private void OnEnable() {
			this.TriggerEvent?.Invoke();
		}
		
	}
}
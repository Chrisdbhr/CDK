using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CAwakeTrigger : MonoBehaviour {
		[SerializeField] UnityEvent TriggerEvent;

		void Awake() {
			TriggerEvent?.Invoke();
		}
		
	}
}
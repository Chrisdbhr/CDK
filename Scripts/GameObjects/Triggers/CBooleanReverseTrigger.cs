using UnityEngine;

namespace CDK {
	public class CBooleanReverseTrigger : MonoBehaviour {

		[SerializeField] CUnityEventBool _reversedBoolEvent;
		
		
		public void TriggerReversed(bool value) {
			_reversedBoolEvent?.Invoke(!value);
		}
		
	}
}

using UnityEngine;

namespace CDK {
	public class CBooleanReverseTrigger : MonoBehaviour {

		[SerializeField] private CUnityEventBool _reversedBoolEvent;
		
		
		public void TriggerReversed(bool value) {
			this._reversedBoolEvent?.Invoke(!value);
		}
		
	}
}

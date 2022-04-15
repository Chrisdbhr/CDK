using UnityEngine;

namespace CDK.Lights {
	[RequireComponent(typeof(Collider))]
	public class CLightTriggerReceiver : MonoBehaviour {
		
		[SerializeField] private Light _light;
		[SerializeField] private CUnityEventBool _sensorTriggerIsNear;
		[SerializeField] private CUnityEventBool _sensorTriggerIsFar;
		
		
		
		
		public void TriggerIsNear() {
			_light.enabled = false;
			_sensorTriggerIsNear?.Invoke(true);
			_sensorTriggerIsFar?.Invoke(false);
		}

		public void TriggerIsFar() {
			_light.enabled = true;
			_sensorTriggerIsNear?.Invoke(false);
			_sensorTriggerIsFar?.Invoke(true);
		}
		
		
		
		private void Reset() {
            var col = this.GetComponent<Collider>();
            if (col) {
                col.isTrigger = true;
            }
            this.gameObject.layer = 1;
        }
	}
}

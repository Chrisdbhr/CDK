using CDK;
using UnityEngine;

namespace CDK.Lights {
	[RequireComponent(typeof(Collider))]
	public class CLightTriggerSensor : MonoBehaviour {
		private void OnTriggerEnter(Collider other) {
        	var lightTrigger = other.GetComponent<CLightTriggerReceiver>();
        	if (!lightTrigger) return;
        	lightTrigger.TriggerIsNear();
        }

        private void OnTriggerExit(Collider other) {
        	var lightTrigger = other.GetComponent<CLightTriggerReceiver>();
        	if (!lightTrigger) return;
        	lightTrigger.TriggerIsFar();
        }
                
        private void OnTriggerEnter2D(Collider2D other) {
            var lightTrigger = other.GetComponent<CLightTriggerReceiver>();
        	if (!lightTrigger) return;
        	lightTrigger.TriggerIsNear();
        }

        private void OnTriggerExit2D(Collider2D other) {
            var lightTrigger = other.GetComponent<CLightTriggerReceiver>();
        	if (!lightTrigger) return;
        	lightTrigger.TriggerIsFar();
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

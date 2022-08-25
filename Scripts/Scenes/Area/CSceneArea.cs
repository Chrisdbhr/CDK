using UnityEngine;

namespace CDK {
    [RequireComponent(typeof(Collider))]
	public class CSceneArea : MonoBehaviour {

        public CMovementSpeed MaximumMovementState => this._maximumMovementState;
        [SerializeField] private CMovementSpeed _maximumMovementState = CMovementSpeed.Walking;
		
        
        

		private void OnTriggerEnter(Collider other) {
			var receiver = other.GetComponent<CSceneAreaReceiver>();
			if (receiver == null) return;
            receiver.AddSceneArea(this);
		}

		private void OnTriggerExit(Collider other) {
			var receiver = other.GetComponent<CSceneAreaReceiver>();
			if (receiver == null) return;
            receiver.RemoveSceneArea(this);
		}


        private void Reset() {
            var col = this.GetComponent<Collider>();
            if (col) col.isTrigger = true;
        }
    }
}
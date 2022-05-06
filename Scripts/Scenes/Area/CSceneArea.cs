using UnityEngine;

namespace CDK {
    [RequireComponent(typeof(Collider))]
	public class CSceneArea : MonoBehaviour {

        public CMovState MaximumMovementState => this._maximumMovementState;
        [SerializeField] private CMovState _maximumMovementState = CMovState.Walking;
		
        
        

		private void OnTriggerEnter(Collider other) {
			var receiver = other.GetComponent<CCharacterBase>();
			if (receiver == null) return;
            receiver.AddSceneArea(this);
		}

		private void OnTriggerExit(Collider other) {
			var receiver = other.GetComponent<CCharacterBase>();
			if (receiver == null) return;
            receiver.RemoveSceneArea(this);
		}


        private void Reset() {
            var col = this.GetComponent<Collider>();
            if (col) col.isTrigger = true;
        }
    }
}
using UnityEngine;

namespace CDK {
    public class CCameraAreaTrigger : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>

        [SerializeField] private CPlayerCamera _playerCamera;
        
        #endregion <<---------- Properties and Fields ---------->>


        
        
        #region <<---------- Mono Behaviour ---------->>

        private void OnTriggerEnter(Collider other) {
            var cameraArea = other.GetComponent<CCameraProfileVolume>();
            if (cameraArea == null) return;
            this._playerCamera.EnteredCameraArea(cameraArea);
        }

        private void OnTriggerExit(Collider other) {
            var cameraArea = other.GetComponent<CCameraProfileVolume>();
            if (cameraArea == null) return;
            this._playerCamera.ExitedCameraArea(cameraArea);
        }

        #endregion <<---------- Mono Behaviour ---------->>

    }
}
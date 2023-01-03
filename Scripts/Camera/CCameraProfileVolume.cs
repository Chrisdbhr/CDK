using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public class CCameraProfileVolume : MonoBehaviour {

		[SerializeField] private bool _isGlobal;
		
		public CPlayerCamera.CameraType CameraType => _cameraType;
		[SerializeField] private CPlayerCamera.CameraType _cameraType;

        private CPlayerCamera _playerCamera;
        private Coroutine _waitingForCameraRoutine;
        
        
        
        private void OnEnable() {
            this._waitingForCameraRoutine = this.CStartCoroutine(this.WaitForCameraRoutine());
        }

        private void OnDisable() {
            this.CStopCoroutine(this._waitingForCameraRoutine);
            if (!this._playerCamera) return;
            this._playerCamera.ExitedCameraArea(this);
        }

        IEnumerator WaitForCameraRoutine() {
            if (!_isGlobal) yield break;
            while (_playerCamera == null) {
                this._playerCamera = FindObjectOfType<CPlayerCamera>();
                yield return null;
            }
            this._playerCamera.EnteredCameraArea(this);
        }


		#if UNITY_EDITOR
		void Reset() {
			Undo.RecordObject(this.gameObject, "Renamed object");
			this.name = "Camera Profile";
		}
		#endif
	}
}
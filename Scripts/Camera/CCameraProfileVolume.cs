using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public class CCameraProfileVolume : CPhysicsTrigger {

        #region <<---------- Properties and Fields ---------->>
        
        [SerializeField] private bool _isGlobal;
		
		public CPlayerCamera.CameraType CameraType => _cameraType;
		[SerializeField] private CPlayerCamera.CameraType _cameraType;

        private CPlayerCamera _playerCamera;
        private Coroutine _waitingForCameraRoutine;
        
        #endregion <<---------- Properties and Fields ---------->>

        
        
        
        #region <<---------- Mono Behaviour ---------->>
        
        private void OnEnable() {
            this._waitingForCameraRoutine = this.CStartCoroutine(this.WaitForCameraRoutine());
        }

        private void OnDisable() {
            this.CStopCoroutine(this._waitingForCameraRoutine);
            if (!this._playerCamera) return;
            this._playerCamera.ExitedCameraArea(this);
        }

        #if UNITY_EDITOR
        protected override void Reset() {
            base.Reset();
            Undo.RecordObject(this.gameObject, "Add Collider");
            if (!this.TryGetComponent<Collider>(out var c)) {
                var box = this.gameObject.AddComponent<BoxCollider>();
                box.size = new Vector3(10, 5, 10);
                box.center = new Vector3(0, 5, 0);
            }
            Undo.RecordObject(this.gameObject, "Renamed object");
            this.name = "Camera Profile";
        }
		#endif
        
        IEnumerator WaitForCameraRoutine() {
            if (!_isGlobal) yield break;
            while (_playerCamera == null) {
                this._playerCamera = FindObjectOfType<CPlayerCamera>();
                yield return null;
            }
            this._playerCamera.EnteredCameraArea(this);
        }
        
        #endregion <<---------- Mono Behaviour ---------->>
        
        
        
        
        

        


        #region <<---------- CPhysicsTrigger ---------->>
        protected override bool WillIgnoreTrigger(Component col) {
            return base.WillIgnoreTrigger(col) || _isGlobal;
        }

        protected override void StartedCollisionOrTrigger(Transform other) {
            base.StartedCollisionOrTrigger(other);
            var playerCamera = GetPlayerCameraFromTransform(other);
            if (playerCamera != null) {
                playerCamera.EnteredCameraArea(this);
            }
        }

        protected override void ExitedCollisionOrTrigger(Transform other) {
            base.ExitedCollisionOrTrigger(other);
            var playerCamera = GetPlayerCameraFromTransform(other);
            if (playerCamera != null) {
                playerCamera.ExitedCameraArea(this);
            }
        }
        
        #endregion <<---------- CPhysicsTrigger ---------->>

        
        

        #region <<---------- General ---------->>

        private CPlayerCamera GetPlayerCameraFromTransform(Transform t) {
            var player = CGamePlayerManager.get.GetPlayerFromTransform(t);
            return player == null ? null : player.GetCamera;
        }

        #endregion <<---------- General ---------->>
        
    }
}
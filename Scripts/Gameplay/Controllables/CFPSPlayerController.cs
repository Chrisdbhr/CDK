using System;
using UniRx;
using UnityEngine;

namespace CDK {
	public class CFPSPlayerController : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
		// references
		[SerializeField] private CCharacterBase _characterBase;
		[SerializeField] private Camera _playerCamera;
		
		
		[NonSerialized] private Quaternion _targetLookRotation;
		[NonSerialized] private Vector2 _movementInputDir;
		[NonSerialized] private Vector3 camF = Vector3.forward;
		[NonSerialized] private Vector3 camR = Vector3.right;
		
		#endregion <<---------- Properties and Fields ---------->>

		

		
		#region <<---------- MonoBehaviour ---------->>

		private void Awake() {
			this._characterBase = this.GetComponent<CCharacterBase>();
			if (this._characterBase == null) {
				Debug.LogError($"Cant find any Character on {this.name}, removing component, character will not be controllable.");
				Destroy(this);
			}
		}
		
		private void Update() {

			if (CBlockingEventsManager.get.IsBlockingEventHappening) return;
			
			// input movement
			this._movementInputDir = new Vector2(Input.GetAxisRaw(CInputKeys.MOV_X), Input.GetAxisRaw(CInputKeys.MOV_Y));
			this._characterBase.InputMovementDirRelativeToCam = this.camF * this._movementInputDir.y + this.camR * this._movementInputDir.x;
			
			// input crouch
			if (Input.GetButtonDown(CInputKeys.CROUCH)) {
				this._characterBase.ToggleCrouch();
			}

			// input walk
			this._characterBase.InputSlowWalk = this._characterBase.IsCrouched || !Input.GetButton(CInputKeys.SLOW_WALK);
			
			// aim _playerCamera
			bool inputAim = Input.GetButton(CInputKeys.AIM);
			this._characterBase.InputAim = inputAim;

			var aimCamera = this._playerCamera.GetComponent<CAim>();
			if (aimCamera != null) {
				aimCamera.IsAiming = inputAim;
			}
			this.ProcessMovementInputDirection();
		}

		#endregion <<---------- MonoBehaviour ---------->>

		private void ProcessMovementInputDirection() {
			var camTransform = this._playerCamera.transform;
			this.camF = camTransform.forward;
			this.camR = camTransform.right;
			this.camF.y = this.camR.y = 0f;
			this.camF.Normalize();
			this.camR.Normalize();

			// absolute input
			this._characterBase.InputMovementDirAbsolute = this._movementInputDir;
			
			// relative to camera input
			this._characterBase.InputMovementDirRelativeToCam = this.camF * this._movementInputDir.y + this.camR * this._movementInputDir.x;
		}
		
	}
}

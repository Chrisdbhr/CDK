using System;
using UnityEngine;

namespace CDK {
	[RequireComponent(typeof(CCharacterBase))]
	public class CCharacterControllerBase : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>
		[SerializeField] private Camera _playerCamera;

		[NonSerialized] private float _yCheckOffset = 0.5f;
		[NonSerialized] private Transform _cameraTransform;
		[NonSerialized] private Quaternion _targetLookRotation;
		[NonSerialized] private Vector2 _inputDir;
		[NonSerialized] private Vector3 camF = Vector3.forward;
		[NonSerialized] private Vector3 camR = Vector3.right;
		[NonSerialized] private const float INTERACT_SPHERE_CHECK_MULTIPLIER = 0.75f;

		// cache
		[NonSerialized] private CCharacterBase _characterBase;
		[NonSerialized] private Transform _myTransform;
		#endregion <<---------- Properties and Fields ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>
		private void Awake() {
			this._myTransform = this.transform;
			this._characterBase = this.GetComponent<CCharacterBase>();
			if (this._characterBase == null) {
				Debug.LogError($"Cant find any Character on {this.name}, removing component, character will not be controllable.");
				Destroy(this);
			}
		}

		private void Update() {
			if (CBlockingEventsManager.get.IsBlockingEventHappening) return;

			// input movement
			this._inputDir = new Vector2(Input.GetAxisRaw(CInputKeys.MOV_X), Input.GetAxisRaw(CInputKeys.MOV_Y));
			this._characterBase.InputMovementDirRelativeToCam = this.camF * this._inputDir.y + this.camR * this._inputDir.x;

			// input walk
			this._characterBase.InputSlowWalk = Input.GetButton(CInputKeys.SLOW_WALK);

			// aim _playerCamera
			bool inputAim = Input.GetButton(CInputKeys.AIM);
			this._characterBase.InputAim = inputAim;

			if (this._playerCamera == null) return;

			var aimCamera = this._playerCamera.GetComponent<CAim>();
			if (aimCamera != null) {
				aimCamera.IsAiming = inputAim;
			}
			this.ProcessInputDirection();
		}
		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		private void ProcessInputDirection() {
			var camTransform = this._playerCamera.transform;
			this.camF = camTransform.forward;
			this.camR = camTransform.right;
			this.camF.y = this.camR.y = 0f;
			this.camF.Normalize();
			this.camR.Normalize();

			// absolute input
			this._characterBase.InputMovementDirAbsolute = this._inputDir;

			// relative to camera input
			this._characterBase.InputMovementDirRelativeToCam = this.camF * this._inputDir.y + this.camR * this._inputDir.x;
		}
	}
}

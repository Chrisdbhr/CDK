using System;
using UniRx;
using UnityEngine;

namespace CDK {
	public class CFPSPlayerController : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
		// references
		[SerializeField] private CCharacterBase _characterBase;
		[SerializeField] private Camera _playerCamera;

		[SerializeField] private LayerMask _interactionLayerMask;
		[SerializeField] private float _interactionMaxDistance = 1.0f;

		[NonSerialized] private float _interactionCapsuleCheckRadius = 0.10f;
		[NonSerialized] private Transform _myTransform;
		[NonSerialized] private Transform _cameraTransform;
		[NonSerialized] private Quaternion _targetLookRotation;
		[NonSerialized] private Vector2 _movementInputDir;
		[NonSerialized] private Vector3 camF = Vector3.forward;
		[NonSerialized] private Vector3 camR = Vector3.right;

		[NonSerialized] private ReactiveProperty<CInteractableObject> _currentInteractable;
		
		#endregion <<---------- Properties and Fields ---------->>

		

		
		#region <<---------- MonoBehaviour ---------->>

		private void Awake() {
			this._myTransform = this.transform;
			this._cameraTransform = this._playerCamera.transform;
			
			this._characterBase = this.GetComponent<CCharacterBase>();
			if (this._characterBase == null) {
				Debug.LogError($"Cant find any Character on {this.name}, removing component, character will not be controllable.");
				Destroy(this);
			}
		}

		private void OnEnable() {
			this._currentInteractable?.Dispose();
			this._currentInteractable = new ReactiveProperty<CInteractableObject>();
			this._currentInteractable.TakeUntilDisable(this).Subscribe(newInteractable => {
				if (!newInteractable) return;
				newInteractable.OnLookTo(this._myTransform);
			});
			
			
		}


		private void Update() {

			this.ProcessLookingInteractable();

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
			
			// input interaction
			bool inputDownInteract = Input.GetButtonDown(CInputKeys.INTERACT);
			if (inputDownInteract) {
				this.TryToInteract();	
			}
			
			// aim _playerCamera
			bool inputAim = Input.GetButton(CInputKeys.AIM);
			this._characterBase.InputAim = inputAim;

			var aimCamera = this._playerCamera.GetComponent<CAim>();
			if (aimCamera != null) {
				aimCamera.IsAiming = inputAim;
			}
			this.ProcessMovementInputDirection();
		}

		#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			this._myTransform = this.transform;
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(
				this._playerCamera.transform.position, 
				this._interactionCapsuleCheckRadius);
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>


		private void ProcessLookingInteractable() {
			var originPos = this._cameraTransform.position;
			var direction = this._cameraTransform.forward.normalized;

			var collided = Physics.SphereCast(
				originPos,
				this._interactionCapsuleCheckRadius,
				direction,
				out var raycastHit,
				this._interactionMaxDistance,
				this._interactionLayerMask,
				QueryTriggerInteraction.Collide
			);
			if (!collided) return;
			
			this._currentInteractable.Value = raycastHit.transform.GetComponent<CInteractableObject>();
		}

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

		private void TryToInteract() {
			if (!this._currentInteractable.Value) return;
			this._currentInteractable.Value.OnInteract(this._characterBase.transform);
		}
		
	}
}

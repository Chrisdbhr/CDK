using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDK {
	public class CPlayerController : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private Camera _playerCamera;
		[SerializeField] private LayerMask _interactionLayerMask;

		[NonSerialized] private CCharacterBase _characterBase;
		[NonSerialized] private float _interactionSphereCheckRadius = 0.75f;
		[NonSerialized] private float _yCheckOffset = 0.5f;
		[NonSerialized] private Transform _myTransform;
		[NonSerialized] private Transform _cameraTransform;
		[NonSerialized] private Quaternion _targetLookRotation;
		[NonSerialized] private Vector2 _inputDir;
		[NonSerialized] private Vector3 camF = Vector3.forward;
		[NonSerialized] private Vector3 camR = Vector3.right;
		[NonSerialized] private const float INTERACT_SPHERE_CHECK_MULTIPLIER = 0.75f;
		
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
			
			// input interaction
			bool inputDownInteract = Input.GetButtonDown(CInputKeys.INTERACT);
			if (inputDownInteract) {
				this.TryToInteract();	
			}
			
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

		#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			this._myTransform = this.transform;
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(
				this.GetCenterSphereCheckPosition(), 
				this._interactionSphereCheckRadius);
		}
		#endif
		
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

		private void TryToInteract() {
			var originPos = this.GetCenterSphereCheckPosition();

			var colliders = Physics.OverlapSphere(
				originPos,
				this._interactionSphereCheckRadius,
				this._interactionLayerMask,
				QueryTriggerInteraction.Collide
			);
			
			if (colliders == null || colliders.Length <= 0) return;
			
			// get list of interactables
			var interactableColliders = new List<Collider>();
			foreach (var col in colliders) {
				var interactable = col.GetComponent<CIInteractable>();
				if (interactable == null) continue;
				interactableColliders.Add(col);
			}
			
			if (interactableColliders.Count <= 0) return;
			
			originPos.x = this._myTransform.position.x;
			originPos.z = this._myTransform.position.z;
			
			// get closest interactable collider index
			int closestColliderIndex = 0;
			if (interactableColliders.Count == 1) closestColliderIndex = 0;
			else {
				float closestDistance = this._interactionSphereCheckRadius * 2f;
				for (int i = 0; i < interactableColliders.Count; i++) {
					float distance = (originPos - interactableColliders[i].transform.position).sqrMagnitude;
					if (distance >= closestDistance) continue;
					closestDistance = distance;
					closestColliderIndex = i;
				}
			}

			var direction = interactableColliders[closestColliderIndex].transform.position - originPos;
			bool hasSomethingBlockingLineOfSight = Physics.Raycast(
				originPos,
				direction,
				direction.magnitude,
				CGameSettings.get.LineOfSightBlockingLayers,
				QueryTriggerInteraction.Collide
			);

			if (hasSomethingBlockingLineOfSight) return;
				
			var choosenInteractable = interactableColliders[closestColliderIndex].GetComponent<CIInteractable>();
			choosenInteractable.OnInteract(this._characterBase.transform);
			
		}
		
		private Vector3 GetCenterSphereCheckPosition() {
			return this._myTransform.position + this._myTransform.forward * (this._interactionSphereCheckRadius * INTERACT_SPHERE_CHECK_MULTIPLIER) + (Vector3.up * this._yCheckOffset);
		}

	}
}

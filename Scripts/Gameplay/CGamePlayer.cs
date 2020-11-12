using System.Collections.Generic;
using GameInput;
using UnityEngine;

namespace CDK {
	public class CGamePlayer {

		#region <<---------- Initializers ---------->>

		public CGamePlayer(int playerNumber) {
			this.PlayerNumber = playerNumber;

			// movement
			this._playerInputActions.gameplay.move.performed += context => {
				var inputMove = context.ReadValue<Vector2>();

				this._controllingCharacter.InputMovementRaw = inputMove;
				
				var camF = Vector3.forward;
				var camR = Vector3.right;
				if (this._playerCamera != null) {
					var camTransform = this._playerCamera.transform;
					camF = camTransform.forward;
					camF.y = 0;
					camF = camF.normalized;
					camR = camTransform.right;
					camR.y = 0;
					camR = camR.normalized;
				}
				
				this._controllingCharacter.InputMovementDirRelativeToCam = camF * inputMove.y + camR * inputMove.x;
			};
			
			// look
			this._playerInputActions.gameplay.look.performed += context => {
				var inputLook = context.ReadValue<Vector2>();
				this._playerCamera.Rotate(inputLook);
			};

			Debug.Log($"Instantiating game player {playerNumber}");
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		
		
		#region <<---------- Properties and Fields ---------->>
		
		public int PlayerNumber { get; } = 0;

		private DefaultPlayerInputActions _playerInputActions = new DefaultPlayerInputActions();
		
		private CPlayerCamera _playerCamera;
		private CCharacterBase _controllingCharacter;

		#endregion <<---------- Properties and Fields ---------->>

		
		
		
		private void Update() {

			if (CBlockingEventsManager.get.IsBlockingEventHappening) return;
			
			
			// input walk
			this._controllingCharacter.InputRun = Input.GetButton(CInputKeys.SLOW_WALK);
			
			// // input interaction
			// bool inputDownInteract = Input.GetButtonDown(CInputKeys.INTERACT);
			// if (inputDownInteract) {
			// 	this.TryToInteract();	
			// }
			
		}
		
		/*
		private void TryToInteract() {
			var originPos = Vector3.zero;

			var colliders = Physics.OverlapSphere(
				originPos,
				this._interactionSphereCheckRadius,
				1,
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
			
			// originPos.x = this._myTransform.position.x;
			// originPos.z = this._myTransform.position.z;
			//
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
			choosenInteractable.OnInteract(this._controllingCharacter.transform);
			
		}
		
		private Vector3 GetCenterSphereCheckPosition() {
			return this._myTransform.position + this._myTransform.forward * (this._interactionSphereCheckRadius * INTERACT_SPHERE_CHECK_MULTIPLIER) + (Vector3.up * this._yCheckOffset);
		}
		*/

	}
}

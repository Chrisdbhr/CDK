using System.Collections.Generic;
using System.Linq;
using GameInput;
using UnityEngine;

namespace CDK {
	public class CGamePlayer {

		#region <<---------- Initializers ---------->>

		public CGamePlayer(int playerNumber) {
			this.PlayerNumber = playerNumber;
			this.SignToInputEvents();
			Debug.Log($"Instantiating game player {playerNumber}");
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		
		
		#region <<---------- Properties and Fields ---------->>
		
		public int PlayerNumber { get; } = 0;
		public CPlayerCamera PlayerCamera { get; private set; }

		private readonly DefaultPlayerInputActions _playerInputActions = new DefaultPlayerInputActions();

		private List<CCharacterBase> _controllingCharacter = new List<CCharacterBase>();

		#endregion <<---------- Properties and Fields ---------->>


		private void SignToInputEvents() {
			// movement
			this._playerInputActions.gameplay.move.performed += context => {
				if (CBlockingEventsManager.IsBlockingEventHappening) return;
				if (this._controllingCharacter == null) return;
				
				// input movement raw
				var inputMove = context.ReadValue<Vector2>();
				
				// input relative to cam direction
				var camF = Vector3.forward;
				var camR = Vector3.right;
				if (this.PlayerCamera != null) {
					var camTransform = this.PlayerCamera.transform;
					camF = camTransform.forward;
					camF.y = 0;
					camF = camF.normalized;
					camR = camTransform.right;
					camR.y = 0;
					camR = camR.normalized;
				}
				
				foreach (var character in this._controllingCharacter.Where(character => character != null)) {
					character.InputMovementRaw = inputMove;
					character.InputMovementDirRelativeToCam = camF * inputMove.y + camR * inputMove.x;
				}
			};
			
			// look
			this._playerInputActions.gameplay.look.performed += context => {
				if (CBlockingEventsManager.IsBlockingEventHappening) return;
				if (this._controllingCharacter == null) return;
				
				var inputLook = context.ReadValue<Vector2>();
				this.PlayerCamera.Rotate(inputLook);
			};
			
			// run
			this._playerInputActions.gameplay.run.performed += context => {
				if (CBlockingEventsManager.IsBlockingEventHappening) return;
				if (this._controllingCharacter == null) return;
				
				var inputRun = context.ReadValueAsButton();

				foreach (var character in this._controllingCharacter.Where(character => character != null)) {
					character.InputRun = inputRun;
				}
			};
		}


		#region <<---------- Character Control ---------->>

		public void AddControllingCharacter(CCharacterBase character) {
			if (this._controllingCharacter.Contains(character)) {
				Debug.LogError($"Will not add {character.name} to player {this.PlayerNumber} control because it is already controlling it!");
				return;
			}

			Object.Instantiate(character.gameObject);
			
			Debug.Log("TODO create character at a teleport point.");

			this._controllingCharacter.Add(character);
		}

		public CCharacterBase GetMainCharacter() {
			return this._controllingCharacter.Count > 0 ? this._controllingCharacter[0] : null;
		}
		
		#endregion <<---------- Character Control ---------->>


		
		
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

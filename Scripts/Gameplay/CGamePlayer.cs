using System;
using System.Collections.Generic;
using System.Linq;
using GameInput;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace CDK {
	public class CGamePlayer : IDisposable {

		#region <<---------- Initializers ---------->>

		public CGamePlayer(int playerNumber) {

			this._compositeDisposable?.Dispose();
			this._compositeDisposable = new CompositeDisposable();
			
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

		[NonSerialized] private CompositeDisposable _compositeDisposable;
		
		
		#endregion <<---------- Properties and Fields ---------->>

		
		

		#region <<---------- Events ---------->>
		
		private void SignToInputEvents() {
			
			this._playerInputActions.Enable();
			
			// movement
			Observable.EveryUpdate().Subscribe(_ => {
				if (this._controllingCharacter.Count <= 0) return;
				
				// input movement raw
				var inputMove = this._playerInputActions.gameplay.move.ReadValue<Vector2>();
				
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
					character.InputMovementRaw = inputMove.normalized;
					character.InputMovementDirRelativeToCam = (camF * inputMove.y + camR * inputMove.x).normalized;
				}
			}).AddTo(this._compositeDisposable);
			
			// look
			Observable.EveryLateUpdate().Subscribe(context => {
				var inputLook = this._playerInputActions.gameplay.look.ReadValue<Vector2>();
				if (inputLook == Vector2.zero) return;
				if (this.PlayerCamera == null) return;
				this.PlayerCamera.Rotate(inputLook);
			}).AddTo(this._compositeDisposable);
			
			// run
			this._playerInputActions.gameplay.run.started += this.InputRunOnCanceled;
			this._playerInputActions.gameplay.run.canceled += this.InputRunOnCanceled;

		}

		private void UnsignFromEvents() {
			// run
			this._playerInputActions.gameplay.run.started -= this.InputRunOnCanceled;
			this._playerInputActions.gameplay.run.canceled -= this.InputRunOnCanceled;

		}

		#endregion <<---------- Events ---------->>



		#region <<---------- Input ---------->>
		
		private void InputRunOnCanceled(InputAction.CallbackContext context) {
			if (this._controllingCharacter.Count <= 0) return;
			var inputRun = context.ReadValueAsButton();
			foreach (var character in this._controllingCharacter.Where(character => character != null)) {
				character.InputRun = inputRun;
			}
		}
		
		
		#endregion <<---------- Input ---------->>
		
		
		
		#region <<---------- Character Control ---------->>

		public void AddControllingCharacter(CCharacterBase character) {
			if (this._controllingCharacter.Contains(character)) {
				Debug.LogError($"Will not add {character.name} to player {this.PlayerNumber} control because it is already controlling it!");
				return;
			}

			CSceneManager.PositionTransformToSceneEntryPoint(character.transform);
			
			this._controllingCharacter.Add(character);
			
			this.CheckIfNeedToCreateCamera();
		}

		public CCharacterBase GetMainControllingCharacter() {
			return this._controllingCharacter.FirstOrDefault(c => c != null);
		}

		private bool HasAnyCharacterToControl() {
			return this._controllingCharacter.Count > 0 && this._controllingCharacter.FirstOrDefault(c => c != null) != null;
		}
		
		#endregion <<---------- Character Control ---------->>




		#region <<---------- Player Camera ---------->>

		private void CheckIfNeedToCreateCamera() {
			if (this.PlayerCamera != null) return;
			var mainChar = this.GetMainControllingCharacter();
			if (mainChar == null) return;

			var createdGo = new GameObject($"[Camera] {mainChar.name}");
			Debug.Log($"Created {mainChar.name} Camera", createdGo);
			this.PlayerCamera = createdGo.AddComponent<CPlayerCamera>();
			this.PlayerCamera.Initialze(this);
		}
		
		#endregion <<---------- Player Camera ---------->>

		


		#region <<---------- Disposable ---------->>
		
		public void Dispose() {
			this._playerInputActions?.Dispose();
			this._compositeDisposable?.Dispose();
			
			this.UnsignFromEvents();
		}
		
		#endregion <<---------- Disposable ---------->>

		
		
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

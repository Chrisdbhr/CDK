using System;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace CDK {
	public class CGamePlayer : IDisposable {

		#region <<---------- Initializers ---------->>

		public CGamePlayer(int playerNumber) {

			this._compositeDisposable?.Dispose();
			this._compositeDisposable = new CompositeDisposable();
			
			this.PlayerNumber = playerNumber;
			this.SignToInputEvents();
			Debug.Log($"Instantiating a new game player number {playerNumber}");
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		
		
		#region <<---------- Properties and Fields ---------->>
		
		public int PlayerNumber { get; } = 0;
		private Rewired.Player _rePlayer;
		private Transform _cameraTransform { get; set; }

		private readonly List<CCharacterBase> _characters = new List<CCharacterBase>();

		[NonSerialized] private CompositeDisposable _compositeDisposable;
		[NonSerialized] private Vector2 _inputMovement;
		
		
		#endregion <<---------- Properties and Fields ---------->>

		
		

		#region <<---------- Events ---------->>
		
		private void SignToInputEvents() {

			this._rePlayer = ReInput.players.GetPlayer(this.PlayerNumber);

			this._rePlayer.AddInputEventDelegate(data => { this._inputMovement.x = data.GetAxis(); }, UpdateLoopType.Update, InputActionEventType.AxisActiveOrJustInactive, CInputKeys.MOV_X);
			this._rePlayer.AddInputEventDelegate(data => { this._inputMovement.y = data.GetAxis(); }, UpdateLoopType.Update, InputActionEventType.AxisActiveOrJustInactive, CInputKeys.MOV_Y);
			
			// movement
			Observable.EveryUpdate().Subscribe(_ => {
				if (this._characters.Count <= 0) return;
				
				// input relative to cam direction
				var camF = Vector3.forward;
				var camR = Vector3.right;
				if (this._cameraTransform != null) {
					camF = this._cameraTransform.forward;
					camF.y = 0;
					camF = camF.normalized;
					camR = this._cameraTransform.right;
					camR.y = 0;
					camR = camR.normalized;
				}
				
				foreach (var character in this._characters.Where(character => character != null)) {
					character.InputMovementRaw = this._inputMovement.normalized;
					character.InputMovementDirRelativeToCam = (camF * this._inputMovement.y + camR * this._inputMovement.x).normalized;
				}
			}).AddTo(this._compositeDisposable);
			
			this._rePlayer.AddInputEventDelegate(this.InputInteract, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, CInputKeys.INTERACT);
			this._rePlayer.AddInputEventDelegate(this.InputRun, UpdateLoopType.Update, CInputKeys.RUN);

		}

		private void UnsignFromInputEvents() {
			this._rePlayer?.RemoveInputEventDelegate(this.InputInteract);
			this._rePlayer?.RemoveInputEventDelegate(this.InputRun);
		}

		#endregion <<---------- Events ---------->>


		

		#region <<---------- Input ---------->>
		
		private void InputRun(InputActionEventData data) {
			var character = this.GetControllingCharacter();
			if (character == null) return;
			var inputRun = data.GetButton();
			character.InputRun = inputRun;
		}
		
		private void InputInteract(InputActionEventData data) {
			var character = this.GetControllingCharacter();
			if (character == null) return;
			var interactionComponent = character.GetComponent<CPlayerInteractionBase>();
			if (interactionComponent == null) return;
			interactionComponent.TryToInteract();
		}
		
		
		#endregion <<---------- Input ---------->>
		
		
		
		
		#region <<---------- Character Control ---------->>

		public void AddControllingCharacter(CCharacterBase character) {
			if (this._characters.Contains(character)) {
				Debug.LogError($"Will not add {character.name} to player {this.PlayerNumber} control because it is already controlling it!");
				return;
			}

			CSceneManager.SetTransformToSceneEntryPoint(character.transform);
			
			this._characters.Add(character);
			
			this.CheckIfNeedToCreateCamera();
		}

		public CCharacterBase GetControllingCharacter() {
			return this._characters.FirstOrDefault(c => c != null);
		}

		public List<GameObject> GetAllRelatedGameObjects() {
			var list = this._characters.Select(characterBase => characterBase.gameObject).ToList();
			if(this._cameraTransform != null) list.Add(this._cameraTransform.root.gameObject);
			return list;
		}

		public bool IsControllingCharacter(CCharacterBase characterBase) {
			return this._characters.Contains(characterBase);
		}

		#endregion <<---------- Character Control ---------->>




		#region <<---------- Player Camera ---------->>

		private void CheckIfNeedToCreateCamera() {
			if (this._cameraTransform != null) return;
			var mainChar = this.GetControllingCharacter();
			if (mainChar == null) return;

			Addressables.LoadAssetAsync<GameObject>("PlayerCamera").Completed += handle => {
				
				var createdGo = Object.Instantiate(handle.Result);
				createdGo.name = $"[Camera] {mainChar.name}";
				
				Debug.Log($"Created {mainChar.name} Camera", createdGo);

				var cPlayerCameraManager = createdGo.GetComponent<CPlayerCamera>();
				cPlayerCameraManager.Initialze(this);
				this._cameraTransform = cPlayerCameraManager.GetCameraTransform();
			};
			
		}
		
		#endregion <<---------- Player Camera ---------->>

		


		#region <<---------- Disposable ---------->>
		
		public void Dispose() {
			this._compositeDisposable?.Dispose();
			
			this.UnsignFromInputEvents();
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

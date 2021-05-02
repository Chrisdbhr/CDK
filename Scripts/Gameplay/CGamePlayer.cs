using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CDK.UI;
using Rewired;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public class CGamePlayer : IDisposable {

		#region <<---------- Initializers ---------->>

		public CGamePlayer(int playerNumber) {

			this._compositeDisposable?.Dispose();
			this._compositeDisposable = new CompositeDisposable();
			
			this.PlayerNumber = playerNumber;
			
			this._rePlayer = ReInput.players.GetPlayer(this.PlayerNumber);
			
			this.SignToInputEvents();

			this.SetInputLayout(false);
			CBlockingEventsManager.OnMenu += this.SetInputLayout;
		
			#if !UNITY_EDITOR
			Application.focusChanged += async focused => {
				if(!focused) await this.OpenMenu();
			};
			#endif
			
			Debug.Log($"Instantiating a new game player number {playerNumber}");
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		
		
		#region <<---------- Properties and Fields ---------->>
		
		public int PlayerNumber { get; } = 0;
		private readonly Rewired.Player _rePlayer;
		private Transform _cameraTransform;
		private CPlayerCamera _cPlayerCamera;

		private readonly List<CCharacterBase> _characters = new List<CCharacterBase>();

		[NonSerialized] private readonly CompositeDisposable _compositeDisposable;
		
		
		#endregion <<---------- Properties and Fields ---------->>

		
		

		#region <<---------- Events ---------->>
		
		private void SignToInputEvents() {
			
			// movement
			Observable.EveryUpdate().Subscribe(_ => {
				if (this._characters.Count <= 0) return;

				var inputMovement = this._rePlayer.GetAxis2D(CInputKeys.MOV_X, CInputKeys.MOV_Y);
				
				var character = this.GetControllingCharacter();
				if (character == null) return;
				var (camF, camR) = this.GetCameraVectors(character);
				
				character.InputMovementRaw = inputMovement.normalized;
				character.InputMovementDirRelativeToCam = (camF * inputMovement.y + camR * inputMovement.x).normalized;
			}).AddTo(this._compositeDisposable);
			
			this._rePlayer.AddInputEventDelegate(this.InputInteract, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, CInputKeys.INTERACT);
			this._rePlayer.AddInputEventDelegate(this.InputRun, UpdateLoopType.Update, CInputKeys.RUN);
			this._rePlayer.AddInputEventDelegate(this.InputResetCameraRotation, UpdateLoopType.Update, CInputKeys.RESET_CAM_ROTATION);
			this._rePlayer.AddInputEventDelegate(this.InputPause, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, CInputKeys.MENU_PAUSE);

		}

		private void UnsignFromInputEvents() {
			this._rePlayer?.RemoveInputEventDelegate(this.InputInteract);
			this._rePlayer?.RemoveInputEventDelegate(this.InputRun);
			this._rePlayer?.RemoveInputEventDelegate(this.InputResetCameraRotation);
			this._rePlayer?.RemoveInputEventDelegate(this.InputPause);
		}

		#endregion <<---------- Events ---------->>
		
		
		
		
		#region <<---------- Character Creation and Exclusion ---------->>
		
		public async Task InstantiateAndAssignCharacter(AssetReference charToCreate) {
			
			if (charToCreate == null || !charToCreate.RuntimeKeyIsValid()) {
				Debug.LogWarning($"Created player {this.PlayerNumber} with no controlling character.");
				return;
			}

			var createdGo = await CAssets.LoadAndInstantiateGameObjectAsync(charToCreate.RuntimeKey.ToString());
			if (createdGo == null) {
				Debug.LogWarning($"Player {this.PlayerNumber} cant find character '{charToCreate}' to control.");
				return;
			}

			var entryPoints = GameObject.FindObjectsOfType<CSceneEntryPoint>();
			if (entryPoints.Length > 0) {
				var entryPoint = entryPoints.OrderBy(x=>x.Number).FirstOrDefault();
				if (entryPoint != null) {
					createdGo.transform.position = entryPoint.transform.position;
				}
			}
			createdGo.transform.Translate(0f,0.001f,0f); // prevent spawning at 0 position so engine does not think it is maybe inside the ground next frame.
			createdGo.SetActive(false);
			createdGo.name = $"[Character] {charToCreate}";
			var character = createdGo.GetComponent<CCharacterBase>();

			if (character == null) {
				Debug.LogError($"{charToCreate} gameobject doesnt have a {nameof(CCharacterBase)} component on it! could not create player!");
				return;
			}
				
			await this.AddControllingCharacter(character);
				
			Debug.Log($"Created player {this.PlayerNumber} controlling character '{charToCreate}'.");
		}

		#endregion <<---------- Character Creation and Exclusion ---------->>

		
		
		
		#region <<---------- Character Control ---------->>

		public async Task AddControllingCharacter(CCharacterBase character) {
			if (this._characters.Contains(character)) {
				Debug.LogError($"Will not add {character.name} to player {this.PlayerNumber} control because it is already controlling it!");
				return;
			}

			CSceneManager.SetTransformToSceneEntryPoint(character.transform);
			
			this._characters.Add(character);
			
			await this.CheckIfNeedToCreateCamera();
			
			character.gameObject.SetActive(true);

			#if UNITY_EDITOR
			Selection.activeGameObject = character.gameObject;
			#endif
		}

		public async Task RemoveAllControllingCharacters() {
			this._characters.Clear();
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

		private async Task CheckIfNeedToCreateCamera() {
			if (this._cameraTransform != null) return;
			var mainChar = this.GetControllingCharacter();
			if (mainChar == null) return;
			
			var createdGo = await CAssets.LoadAndInstantiateGameObjectAsync("PlayerCamera");
			createdGo.name = $"[Camera] {mainChar.name}";
			
			Debug.Log($"Created {mainChar.name} Camera", createdGo);

			this._cPlayerCamera = createdGo.GetComponent<CPlayerCamera>();
			this._cPlayerCamera.Initialize(this);
			this._cameraTransform = this._cPlayerCamera.GetCameraTransform();
		}

		private (Vector3 camF, Vector3 camR) GetCameraVectors(CCharacterBase relativeTo) {
			var (camF, camR) = (Vector3.forward, Vector3.right);
			if (this._cameraTransform == null) return (camF, camR);

			if (relativeTo == null) {
				camF = this._cameraTransform.forward;
				camF.y = 0;
				camF.Normalize();
			
				camR = this._cameraTransform.right;
				camR.y = 0;
				camR.Normalize();
				
				return (camF, camR);
			}

			camF = relativeTo.Position - this._cameraTransform.position;
			camF.y = 0;
			camF.Normalize();

			camR = -Vector3.Cross(camF, Vector3.up);
			camR.y = 0;
			camR.Normalize();
			
			return (camF, camR);
		}
		
		#endregion <<---------- Player Camera ---------->>

		

		
		#region <<---------- Input ---------->>

		private void InputPause(InputActionEventData data) {
			if (!data.GetButtonDown()) return;

			if (Time.timeScale <= 0) return;
			
			if (!CBlockingEventsManager.IsOnMenu) {
				this.OpenMenu().CAwait();
			}
		}

		private void InputResetCameraRotation(InputActionEventData data) {
			if (!data.GetButtonDown()) return;
			if (this._cPlayerCamera == null) return;
			this._cPlayerCamera.ResetRotation();
		}
		
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




		#region <<---------- Controlls Mappings ---------->>

		private void SetInputLayout(bool onMenu) {
			this._rePlayer.controllers.maps.SetMapsEnabled(!onMenu, "Default");
			this._rePlayer.controllers.maps.SetMapsEnabled(onMenu, "UI"); 
			//ReInput.players.GetSystemPlayer().controllers.maps.SetMapsEnabled(onMenu, "UI");
			
			Debug.Log($"Player {this.PlayerNumber} controllers maps onMenu changed to {onMenu}");
		}
		
		#endregion <<---------- Controlls Mappings ---------->>
		
		


		#region <<---------- Pause Menu ---------->>
		
		private async Task OpenMenu() {
			if (CBlockingEventsManager.IsOnMenu) return;
			CBlockingEventsManager.IsOnMenu = true;
			try {
				await CUINavigation.get.OpenMenu(CGameSettings.AssetRef_PauseMenu, null, null);
			} catch (Exception e) {
				CBlockingEventsManager.IsOnMenu = false;
				Debug.LogError("Exception trying to OpenMenu on GamePlayer: " + e);
			}
		}
		
		#endregion <<---------- Pause Menu ---------->>
		

		

		#region <<---------- Disposable ---------->>
		
		public void Dispose() {
			this._compositeDisposable?.Dispose();
			
			this.UnsignFromInputEvents();
			CBlockingEventsManager.OnMenu -= this.SetInputLayout; 
		}
		
		#endregion <<---------- Disposable ---------->>

	}
}

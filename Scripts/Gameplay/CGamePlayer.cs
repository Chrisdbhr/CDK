using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CDK.UI;
using UniRx;
using UnityEngine;

#if Rewired
using Rewired;
#endif

#if UnityAddressables
using UnityEngine.AddressableAssets;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public class CGamePlayer : IDisposable {

		#region <<---------- Initializers ---------->>

		public CGamePlayer(int playerNumber) {

			this._gameSettings = CDependencyResolver.Get<CGameSettings>();
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
            this._navigationManager = CDependencyResolver.Get<CUINavigationManager>();

			this._compositeDisposable?.Dispose();
			this._compositeDisposable = new CompositeDisposable();

			this.PlayerNumber = playerNumber;
			
			#if Rewired	
			this._rePlayer = ReInput.players.GetPlayer(this.PlayerNumber);
			#endif

			this.SignToInputEvents();
			
			#if Rewired
			SetInputLayout(this._rePlayer, false);
			this._blockingEventsManager.OnMenu += this.SetInputLayout;
			#endif

			if (!Application.isEditor) {
				Application.focusChanged += async focused => {
					if(!focused) await this.OpenMenu();
				};
			}
			
			Debug.Log($"Instantiating a new game player number {playerNumber}");
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		
		
		#region <<---------- Properties and Fields ---------->>
		
		public int PlayerNumber { get; } = 0;
		
		#if Rewired	
		private readonly Rewired.Player _rePlayer;
		#endif
		
		private Transform _cameraTransform;

		public CPlayerCamera GetCamera => this._playerCamera;
		private CPlayerCamera _playerCamera;

		private readonly List<CCharacterBase> _characters = new List<CCharacterBase>();

		private readonly CompositeDisposable _compositeDisposable;
		private readonly CGameSettings _gameSettings;
		private readonly CBlockingEventsManager _blockingEventsManager;
        private CUINavigationManager _navigationManager;

        #endregion <<---------- Properties and Fields ---------->>

		
		

		#region <<---------- Events ---------->>
		
		private void SignToInputEvents() {
			
			// movement
			Observable.EveryUpdate().Subscribe(_ => {
				if (this._characters.Count <= 0) return;
				
				var character = this.GetControllingCharacter();
				if (character == null) return;

				if (this._blockingEventsManager.IsAnyBlockingEventHappening) {
					character.InputMovementRaw = Vector2.zero;
					character.InputMovementDirRelativeToCam = Vector3.zero;
					return;
				}
				
				#if Rewired
				var inputMovement = this._rePlayer.GetAxis2D(CInputKeys.MOV_X, CInputKeys.MOV_Y);
				
				var (camF, camR) = this.GetCameraVectors();
				
				character.InputMovementRaw = inputMovement.normalized;
				character.InputMovementDirRelativeToCam = (camF * inputMovement.y + camR * inputMovement.x).normalized;
				#else
				Debug.LogError("'GamePlayer movement handling not implemented without Rewired'");
				#endif
			}).AddTo(this._compositeDisposable);
			
			#if Rewired
			this._rePlayer.AddInputEventDelegate(this.InputInteract, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, CInputKeys.INTERACT);
			this._rePlayer.AddInputEventDelegate(this.InputRun, UpdateLoopType.Update, CInputKeys.RUN);
			this._rePlayer.AddInputEventDelegate(this.InputResetCameraRotation, UpdateLoopType.Update, CInputKeys.RESET_CAM_ROTATION);
			this._rePlayer.AddInputEventDelegate(this.InputPause, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, CInputKeys.MENU_PAUSE);
			#endif

		}

		private void UnsignFromInputEvents() {
			#if Rewired
			this._rePlayer?.RemoveInputEventDelegate(this.InputInteract);
			this._rePlayer?.RemoveInputEventDelegate(this.InputRun);
			this._rePlayer?.RemoveInputEventDelegate(this.InputResetCameraRotation);
			this._rePlayer?.RemoveInputEventDelegate(this.InputPause);
			#endif
		}

		#endregion <<---------- Events ---------->>
		
		
		
		
		#region <<---------- Character Creation and Exclusion ---------->>
		
		#if UnityAddressables
		public async Task<CCharacterBase> InstantiateAndAssignCharacter(AssetReference charToCreate) {
			
			if (charToCreate == null || !charToCreate.RuntimeKeyIsValid()) {
				Debug.LogWarning($"Created player {this.PlayerNumber} with no controlling character.");
				return null;
			}

			var createdGo = await CAssets.LoadAndInstantiateGameObjectAsync(charToCreate.RuntimeKey.ToString());
			if (createdGo == null) {
				Debug.LogWarning($"Player {this.PlayerNumber} cant find character '{charToCreate}' to control.");
				return null;
			}

			var character = createdGo.GetComponent<CCharacterBase>();

			if (character == null) {
				Debug.LogError($"{charToCreate} gameobject doesnt have a {nameof(CCharacterBase)} component on it! could not create player!");
				return null;
			}

			var entryPoint = CSceneEntryPoint.GetSceneEntryPointTransform(0);
			if (entryPoint != null) {
				Debug.Log($"Setting '{createdGo.name}' to entryPoint number'{0}'", entryPoint.gameObject);
				character.TeleportToLocation(entryPoint.transform.position, entryPoint.transform.rotation);
				Physics.SyncTransforms();
			}
			createdGo.SetActive(false);
			createdGo.name = $"[Character] {createdGo.name}";
			
			await this.AddControllingCharacter(character);
			createdGo.SetActive(true);
			
			await this.CheckIfNeedToCreateCamera();

			Debug.Log($"Created player {this.PlayerNumber} controlling character '{createdGo.name}'.", createdGo);

			return character;
		}
		#endif

		#endregion <<---------- Character Creation and Exclusion ---------->>

		
		
		
		#region <<---------- Character Control ---------->>

		public async Task AddControllingCharacter(CCharacterBase character) {
			if (this._characters.Contains(character)) {
				Debug.LogError($"Will not add {character.name} to player {this.PlayerNumber} control because it is already controlling it!");
				return;
			}

			this._characters.Add(character);
		}

		public async Task RemoveAndDestroyAllControllingCharacters() {
            foreach (var character in this._characters) {
                character.CDestroy();
            }
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
        
        public bool IsTransformRelatedToPlayer(Transform transformToCheck) {
            return this._characters.Any(character => character != null && character.transform == transformToCheck);
        }

		#endregion <<---------- Character Control ---------->>




		#region <<---------- Player Camera ---------->>

		private async Task CheckIfNeedToCreateCamera() {
			if (this._cameraTransform != null) return;
			var mainChar = this.GetControllingCharacter();
			if (mainChar == null) return;
			
			#if UnityAddressables
			var createdGo = await CAssets.LoadAndInstantiateGameObjectAsync("PlayerCamera");
			createdGo.name = $"[CAM] {mainChar.name}";
			
			Debug.Log($"Created {mainChar.name} Camera", createdGo);

			this._playerCamera = createdGo.GetComponent<CPlayerCamera>();
			
			this._playerCamera.Initialize(this);
			this._cameraTransform = this._playerCamera.GetCameraTransform();
			#else
			Debug.LogError("Default player camera creation not implemented without UnityAddressables");
			#endif
		}

		private (Vector3 camF, Vector3 camR) GetCameraVectors() {
			if (this._cameraTransform == null) return (Vector3.forward, Vector3.right);
			
			var camF = this._cameraTransform.forward;
			camF.y = 0;
			camF.Normalize();
			
			var camR = this._cameraTransform.right;
			camR.y = 0;
			camR.Normalize();
				
			return (camF, camR);
		}

		private (Vector3 camF, Vector3 camR) GetCameraVectorsRelativeToCharacter(CCharacterBase relativeTo) {
			if (this._cameraTransform == null) return (Vector3.forward, Vector3.right);
			
			var camF = relativeTo.Position - this._cameraTransform.position;
			camF.y = 0;
			camF.Normalize();

			var camR = -Vector3.Cross(camF, Vector3.up);
			camR.y = 0;
			camR.Normalize();
			
			return (camF, camR);
		}
		
		#endregion <<---------- Player Camera ---------->>

		

		
		#region <<---------- Input ---------->>

		#if Rewired
		
		private void InputPause(InputActionEventData data) {
			if (!data.GetButtonDown()) return;

			if (Time.timeScale <= 0) return;
			
			if (!this._blockingEventsManager.IsOnMenu) {
				this.OpenMenu().CAwait();
			}
		}

		private void InputResetCameraRotation(InputActionEventData data) {
			if (this._blockingEventsManager.IsAnyBlockingEventHappening) return;
			if (!data.GetButtonDown()) return;
			if (this._playerCamera == null) return;
			this._playerCamera.ResetRotation();
		}
		
		private void InputRun(InputActionEventData data) {
			if (this._blockingEventsManager.IsAnyBlockingEventHappening) return;
			var character = this.GetControllingCharacter();
			if (character == null) return;
			var inputRun = data.GetButton();
			character.InputRun = inputRun;
		}
		
		private void InputInteract(InputActionEventData data) {
			if (this._blockingEventsManager.IsAnyBlockingEventHappening) return;
			var character = this.GetControllingCharacter();
			if (character == null) {
				Debug.LogError($"Character number '{this.PlayerNumber}' tried to interact but is not controlling any character.");
				return;
			}
			var interactionComponent = character.GetComponentInChildren<CPlayerInteractionBase>();
			if (interactionComponent == null) {
				Debug.LogError($"Character number '{this.PlayerNumber}' tried to interact but doesnt have any interaction component.");
				return;
			}
			interactionComponent.TryToInteract();
		}
		
		#endif
		
		#endregion <<---------- Input ---------->>




		#region <<---------- Controlls Mappings ---------->>

		#if Rewired
		private void SetInputLayout(bool onMenu) {
			SetInputLayout(this._rePlayer, onMenu);
		}

		public static void SetInputLayout(Rewired.Player rePlayer, bool onMenu) {
			if (rePlayer == null) {
				Debug.LogError($"cannot set input for a null Rewired.Player");
				return;
			}

			int joystickControllersCount = rePlayer.controllers.joystickCount;
			int customControllersCount = rePlayer.controllers.customControllerCount;
			#if Rewired
			rePlayer.controllers.maps.SetMapsEnabled(!onMenu, "Default");
			rePlayer.controllers.maps.SetMapsEnabled(onMenu, "UI"); 
			//ReInput.players.GetSystemPlayer().controllers.maps.SetMapsEnabled(onMenu, "UI");
			#endif
			
			Debug.Log($"Player ID '{rePlayer.id}' controllers maps onMenu changed to '{onMenu}'\nCustom Controllers: {customControllersCount}, JoystickControllers: {joystickControllersCount}");
		}
		#endif
		
		#endregion <<---------- Controlls Mappings ---------->>
		
		


		#region <<---------- Pause Menu ---------->>
		
		private async Task OpenMenu() {
			if (this._blockingEventsManager.IsOnMenu) return;
			this._blockingEventsManager.IsOnMenu = true;
			try {
				#if UnityAddressables
				await this._navigationManager.OpenMenu(this._gameSettings.AssetRef_PauseMenu, null, null);
				#else
				Debug.LogError("'GamePlayer OpenMenu' not implemented without UnityAddressables");
				#endif
			} catch (Exception e) {
				this._blockingEventsManager.IsOnMenu = false;
				Debug.LogError("Exception trying to OpenMenu on GamePlayer: " + e);
			}
		}
		
		#endregion <<---------- Pause Menu ---------->>
		

		

		#region <<---------- Disposable ---------->>
		
		public void Dispose() {
			this._compositeDisposable?.Dispose();

			this.UnsignFromInputEvents();
			#if Rewired
			this._blockingEventsManager.OnMenu -= this.SetInputLayout;
			#endif
		}
		
		#endregion <<---------- Disposable ---------->>

    }
}

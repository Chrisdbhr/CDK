using System;
using System.Collections.Generic;
using System.Linq;
using CDK.Interaction;
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

			this._disposables?.Dispose();
			this._disposables = new CompositeDisposable();

			this.PlayerNumber = playerNumber;
			
			#if Rewired	
			this._rePlayer = ReInput.players.GetPlayer(this.PlayerNumber);
			#endif

			this.SignToInputEvents();
			
			#if Rewired
			SetInputLayout(this._rePlayer, this._blockingEventsManager.IsOnMenu);
			this._blockingEventsManager.OnMenuRetainable.IsRetainedRx.Subscribe(this.SetInputLayout)
            .AddTo(this._disposables);
			#endif

            // application lost focus
			if (!Application.isEditor) {
                Application.focusChanged += this.OnApplicationFocusChanged;
            }
            
            // Steam
            CSteamManager.OnSteamOverlayOpen += this.TryPauseGame;
			
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

		private readonly List<CCharacter_Base> _characters = new List<CCharacter_Base>();

		private readonly CompositeDisposable _disposables;
		private readonly CGameSettings _gameSettings;
        private readonly CBlockingEventsManager _blockingEventsManager;
        private readonly CSteamManager _steamManager;
        private CUINavigationManager _navigationManager;

        #endregion <<---------- Properties and Fields ---------->>

		
		

		#region <<---------- Events ---------->>
		
		private void SignToInputEvents() {
			
			// movement
			Observable.EveryUpdate().Subscribe(_ => {
				if (this._characters.Count <= 0) return;
				
				var character = this.GetControllingCharacter();
				if (character == null) return;
				
				#if Rewired
				var inputMovement2d = this._rePlayer.GetAxis2D(CInputKeys.MOV_X, CInputKeys.MOV_Y);
				
				var (camF, camR) = this.GetCameraVectors();
				
				character.Input.Movement = Vector3.ClampMagnitude(((camF * inputMovement2d.y) + (camR * inputMovement2d.x)), 1f);
				#else
				Debug.LogError("'GamePlayer movement handling not implemented without Rewired'");
				#endif
			}).AddTo(this._disposables);
            
			#if Rewired
			this._rePlayer.AddInputEventDelegate(this.InputInteract, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, CInputKeys.INTERACT);
            this._rePlayer.AddInputEventDelegate(this.InputRun, UpdateLoopType.Update, CInputKeys.RUN);
            this._rePlayer.AddInputEventDelegate(this.InputJump, UpdateLoopType.Update, CInputKeys.JUMP);
            this._rePlayer.AddInputEventDelegate(this.InputWalk, UpdateLoopType.Update, CInputKeys.WALK);
			this._rePlayer.AddInputEventDelegate(this.InputResetCameraRotation, UpdateLoopType.Update, CInputKeys.RESET_CAM_ROTATION);
			this._rePlayer.AddInputEventDelegate(this.InputPause, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, CInputKeys.MENU_PAUSE);
			#endif

		}

		private void UnsignFromInputEvents() {
			#if Rewired
			this._rePlayer?.RemoveInputEventDelegate(this.InputInteract);
            this._rePlayer?.RemoveInputEventDelegate(this.InputRun);
            this._rePlayer?.RemoveInputEventDelegate(this.InputJump);
            this._rePlayer?.RemoveInputEventDelegate(this.InputWalk);
			this._rePlayer?.RemoveInputEventDelegate(this.InputResetCameraRotation);
			this._rePlayer?.RemoveInputEventDelegate(this.InputPause);
			#endif
		}

		#endregion <<---------- Events ---------->>
		
		
		
		
		#region <<---------- Character Creation and Exclusion ---------->>
		
		#if UnityAddressables
		
		public CCharacter_Base InstantiateAndAssignCharacter(string key) {
			if (key.CIsNullOrEmpty()) {
				Debug.LogWarning($"Created player {this.PlayerNumber} with no controlling character because '{nameof(key)}' is null!");
				return null;
			}

            var character = CAssets.LoadResourceAndInstantiate<CCharacter_Base>(key);

            if (character == null) {
                Debug.LogError($"Asset key '{key}' gameobject doesnt have a {nameof(CCharacter_Base)} component on it! could not create player!");
                return null;
            }

            return this.AssignInstantiatedCharacter(character);
		}

        public CCharacter_Base AssignInstantiatedCharacter(CCharacter_Base instantiatedCharacter) {
            var entryPoint = CSceneEntryPoint.GetSceneEntryPointTransformByNumber(0);
            if (entryPoint != null) {
                Debug.Log($"Setting '{instantiatedCharacter.name}' to entryPoint number'{0}'", entryPoint.gameObject);
                instantiatedCharacter.TeleportToLocation(entryPoint.transform.position, entryPoint.transform.rotation);
                Physics.SyncTransforms();
            }
            instantiatedCharacter.gameObject.SetActive(false);
            instantiatedCharacter.name = $"CHARACTER - {instantiatedCharacter.name}";
			
            this.AddControllingCharacter(instantiatedCharacter);
            instantiatedCharacter.gameObject.SetActive(true);
			
            this.CheckIfNeedToCreateCamera();

            Debug.Log($"Created player {this.PlayerNumber} controlling character '{instantiatedCharacter.name}'.", instantiatedCharacter);
            return instantiatedCharacter;
        }
        
		#endif

		#endregion <<---------- Character Creation and Exclusion ---------->>

		
		
		
		#region <<---------- Character Control ---------->>

		public void AddControllingCharacter(CCharacter_Base character) {
			if (this._characters.Contains(character)) {
				Debug.LogError($"Will not add {character.name} to player {this.PlayerNumber} control because it is already controlling it!");
				return;
			}

			this._characters.Add(character);
		}

		public void RemoveAndDestroyAllControllingCharacters() {
            foreach (var character in this._characters) {
                character.CDestroy();
            }
            this._characters.Clear();
		}
		
		public CCharacter_Base GetControllingCharacter() {
			return this._characters.FirstOrDefault(c => c != null);
		}

		public List<GameObject> GetAllRelatedGameObjects() {
			var list = this._characters.Select(characterBase => characterBase.gameObject).ToList();
			if(this._cameraTransform != null) list.Add(this._cameraTransform.root.gameObject);
			return list;
		}

		public bool IsControllingCharacter(CCharacter_Base characterBase) {
			return this._characters.Contains(characterBase);
		}
        
        public bool IsTransformRelatedToPlayer(Transform transformToCheck) {
            return this._characters.Any(character => character != null && character.transform == transformToCheck);
        }

		#endregion <<---------- Character Control ---------->>




		#region <<---------- Player Camera ---------->>

		private void CheckIfNeedToCreateCamera() {
			if (this._cameraTransform != null) return;
			var mainChar = this.GetControllingCharacter();
			if (mainChar == null) return;
			
			#if UnityAddressables
			this._playerCamera = CAssets.LoadResourceAndInstantiate<CPlayerCamera>("PlayerCamera");
            this._playerCamera.name = $"CAM - {mainChar.name}";
           
            Debug.Log($"Created {mainChar.name} Camera", this._playerCamera);

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

		private (Vector3 camF, Vector3 camR) GetCameraVectorsRelativeToCharacter(CCharacter_Base relativeTo) {
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
            this.TryPauseGame();
		}

		private void InputResetCameraRotation(InputActionEventData data) {
			if (this._blockingEventsManager.IsAnyHappening) return;
			if (!data.GetButtonDown()) return;
			if (this._playerCamera == null) return;
			this._playerCamera.ResetRotation();
		}
		
        private void InputWalk(InputActionEventData data) {
            if (this._blockingEventsManager.IsAnyHappening) return;
            var character = this.GetControllingCharacter();
            if (character == null) return;
            character.Input.Walk = data.GetButton();
        }
        
		private void InputRun(InputActionEventData data) {
			if (this._blockingEventsManager.IsAnyHappening) return;
			var character = this.GetControllingCharacter();
			if (character == null) return;
			character.Input.Run = data.GetButton();
		}

        private void InputJump(InputActionEventData data) {
            if (this._blockingEventsManager.IsAnyHappening) return;
            var character = this.GetControllingCharacter();
            if (character == null) return;
            character.Input.Jump = data.GetButton();
        }
		
		private void InputInteract(InputActionEventData data) {
			if (this._blockingEventsManager.IsAnyHappening) return;
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
            
            if (rePlayer.controllers == null) {
                Debug.LogError($"cannot set input for a Rewired.Player with null controllers");
                return;
            }

			int joystickControllersCount = rePlayer.controllers.joystickCount;
			int customControllersCount = rePlayer.controllers.customControllerCount;
			#if Rewired
			rePlayer.controllers.maps.SetMapsEnabled(!onMenu, "Default");
			rePlayer.controllers.maps.SetMapsEnabled(onMenu, "UI"); 
			#endif
			
			Debug.Log($"Player ID '{rePlayer.id}' controllers maps onMenu changed to '{onMenu}'\nCustom Controllers: {customControllersCount}, JoystickControllers: {joystickControllersCount}");
		}
		#endif
		
		#endregion <<---------- Controlls Mappings ---------->>
		
		


		#region <<---------- Pause ---------->>
		
		private void TryPauseGame() {
            if (Time.timeScale <= 0) return;
			if (this._blockingEventsManager.IsOnMenu) return;
            this._navigationManager.OpenMenu(CGameSettings.AssetRef_PauseMenu, null, null);
		}

        private void OnApplicationFocusChanged(bool focused) {
            if (focused) return;
            this.TryPauseGame();
        }
        
		#endregion <<---------- Pause ---------->>
		

		

		#region <<---------- Disposable ---------->>
		
		public void Dispose() {
			this._disposables?.Dispose();
            if(this._steamManager) CSteamManager.OnSteamOverlayOpen -= this.TryPauseGame;
            Application.focusChanged -= this.OnApplicationFocusChanged;
			this.UnsignFromInputEvents();
		}
		
		#endregion <<---------- Disposable ---------->>

    }
}

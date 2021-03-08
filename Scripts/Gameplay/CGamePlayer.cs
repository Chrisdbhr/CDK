using System;
using System.Collections.Generic;
using System.Linq;
using CDK.UI;
using Rewired;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;
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
			this._rePlayer.controllers.maps.SetMapsEnabled(true, "Default");
			this._rePlayer.controllers.maps.SetMapsEnabled(false, "UI");
			
			this.SignToInputEvents();

			CBlockingEventsManager.OnMenuEvent += this.SetInputLayout;
		

			Application.focusChanged += focused => {
				if(!focused) this.OpenMenu();
			};
			
			Debug.Log($"Instantiating a new game player number {playerNumber}");
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		
		
		#region <<---------- Properties and Fields ---------->>
		
		private const string MENU_PAUSE_ASSETKEY = "menu-pause";
		private const string MENU_INVENTORY_ASSETKEY = "menu-ingame";

		public int PlayerNumber { get; } = 0;
		private Rewired.Player _rePlayer;
		private CUIBase _openPauseMenu;
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
					character.InputMovementRaw = inputMovement.normalized;
					character.InputMovementDirRelativeToCam = (camF * inputMovement.y + camR * inputMovement.x).normalized;
				}
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
		
		
		
		
		#region <<---------- Character Control ---------->>

		public void AddControllingCharacter(CCharacterBase character) {
			if (this._characters.Contains(character)) {
				Debug.LogError($"Will not add {character.name} to player {this.PlayerNumber} control because it is already controlling it!");
				return;
			}

			CSceneManager.SetTransformToSceneEntryPoint(character.transform);
			
			this._characters.Add(character);
			
			this.CheckIfNeedToCreateCamera();
			
			
			#if UNITY_EDITOR
			Selection.activeGameObject = character.gameObject;
			#endif
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

				this._cPlayerCamera = createdGo.GetComponent<CPlayerCamera>();
				this._cPlayerCamera.Initialize(this);
				this._cameraTransform = this._cPlayerCamera.GetCameraTransform();
			};
			
		}
		
		#endregion <<---------- Player Camera ---------->>

		

		
		#region <<---------- Input ---------->>

		private void InputPause(InputActionEventData data) {
			if (!data.GetButtonDown()) return;
			
			if (this._openPauseMenu == null) {
				this.OpenMenu();
			}
			else {
				this.CloseMenu();
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
			//var layoutName = onMenu ? "UI" : "Default";
			
			this._rePlayer.controllers.maps.SetMapsEnabled(!onMenu, "Default");
			ReInput.players.GetSystemPlayer().controllers.maps.SetMapsEnabled(onMenu, "Default");
			
			Debug.Log($"Player {this.PlayerNumber} controllers maps onMenu changed to {onMenu}");
		}
		
		#endregion <<---------- Controlls Mappings ---------->>
		
		


		#region <<---------- Pause Menu ---------->>
		
		private void OpenMenu() {
			if (CBlockingEventsManager.IsOnMenu) {
				Debug.LogWarning("Tried to open a menu when already on some Menu!");
				return;
			}

			var menuAssetKey = CBlockingEventsManager.IsPlayingCutscene ? MENU_PAUSE_ASSETKEY : MENU_INVENTORY_ASSETKEY;
			
			CBlockingEventsManager.IsOnMenu = true;
			try {
				Addressables.LoadAssetAsync<GameObject>(menuAssetKey).Completed += handle => {
					this._openPauseMenu = Object.Instantiate(handle.Result).GetComponent<CUIBase>();
					this._openPauseMenu.name = $"[{menuAssetKey.ToUpper()}] {this._openPauseMenu.name}";
					
					Debug.Log($"Created menu: {this._openPauseMenu.name}", this._openPauseMenu);

					this._openPauseMenu.OpenMenu().CAwait();
				};
				
			} catch (Exception e) {
				CBlockingEventsManager.IsOnMenu = false;
				Debug.LogError(e);
			}
		}
		private void CloseMenu() {
			this._openPauseMenu.CloseMenu().CAwait();
		}
		
		#endregion <<---------- Pause Menu ---------->>
		

		

		#region <<---------- Disposable ---------->>
		
		public void Dispose() {
			this._compositeDisposable?.Dispose();
			
			this.UnsignFromInputEvents();
			CBlockingEventsManager.OnMenuEvent -= this.SetInputLayout; 
		}
		
		#endregion <<---------- Disposable ---------->>

	}
}

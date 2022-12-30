using System;
using UnityEngine;
using UniRx;

namespace CDK {
	public class CCursorManager {

		[NonSerialized] private readonly CGameSettings _gameSettings;
		[NonSerialized] private readonly CBlockingEventsManager _blockingEventsManager;

		
		
		
		public CCursorManager() {
			this._gameSettings = CDependencyResolver.Get<CGameSettings>();
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
            
            this._blockingEventsManager.OnMenuRetainable.IsRetainedRx.Subscribe(onMenu => {
                if (!onMenu) {
                    SetCursorState(false);
                    return;
                }
                ShowMouseIfNeeded();
            });
            
            SetCursorState(!this._gameSettings.CursorStartsHidden);

            CInputManager.InputTypeChanged += OnInputTypeChanged;
		}

		private void OnInputTypeChanged(CInputManager.InputType newType) {
			SetCursorState(CInputManager.ActiveInputType == CInputManager.InputType.Mouse && this._blockingEventsManager.IsOnMenu);
		}

		private void SetCursorState(bool visible) {
			if (Cursor.visible != visible) Debug.Log($"Setting cursor visibility to {visible}");
			Cursor.visible = visible;
			
			if (visible) {
				Cursor.lockState = CursorLockMode.None;
			}
			else {
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

        public void ShowMouseIfNeeded() {
            if (!CInputManager.ActiveInputType.IsMouseOrKeyboard()) return;
            SetCursorState(true);
        }

        public void HideCursor() {
            SetCursorState(false);
        }

	}
}

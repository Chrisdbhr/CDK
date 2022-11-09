using System;
using CDK.UI;
using UnityEngine;

namespace CDK {
	public class CCursorManager {

		[NonSerialized] private readonly CGameSettings _gameSettings;
		[NonSerialized] private readonly CBlockingEventsManager _blockingEventsManager;

		
		
		
		public CCursorManager() {
			this._gameSettings = CDependencyResolver.Get<CGameSettings>();
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();

            SetCursorState(!this._gameSettings.CursorStartsHidden);
			this._blockingEventsManager.OnMenu += onMenu => {
                if (!onMenu) {
                    SetCursorState(false);
                    return;
                }
                ShowMouseIfNeeded();
            };
            CInputManager.InputTypeChanged += OnInputTypeChanged;
		}

		private void OnInputTypeChanged(CInputManager.InputType newType) {
			SetCursorState(CInputManager.ActiveInputType == CInputManager.InputType.Mouse && this._blockingEventsManager.IsOnMenu);
		}

		public static void SetCursorState(bool visible) {
			if(Cursor.visible != visible) Debug.Log($"Setting cursor visibility to {visible}");
			Cursor.visible = visible;
			
			if (visible) {
				Cursor.lockState = CursorLockMode.None;
			}
			else {
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

        public static void ShowMouseIfNeeded() {
            if (CInputManager.ActiveInputType != CInputManager.InputType.Mouse) return;
            SetCursorState(true);
        }

	}
}

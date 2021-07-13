using System;
using UnityEngine;

namespace CDK {
	public class CCursorManager {

		[NonSerialized] private readonly CGameSettings _gameSettings;
		[NonSerialized] private readonly CBlockingEventsManager _blockingEventsManager;

		
		
		
		public CCursorManager() {
			this._gameSettings = CDependencyResolver.Get<CGameSettings>();
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();

			SetCursorState(!this._gameSettings.CursorStartsHidden);
			this._blockingEventsManager.OnMenu += SetCursorState;
			CInputManager.InputTypeChanged += OnInputTypeChanged;
		}

		private void OnInputTypeChanged(CInputManager.InputType newType) {
			SetCursorState(CInputManager.ActiveInputType == CInputManager.InputType.MouseAndKeyboard && this._blockingEventsManager.IsOnMenu);
		}

		public static void SetCursorState(bool visible) {
			Debug.Log($"Setting cursor visibility to {visible}");
			Cursor.visible = visible;
			
			if (visible) {
				Cursor.lockState = CursorLockMode.None;
			}
			else {
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

	}
}

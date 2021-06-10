using UnityEngine;

namespace CDK {
	public class CCursorManager {

		private readonly CGameSettings _gameSettings;
		
		
		
		
		public CCursorManager() {
			this._gameSettings = CDependencyContainer.Get<CGameSettings>();
			
			SetCursorState(!this._gameSettings.CursorStartsHidden);
			CBlockingEventsManager.OnMenu += SetCursorState;
			CInputManager.InputTypeChanged += OnInputTypeChanged;
		}

		private void OnInputTypeChanged(CInputManager.InputType newType) {
			SetCursorState(CInputManager.ActiveInputType == CInputManager.InputType.MouseAndKeyboard && CBlockingEventsManager.IsOnMenu);
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

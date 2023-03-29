using System;
using UnityEngine;
using UniRx;

namespace CDK {
	public class CCursorManager {
        
        #region <<---------- Singleton ---------->>

        public static CCursorManager get { 
            get{
                if (CApplication.IsQuitting || !Application.isPlaying) {
                    return null;
                }
                if (_instance == null) {
                    _instance = new CCursorManager();
                }
                return _instance;
            }
        }
        private static CCursorManager _instance;

        #endregion <<---------- Singleton ---------->>
        
        
        
        
		[NonSerialized] private readonly CGameSettings _gameSettings;
		[NonSerialized] private readonly CBlockingEventsManager _blockingEventsManager;

		
		
		
		public CCursorManager() {
			this._gameSettings = CDependencyResolver.Get<CGameSettings>();
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
            
            this._blockingEventsManager.OnMenuRetainable.IsRetainedAsObservable().Subscribe(onMenu => {
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
			SetCursorState(CInputManager.ActiveInputType.IsMouseOrKeyboard() && this._blockingEventsManager.IsOnMenu);
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

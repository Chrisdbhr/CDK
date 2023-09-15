using System;
using UnityEngine;
using UniRx;

namespace CDK {
	public class CCursorManager {
        
        #region <<---------- Singleton ---------->>

        public static CCursorManager get { 
            get{
                if (CSingletonHelper.CannotCreateAnyInstance() || _instance != null) return _instance;
                return (_instance = new CCursorManager());
            }
        }
        private static CCursorManager _instance;

        #endregion <<---------- Singleton ---------->>




        #region <<---------- Properties and Fields ---------->>

        [NonSerialized] private readonly CBlockingEventsManager _blockingEventsManager;

        #endregion <<---------- Properties and Fields ---------->>





        #region <<---------- Initializers ---------->>
        
        private CCursorManager() {
            this._blockingEventsManager = CBlockingEventsManager.get;
            
            this._blockingEventsManager.OnMenuRetainable.IsRetainedAsObservable().Subscribe(onMenu => {
                if (!onMenu) {
                    SetCursorState(false);
                    return;
                }
                ShowMouseIfNeeded();
            });
            
            SetCursorState(!CGameSettings.get.CursorStartsHidden);

            CInputManager.InputTypeChanged += OnInputTypeChanged;
        }

        #endregion <<---------- Initializers ---------->>


		private void OnInputTypeChanged(CInputManager.InputType newType) {
			SetCursorState(CInputManager.ActiveInputType.IsMouseOrKeyboard() && this._blockingEventsManager.IsOnMenu);
		}

		private void SetCursorState(bool visible) {
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
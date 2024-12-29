using UnityEngine;
using Reflex.Core;

namespace CDK {
	public class CCursorManager {

        #region <<---------- Properties and Fields ---------->>

        readonly CBlockingEventsManager _blockingEventsManager;
        readonly CInputManager _inputManager;

        #endregion <<---------- Properties and Fields ---------->>





        #region <<---------- Initializers ---------->>

        public CCursorManager(Container container) {
	        _blockingEventsManager = container.Resolve<CBlockingEventsManager>();
	        _inputManager = container.Resolve<CInputManager>();

            _blockingEventsManager.MenuRetainable.StateEvent += (onMenu) => {
                if (!onMenu) {
                    SetCursorState(false);
                    return;
                }
                ShowMouseIfNeeded();
            };

            _inputManager.InputTypeChanged += OnInputTypeChanged;
        }

        #if UNITY_EDITOR
		~CCursorManager() {
			SetCursorState(true);
		}
		#endif

		#endregion <<---------- Initializers ---------->>


		void OnInputTypeChanged(object sender, CInputManager.InputType inputType) {
			SetCursorState(_inputManager.ActiveInputType.IsMouseOrKeyboard() && _blockingEventsManager.IsInMenu);
		}

		static void SetCursorState(bool visible)
		{
			Cursor.visible = visible;
			Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
		}

        public void ShowMouseIfNeeded() {
            if (!_inputManager.ActiveInputType.IsMouseOrKeyboard()) return;
            SetCursorState(true);
        }

		public void HideCursor() {
            SetCursorState(false);
        }

	}
}
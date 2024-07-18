using UnityEngine;
using R3;
using Reflex.Core;

namespace CDK {
	public class CCursorManager {

        #region <<---------- Properties and Fields ---------->>

        readonly CBlockingEventsManager _blockingEventsManager;
        readonly CInputManager _inputManager;
		CompositeDisposable _disposeOnDestroy = new CompositeDisposable();

        #endregion <<---------- Properties and Fields ---------->>





        #region <<---------- Initializers ---------->>

        public CCursorManager(Container container) {
	        _blockingEventsManager = container.Resolve<CBlockingEventsManager>();
	        _inputManager = container.Resolve<CInputManager>();

			_disposeOnDestroy.Clear();

            _blockingEventsManager.OnMenuRetainable.IsRetainedAsObservable().Subscribe(onMenu => {
                if (!onMenu) {
                    SetCursorState(false);
                    return;
                }
                ShowMouseIfNeeded();
            })
			.AddTo(_disposeOnDestroy);

            _inputManager.InputTypeChanged += OnInputTypeChanged;
        }

        #if UNITY_EDITOR
		~CCursorManager() {
			SetCursorState(true);
			_disposeOnDestroy?.Dispose();
		}
		#endif

		#endregion <<---------- Initializers ---------->>


		void OnInputTypeChanged(object sender, CInputManager.InputType inputType) {
			SetCursorState(_inputManager.ActiveInputType.IsMouseOrKeyboard() && _blockingEventsManager.IsOnMenu);
		}

		static void SetCursorState(bool visible) {
			Cursor.visible = visible;
			
			if (visible) {
				Cursor.lockState = CursorLockMode.None;
			}
			else {
				Cursor.lockState = CursorLockMode.Locked;
			}
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
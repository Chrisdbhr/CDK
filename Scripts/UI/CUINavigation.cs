using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

namespace CDK.UI {
	public class CUINavigation {

		#region <<---------- Singleton ---------->>
		
		public static CUINavigation get {
			get {
				if (_instance != null) return _instance;
				Debug.Log($"Creating new instance of {nameof(CUINavigation)}");
				CApplication.IsQuitting += () => {
					_instance = null;
				};
				return _instance = new CUINavigation();
			}
		}
		private static CUINavigation _instance;
		
		#endregion <<---------- Singleton ---------->>


		

		#region <<---------- Initializers ---------->>

		public CUINavigation() {

			this._navigationHistory = new Stack<CUIBase>();

		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		

		#region <<---------- Properties ---------->>
		public CUIBase[] NavigationHistory {
			get { return this._navigationHistory.ToArray(); }
		}
		private int LastFrameAMenuClosed;
		private CompositeDisposable _navigationDisposables;
		private readonly Stack<CUIBase> _navigationHistory;

		#endregion <<---------- Properties ---------->>
		
		
		
		
		#region <<---------- Open / Close ---------->>

		/// <summary>
		/// Opens a menu, registering the button that opened it.
		/// </summary>
		/// <returns>returns the new opened menu.</returns>
		public async Task<CUIBase> OpenMenu(AssetReference uiReference, CUIInteractable originButton, CUIBase originUI) {
			if (CApplication.Quitting) return null;

			var ui = await CAssets.LoadAndInstantiateUI(uiReference);
			if (ui == null) {
				Debug.LogError($"Could not open menu '{uiReference.RuntimeKey}'");
				return null;
			}
			
			bool alreadyOpened = this._navigationHistory.Any(x => x == ui);
			if (alreadyOpened) {
				Debug.LogError($"Tried to open the same menu twice! Will not open menu '{ui.name}'");
				Addressables.ReleaseInstance(ui.gameObject);
				return null;
			}
			
			Debug.Log($"Requested navigation to '{ui.name}'");

			this.HideLastMenuIfSet();
			
			await ui.Open(this._navigationHistory.Count, originUI, originButton);
			
			this.CheckIfIsFirstMenu();
			
			this._navigationHistory.Push(ui);
			
			return ui;
		}

		/// <summary>
		/// Closes active menu selecting previous button.
		/// </summary>
		public async Task CloseCurrentMenu() {
			if (this._navigationHistory.Count <= 0) {
				Debug.LogError("No menu to close");
				return;
			}

			if (this.LastFrameAMenuClosed == Time.frameCount) {
				var lastInHistory = this._navigationHistory.Peek();
				Debug.LogWarning($"Will not close menu '{lastInHistory.name}' because one already closed in this frame.", lastInHistory);
				return;
			}

			var ui = this._navigationHistory.Pop();

			if (ui == null) {
				Debug.LogError("UI popped from Stack was null");
			}
			
			this.CheckIfIsLastMenu();

			if (ui != null) {
				Debug.Log($"Closing Menu '{ui.name}'", ui);
				this.LastFrameAMenuClosed = Time.frameCount;
				ui.Close();
			}
		}

		public async Task EndNavigation() {
			Debug.Log($"Requested EndNavigation of {this._navigationHistory.Count} Menus in history.");
			foreach (var ui in this._navigationHistory) {
				if(ui == null) continue;
				ui.Close();
			}
			this._navigationHistory.Clear();
			CBlockingEventsManager.IsOnMenu = false;
		}
		
		#endregion <<---------- Open / Close ---------->>


		
		
		#region <<---------- Navigation ---------->>
		
		private void CheckIfIsFirstMenu() {
			if (this._navigationHistory.Count > 0) return;
			
			this._navigationDisposables?.Dispose();
			this._navigationDisposables = new CompositeDisposable();
			
			CBlockingEventsManager.IsOnMenu = true;
			
			Observable.EveryUpdate().Subscribe(_ => {
				if (CInputManager.ActiveInputType != CInputManager.InputType.JoystickController) return;
				if (this._navigationHistory.Count <= 0) return;
				var current = EventSystem.current;
				if (current == null) return;
				if (current.currentSelectedGameObject != null) return;
				var activeUi = this._navigationHistory.Peek();
				current.SetSelectedGameObject(activeUi.FirstSelectedObject);
			}).AddTo(this._navigationDisposables);
		}

		private void CheckIfIsLastMenu() {
			if (this._navigationHistory.Count > 0) return;
			
			CBlockingEventsManager.IsOnMenu = false;
			
			this._navigationDisposables?.Dispose();
		}
		
		private void HideLastMenuIfSet() {
			if (this._navigationHistory.Count <= 0) return;
			var current = this._navigationHistory.Peek();
			if (current == null) {
				Debug.LogError("There was a null item on navigation history, this should not happen!");
			}
			else {
				current.HideIfSet(); 
			}
		}
		
		#endregion <<---------- Navigation ---------->>

	}
}

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

			this._compositeDisposable?.Dispose();
			this._compositeDisposable = new CompositeDisposable();
			
			this._navigationHistory = new Stack<(CUIBase current, CUIButton originButton)>();
			
			// check for select button
			Observable.EveryUpdate().Subscribe(_ => {
				if (this._navigationHistory.Count <= 0) return;
				this.CheckForActiveEventSystem(null);
			}).AddTo(this._compositeDisposable);

		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		

		#region <<---------- Properties ---------->>
		public (CUIBase current, CUIButton originButton)[] NavigationHistory {
			get { return this._navigationHistory.ToArray(); }
		}
		private readonly Stack<(CUIBase current, CUIButton originButton)> _navigationHistory;
		private EventSystem _currentEventSystem;

		private CompositeDisposable _compositeDisposable;
		
		#endregion <<---------- Properties ---------->>
		
		
		
		
		#region <<---------- Open / Close ---------->>

		/// <summary>
		/// Opens a menu, registering the button that opened it.
		/// </summary>
		/// <returns>returns the new opened menu.</returns>
		public async Task<CUIBase> OpenMenu(AssetReference uiReference, CUIButton originButton) {
			bool alreadyOpened = this._navigationHistory.Any(x => x.originButton == originButton);
			if (alreadyOpened) {
				Debug.LogError($"Tried to open the same menu twice! Will not open menu '{uiReference.RuntimeKey}'");
				return null;
			}

			var ui = await CAssets.LoadAndInstantiateUI(uiReference);
			if (ui == null) {
				Debug.LogError($"Could not open menu '{uiReference.RuntimeKey}'");
				return null;
			}
			
			Debug.Log($"Requested navigation to '{ui.name}'");

			this.HideLastMenuIfSet();
			
			this._navigationHistory.Push((ui, originButton));

			await ui.Open(this._navigationHistory.Count);
			if (this._navigationHistory.Count > 0) {
				CBlockingEventsManager.IsOnMenu = true;
				this.CheckForActiveEventSystem(ui.FirstSelected);
			}
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

			var ui = this._navigationHistory.Pop();

			if (ui.current == null) {
				Debug.LogError("UI popped from Stack was null");
			}

			this.ShowLastMenuIfHidden();
			
			if (this._navigationHistory.Count <= 0) {
				// this is the last menu in navigation
				CBlockingEventsManager.IsOnMenu = false;
			}
			else {
				this.CheckForActiveEventSystem(ui.originButton);
			}

			if (ui.current != null) {
				Debug.Log($"Closing Menu '{ui.current.name}'");
				await ui.current.Close();
			}
		}

		public async Task EndNavigation() {
			foreach (var (ui, button) in this._navigationHistory) {
				if(ui == null) continue;
				await ui.Close();
			}
			this._navigationHistory.Clear();
			CBlockingEventsManager.IsOnMenu = false;
		}
		
		#endregion <<---------- Open / Close ---------->>


		/// <summary>
		/// check for naoa
		/// </summary>
		/// <param name="interactableToSelect">define taokt gaopgmdosp</param>
		private void CheckForActiveEventSystem(CUIInteractable interactableToSelect) {
			if (this._navigationHistory.Count <= 0) return;
			var nh = this._navigationHistory.Peek();
			if (this._currentEventSystem == null
			   || this._currentEventSystem.transform.root != nh.current.transform.root) {
				this._currentEventSystem = nh.current.GetComponentInChildren<EventSystem>();
			}
			if (this._currentEventSystem.currentSelectedGameObject == null
				|| this._currentEventSystem.currentSelectedGameObject.transform.root != nh.current.transform.root) {
				this._currentEventSystem.SetSelectedGameObject(interactableToSelect != null ? interactableToSelect.gameObject : nh.current.FirstSelected.gameObject);
				Debug.Log($"Setting selected object to '{nh.current.FirstSelected.name}' in menu '{nh.current.name}'");
			}
		}


		private void HideLastMenuIfSet() {
			if (this._navigationHistory.Count <= 0) return;
			var current = this._navigationHistory.Peek().current;
			if (current == null) {
				Debug.LogError("There was a null item on navigation history, this should not happen!");
			}
			else {
				current.HideIfSet(); 
			}
		}
		private void ShowLastMenuIfHidden() {
			if (this._navigationHistory.Count <= 0) return;
			var current = this._navigationHistory.Peek().current;
			if (current == null) {
				Debug.LogError("There was a null item on navigation history, this should not happen!");
			}
			else {
				current.ShowIfHidden(); 
			}
		}
		
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

#if UnityAddressables
using UnityEngine.AddressableAssets;
#endif

namespace CDK.UI {
	public class CUINavigation {

		#region <<---------- Singleton ---------->>
		
		public static CUINavigation get {
			get {
				if (_instance != null) return _instance;
				Debug.Log($"Creating new instance of {nameof(CUINavigation)}");
				CApplication.QuittingEvent += () => {
					_instance = null;
				};
				return _instance = new CUINavigation();
			}
		}
		private static CUINavigation _instance;
		
		#endregion <<---------- Singleton ---------->>


		

		#region <<---------- Initializers ---------->>

		public CUINavigation() {
			this._navigationHistory = new HashSet<CUIBase>();
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		

		#region <<---------- Properties ---------->>
		public CUIBase[] NavigationHistoryToArray => this._navigationHistory.ToArray();
		
		private int LastFrameAMenuClosed;
		private IDisposable _disposableIsNavigating;

        private readonly HashSet<CUIBase> _navigationHistory;
		private readonly CBlockingEventsManager _blockingEventsManager;

		#endregion <<---------- Properties ---------->>
		
		
		
		
		#region <<---------- Open / Close ---------->>

		#if UnityAddressables
	
		public async Task<T> OpenMenu<T>(AssetReference uiReference, CUIBase originUI, CUIInteractable originButton) {
			var openedMenu = await OpenMenu(uiReference, originUI, originButton);
			return openedMenu != null ? openedMenu.GetComponent<T>() : default;
		}

		/// <summary>
		/// Opens a menu, registering the button that opened it.
		/// </summary>
		/// <returns>returns the new opened menu.</returns>
		public async Task<CUIBase> OpenMenu(AssetReference uiReference, CUIBase originUI, CUIInteractable originButton) {
			if (CApplication.IsQuitting) return null;

            var ui = await CAssets.LoadAndInstantiateUI(uiReference);
			if (ui == null) {
				Debug.LogError($"Could not open menu '{uiReference.RuntimeKey}'");
				return null;
			}
            
            this.RemoveNullFromNavigationHistory();
	
			bool alreadyOpened = this._navigationHistory.Any(x => x == ui);
			if (alreadyOpened) {
				Debug.LogError($"Tried to open the same menu twice! Will not open menu '{ui.name}'");
				CAssets.UnloadAsset(ui.gameObject);
				return null;
			}
			
			Debug.Log($"Requested navigation to '{ui.name}'");

			this.HideLastMenuIfSet();
			
			await ui.Open(this._navigationHistory.Count, originUI, originButton);
			
			this.CheckIfIsFirstMenu();
			
			this._navigationHistory.Add(ui);
			
			return ui;
		}
		
		#endif

		/// <summary>
		/// Closes active menu selecting previous button.
		/// </summary>
		public async Task CloseCurrentMenu() {
            this.RemoveNullFromNavigationHistory();
			if (this._navigationHistory.Count <= 0) {
				Debug.LogError("No menu to close");
				return;
			}

            var lastInHistory = this._navigationHistory.Last();
			if (this.LastFrameAMenuClosed == Time.frameCount) {
				Debug.LogWarning($"Will not close menu '{lastInHistory.name}' because one already closed in this frame.", lastInHistory);
				return;
			}

			this._navigationHistory.Remove(lastInHistory);
			
			this.CheckIfIsLastMenu();

            Debug.Log($"Closing Menu '{lastInHistory.name}'", lastInHistory);
            this.LastFrameAMenuClosed = Time.frameCount;
            lastInHistory.Close();
		}

		public async Task EndNavigation() {
			Debug.Log($"Requested EndNavigation of {this._navigationHistory.Count} Menus in history.");
            RemoveNullFromNavigationHistory();
			foreach (var ui in this._navigationHistory) {
				ui.Close();
			}
            this._disposableIsNavigating?.Dispose();
			this._navigationHistory.Clear();
			this._blockingEventsManager.IsOnMenu = false;
            CTime.TimeScale = 1f;
		}
		
		#endregion <<---------- Open / Close ---------->>


		
		
		#region <<---------- Navigation ---------->>

        private bool RemoveNullFromNavigationHistory() {
            int removedCount = this._navigationHistory.RemoveWhere(x => x == null);
            if (removedCount > 0) {
                Debug.LogWarning($"<color=yellow>Removed {removedCount} null UI from navigation history.</color>");
                return true;
            }
            return false;
        }
        
		private void CheckIfIsFirstMenu() {
            this.RemoveNullFromNavigationHistory();
			if (this._navigationHistory.Count > 0) return;

            this._blockingEventsManager.IsOnMenu = true;

			this._disposableIsNavigating?.Dispose();
            this._disposableIsNavigating = Observable.EveryUpdate().Subscribe(_ => {
				//if (CInputManager.ActiveInputType != CInputManager.InputType.JoystickController) return;
                this.RemoveNullFromNavigationHistory();
				if (this._navigationHistory.Count <= 0) return;
				var current = EventSystem.current;
				if (current == null) return;
				if (current.currentSelectedGameObject != null) return;
				var activeUi = this._navigationHistory.Last();
                var objectToSelect = activeUi.FirstSelectedObject;
                if (objectToSelect != null) {
                    current.SetSelectedGameObject(objectToSelect);
                    return;
                }
                var firstInteractable = activeUi.GetComponentsInChildren<CUIInteractable>().FirstOrDefault();
                if (firstInteractable == null) return;
                current.SetSelectedGameObject(firstInteractable.gameObject);
            });
		}

		private void CheckIfIsLastMenu() {
            if (this.RemoveNullFromNavigationHistory() && this._navigationHistory.Count <= 0) {
                // there was null UI on navigation history now it doesnt have anything, end navigation.
                this.EndNavigation().CAwait();
                return;
            }
            if (this._navigationHistory.Count > 0) return;
            this.EndNavigation().CAwait();
        }
		
		private void HideLastMenuIfSet() {
            this.RemoveNullFromNavigationHistory();
			if (this._navigationHistory.Count <= 0) return;
			var current = this._navigationHistory.Last();
            current.HideIfSet(); 
		}
		
		#endregion <<---------- Navigation ---------->>

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

#if UnityAddressables
using UnityEngine.AddressableAssets;
#endif

namespace CDK.UI {
	public class CUINavigationManager {

        #region <<---------- Initializers ---------->>

		public CUINavigationManager() {
			this._navigationHistory = new HashSet<CUIViewBase>();
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		

		#region <<---------- Properties ---------->>
		public CUIViewBase[] NavigationHistoryToArray => this._navigationHistory.ToArray();
		
		private int LastFrameAMenuClosed;
		private IDisposable _disposableIsNavigating;

        private readonly HashSet<CUIViewBase> _navigationHistory;
		private readonly CBlockingEventsManager _blockingEventsManager;

		#endregion <<---------- Properties ---------->>
		
		
		
		
		#region <<---------- Open / Close ---------->>

		#if UnityAddressables
	
		public T OpenMenu<T>(AssetReference uiReference, CUIViewBase originUI, CUIInteractable originButton, bool canCloseByReturnButton = true) {
			var openedMenu = this.OpenMenu(uiReference, originUI, originButton, canCloseByReturnButton);
			return openedMenu != null ? openedMenu.GetComponent<T>() : default;
		}

		/// <summary>
		/// Opens a menu, registering the button that opened it.
		/// </summary>
		/// <returns>returns the new opened menu.</returns>
		public CUIViewBase OpenMenu(AssetReference uiReference, CUIViewBase originUI, CUIInteractable originButton, bool canCloseByReturnButton = true) {
			if (CApplication.IsQuitting) return null;

            var ui = CAssets.LoadAndInstantiateUI(uiReference);
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
			
			ui.Open(this._navigationHistory.Count, originUI, originButton, canCloseByReturnButton);
			
			this.CheckIfIsFirstMenu();
			
			this._navigationHistory.Add(ui);
			
			return ui;
		}
		
		#endif

		/// <summary>
		/// Closes active menu selecting previous button.
		/// </summary>
		/// <returns>returns TRUE if the menu closed.</returns>
		public bool CloseCurrentMenu(bool closeRequestedByReturnButton = false) {
            this.RemoveNullFromNavigationHistory();
			if (this._navigationHistory.Count <= 0) {
				Debug.LogError("No menu to close");
				return false;
			}

            var lastInHistory = this._navigationHistory.Last();
            if (lastInHistory == null) {
                Debug.LogError("Last menu to close in navigation history was null.");
                return false;
            }

            if (closeRequestedByReturnButton && !lastInHistory.CanCloseByReturnButton) {
                Debug.Log("User tried to close a menu that cannot be closed by pressing the return button.");
                return false;
            }
            
			if (this.LastFrameAMenuClosed == Time.frameCount) {
				Debug.LogWarning($"Will not close menu '{lastInHistory.name}' because one already closed in this frame.", lastInHistory);
				return false;
			}

            this._navigationHistory.Remove(lastInHistory);
			
			this.CheckIfIsLastMenu();

            Debug.Log($"Closing Menu '{lastInHistory.name}'", lastInHistory);
            this.LastFrameAMenuClosed = Time.frameCount;
            return lastInHistory.Close();
        }

		public void EndNavigation() {
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
        
		private bool CheckIfIsFirstMenu() {
            this.RemoveNullFromNavigationHistory();
			if (this._navigationHistory.Count > 0) return false;

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

            return true;
        }
        
		private void CheckIfIsLastMenu() {
            if (this.RemoveNullFromNavigationHistory() && this._navigationHistory.Count <= 0) {
                // there was null UI on navigation history now it doesnt have anything, end navigation.
                this.EndNavigation();
                return;
            }
            if (this._navigationHistory.Count > 0) return;
            this.EndNavigation();
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

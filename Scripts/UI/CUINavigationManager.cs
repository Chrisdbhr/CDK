using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace CDK.UI {
	public class CUINavigationManager {

        #region <<---------- Singleton ---------->>

        public static CUINavigationManager get {
            get {
                if (CSingletonHelper.CannotCreateAnyInstance() || _instance != null) return _instance;
                return (_instance = new CUINavigationManager());
            }
        }
        private static CUINavigationManager _instance;

        #endregion <<---------- Singleton ---------->>

        
        
        
        
        #region <<---------- Initializers ---------->>

		public CUINavigationManager() {
			this._navigationHistory = new HashSet<CUIViewBase>();
			this._blockingEventsManager = CBlockingEventsManager.get;
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
        
		#if UNITY_ADDRESSABLES_EXIST
	    
		public T OpenMenu<T>(string uiReference, CUIViewBase originUI, CUIInteractable originButton, bool canCloseByReturnButton = true) {
			var openedMenu = this.OpenMenu(uiReference, originUI, originButton, canCloseByReturnButton);
			return openedMenu != null ? openedMenu.GetComponent<T>() : default;
		}
        
		/// <summary>
		/// Opens a menu, registering the button that opened it.
		/// </summary>
		/// <returns>returns the new opened menu.</returns>
		public CUIViewBase OpenMenu(string uiReference, CUIViewBase originUI, CUIInteractable originButton, bool canCloseByReturnButton = true) {
			if (CApplication.IsQuitting) return null;
            
            var uiGameObject = CAssets.LoadResourceAndInstantiate<CUIViewBase>(uiReference);
			if (uiGameObject == null) {
				Debug.LogError($"Could not open menu '{uiReference}'");
				return null;
			}

            return this.ShowSpawnedMenu(uiGameObject, originUI, originButton, canCloseByReturnButton);
		}
        
        public T OpenMenu<T>(CUIViewBase uiPrefab, CUIViewBase originUI, CUIInteractable originButton, bool canCloseByReturnButton = true) {
            var openedMenu = OpenMenu(uiPrefab, originUI, originButton, canCloseByReturnButton);
            return openedMenu.GetComponent<T>();
        }
        
        public CUIViewBase OpenMenu(CUIViewBase uiPrefab, CUIViewBase originUI, CUIInteractable originButton, bool canCloseByReturnButton = true) {
            if (CApplication.IsQuitting) return null;

            if (uiPrefab == null) {
                Debug.LogError("Tried to open menu uiPrefab that is null. Maybe the reference in inspector is null.", originUI);
                return null;
            }
            
            var uiGameObject = Object.Instantiate(uiPrefab);
            if (uiGameObject == null) {
                Debug.LogError($"Could not open menu '{uiPrefab}'");
                return null;
            }

            return this.ShowSpawnedMenu(uiGameObject, originUI, originButton, canCloseByReturnButton);
        }

        private CUIViewBase ShowSpawnedMenu(CUIViewBase ui, CUIViewBase originUI, CUIInteractable originButton, bool canCloseByReturnButton = true) {
            if (CApplication.IsQuitting) return null;
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
		public bool CloseLastMenu(bool closeRequestedByReturnButton = false) {
            this.RemoveNullFromNavigationHistory();
			if (this._navigationHistory.Count <= 0) {
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
            return lastInHistory.NavigationRequestedClose();
        }
        
		public void EndNavigation() {
			Debug.Log($"Requested EndNavigation of {this._navigationHistory.Count} Menus in history.");
            RemoveNullFromNavigationHistory();
			foreach (var ui in this._navigationHistory) {
				ui.NavigationRequestedClose();
			}
            this._disposableIsNavigating?.Dispose();
			this._navigationHistory.Clear();
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

			this._disposableIsNavigating?.Dispose();
            this._disposableIsNavigating = Observable.EveryUpdate().Subscribe(_ => {
				//if (CInputManager.ActiveInputType != CInputManager.InputType.JoystickController) return;
                this.RemoveNullFromNavigationHistory();
				if (this._navigationHistory.Count <= 0) return;
				var current = EventSystem.current;
				if (current == null) return;
                if (current.currentSelectedGameObject != null && current.currentSelectedGameObject.activeInHierarchy && current.currentSelectedGameObject.TryGetComponent<CUIInteractable>(out var _)) {
                    return;
                }
				var activeUi = this._navigationHistory.Last();
                var objectToSelect = activeUi.FirstSelectedObject;
                if (objectToSelect != null && objectToSelect.activeInHierarchy && objectToSelect.TryGetComponent<CUIInteractable>(out var interactable)) {
                    current.SetSelectedGameObject(interactable.gameObject);
                    return;
                }
                var firstInteractable = activeUi.GetComponentsInChildren<CUIInteractable>(false).FirstOrDefault();
                if (firstInteractable == null) {
                    Debug.LogError("Could not find valid UI element to set cursor selection!");
                    return;
                }
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
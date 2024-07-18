using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using Reflex.Attributes;
using Reflex.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace CDK.UI {
	public class CUINavigationManager {

        #region <<---------- Initializers ---------->>

		public CUINavigationManager(Container container) {
			_navigationHistory = new HashSet<CUIViewBase>();
			_blockingEventsManager = container.Resolve<CBlockingEventsManager>();
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		

		#region <<---------- Properties ---------->>
		public CUIViewBase[] NavigationHistoryToArray => this._navigationHistory.ToArray();
		
		int LastFrameAMenuClosed;
		IDisposable _disposableIsNavigating;

        readonly HashSet<CUIViewBase> _navigationHistory;
        readonly CBlockingEventsManager _blockingEventsManager;

		#endregion <<---------- Properties ---------->>
		
		
		
		
		#region <<---------- Open / Close ---------->>

		bool IsAlreadyOnNavigation(CUIViewBase view) {
			if (_navigationHistory.Any(m => m.name.Replace("(Clone)", "") == view.name)) {
				Debug.LogError($"Tried to open the same menu twice! Will not open menu '{view.name}'");
				return true;
			}
			return false;
		}

		/// <returns>The type of the opened menu using GetComponent</returns>
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

            if (IsAlreadyOnNavigation(uiPrefab)) return null;

            var uiGameObject = CAssets.CInstantiate(uiPrefab);
            if (uiGameObject == null) {
                Debug.LogError($"Could not open menu '{uiPrefab}'");
                return null;
            }

            return this.ShowSpawnedMenu(uiGameObject, originUI, originButton, canCloseByReturnButton);
        }

        CUIViewBase ShowSpawnedMenu(CUIViewBase ui, CUIViewBase originUI, CUIInteractable originButton, bool canCloseByReturnButton = true) {
            if (CApplication.IsQuitting) return null;

            bool alreadyOpened = this._navigationHistory.Any(x => x == ui);
            if (alreadyOpened) {

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

		/// <returns>true if removed some element</returns>
		bool RemoveNullFromNavigationHistory() {
            int removedCount = this._navigationHistory.RemoveWhere(x => x == null);
            if (removedCount > 0) {
                Debug.LogWarning($"<color=yellow>Removed {removedCount} null UI from navigation history.</color>");
                return true;
            }
            return false;
        }

		bool CheckIfIsFirstMenu() {
            this.RemoveNullFromNavigationHistory();
			if (this._navigationHistory.Count > 0) return false;

			this._disposableIsNavigating?.Dispose();
            this._disposableIsNavigating = Observable.EveryUpdate(UnityFrameProvider.PostLateUpdate).Subscribe(_ => {
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

		void CheckIfIsLastMenu() {
            if (this.RemoveNullFromNavigationHistory() && this._navigationHistory.Count <= 0) {
                // there was null UI on navigation history now it doesnt have anything, end navigation.
                this.EndNavigation();
                return;
            }
            if (this._navigationHistory.Count > 0) return;
            this.EndNavigation();
        }

		void HideLastMenuIfSet() {
            this.RemoveNullFromNavigationHistory();
			if (this._navigationHistory.Count <= 0) return;
			var current = this._navigationHistory.Last();
            current.HideIfSet(); 
		}
		
		#endregion <<---------- Navigation ---------->>
        
	}
}
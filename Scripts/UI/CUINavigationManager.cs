using System;
using System.Collections.Generic;
using System.Linq;

using Reflex.Attributes;
using Reflex.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace CDK.UI {
	public class CUINavigationManager : CMonoBehaviour {

        #region <<---------- Initializers ---------->>

		public CUINavigationManager Init(Container container) {
			_navigationHistory = new HashSet<CUIViewBase>();
			_blockingEventsManager = container.Resolve<CBlockingEventsManager>();
			return this;
		}
		
		#endregion <<---------- Initializers ---------->>



		#region MonoBehaviour

		protected override void Awake()
		{
			base.Awake();

		}

		void Update()
		{
			//if (CInputManager.ActiveInputType != CInputManager.InputType.JoystickController) return;
			RemoveNullFromNavigationHistory();
			if (_navigationHistory.Count <= 0) return;
			var current = EventSystem.current;
			if (current == null) return;
			if (current.currentSelectedGameObject != null && current.currentSelectedGameObject.activeInHierarchy && current.currentSelectedGameObject.TryGetComponent<CUIInteractable>(out var _)) {
				return;
			}
			var activeUi = _navigationHistory.Last();
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
		}

		#endregion MonoBehaviour
		
		

		#region <<---------- Properties ---------->>
		public CUIViewBase[] NavigationHistoryToArray => _navigationHistory.ToArray();
		
		[NonSerialized] int LastFrameAMenuClosed;
		[NonSerialized] IDisposable _disposableIsNavigating;

		[NonSerialized] HashSet<CUIViewBase> _navigationHistory;
		[NonSerialized] CBlockingEventsManager _blockingEventsManager;

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

            var uiGameObject = Object.Instantiate(uiPrefab);
            if (uiGameObject == null) {
                Debug.LogError($"Could not open menu '{uiPrefab}'");
                return null;
            }

            return ShowSpawnedMenu(uiGameObject, originUI, originButton, canCloseByReturnButton);
        }

        CUIViewBase ShowSpawnedMenu(CUIViewBase ui, CUIViewBase originUI, CUIInteractable originButton, bool canCloseByReturnButton = true) {
            if (CApplication.IsQuitting) return null;

            bool alreadyOpened = _navigationHistory.Any(x => x == ui);
            if (alreadyOpened) {

                CAssets.UnloadAsset(ui.gameObject);
                return null;
            }
			
            Debug.Log($"Requested navigation to '{ui.name}'");
            
            HideLastMenuIfSet();
			
            ui.Open(_navigationHistory.Count, originUI, originButton, canCloseByReturnButton);
			
            CheckIfIsFirstMenu();
			
            _navigationHistory.Add(ui);
            
            return ui;
        }
		
		/// <summary>
		/// Closes active menu selecting previous button.
		/// </summary>
		/// <returns>returns TRUE if the menu closed.</returns>
		public bool CloseLastMenu(bool closeRequestedByReturnButton = false) {
            RemoveNullFromNavigationHistory();
			if (_navigationHistory.Count <= 0) {
				return false;
			}

            var lastInHistory = _navigationHistory.Last();
            if (lastInHistory == null) {
                Debug.LogError("Last menu to close in navigation history was null.");
                return false;
            }

            if (closeRequestedByReturnButton && !lastInHistory.CanCloseByReturnButton) {
                Debug.Log("User tried to close a menu that cannot be closed by pressing the return button.");
                return false;
            }
            
			if (LastFrameAMenuClosed == Time.frameCount) {
				Debug.LogWarning($"Will not close menu '{lastInHistory.name}' because one already closed in this frame.", lastInHistory);
				return false;
			}

            _navigationHistory.Remove(lastInHistory);
			
			CheckIfIsLastMenu();

            Debug.Log($"Closing Menu '{lastInHistory.name}'", lastInHistory);
            LastFrameAMenuClosed = Time.frameCount;
            lastInHistory.NavigationRequestedClose();
            return true;
		}
        
		public void EndNavigation() {
			Debug.Log($"Requested EndNavigation of {_navigationHistory.Count} Menus in history.");
            RemoveNullFromNavigationHistory();
			foreach (var ui in _navigationHistory) {
				ui.NavigationRequestedClose();
			}
            _disposableIsNavigating?.Dispose();
			_navigationHistory.Clear();
            CTime.TimeScale = 1f;
		}
		
		#endregion <<---------- Open / Close ---------->>
        
        
		
		
		#region <<---------- Navigation ---------->>

		/// <returns>true if removed some element</returns>
		bool RemoveNullFromNavigationHistory() {
            int removedCount = _navigationHistory.RemoveWhere(x => x == null);
            if (removedCount > 0) {
                Debug.LogWarning($"<color=yellow>Removed {removedCount} null UI from navigation history.</color>");
                return true;
            }
            return false;
        }

		bool CheckIfIsFirstMenu() {
            RemoveNullFromNavigationHistory();
			return _navigationHistory.Count <= 0;
		}

		void CheckIfIsLastMenu() {
            if (RemoveNullFromNavigationHistory() && _navigationHistory.Count <= 0) {
                // there was null UI on navigation history now it doesnt have anything, end navigation.
                EndNavigation();
                return;
            }
            if (_navigationHistory.Count > 0) return;
            EndNavigation();
        }

		void HideLastMenuIfSet() {
            RemoveNullFromNavigationHistory();
			if (_navigationHistory.Count <= 0) return;
			var current = _navigationHistory.Last();
            current.HideIfSet(); 
		}
		
		#endregion <<---------- Navigation ---------->>
        
	}
}
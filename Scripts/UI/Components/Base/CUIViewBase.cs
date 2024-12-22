using System;
using System.Collections;

using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if FMOD
using FMODUnity;
#endif

namespace CDK.UI {
	public abstract class CUIViewBase : CMonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		public GameObject FirstSelectedObject => _eventSystem.firstSelectedGameObject;

		[Header("Setup")]
		[NonSerialized, GetComponentInChildren] protected EventSystem _eventSystem;
        protected CUIButton ButtonReturn => _buttonReturn;
        [SerializeField] CUIButton _buttonReturn;

        public bool ShouldPauseTheGame = true;

		[Obsolete("Trigger audio using OnOpen/OnClose unity events")]
		[NonSerialized] protected bool _shouldPlayOpenAndCloseMenuSound;

		public bool CanCloseByReturnButton => _canCloseByReturnButton;
        protected bool _canCloseByReturnButton = true;

		[NonSerialized] CUIViewBase _previousUI;
		[NonSerialized] CUIInteractable _previousButton;

		public event Action OnOpen {
			add {
				_onOpen -= value;
				_onOpen += value;
			}
			remove {
				_onOpen -= value;
			}
		}
		[NonSerialized] Action _onOpen;

		public event Action<CUIViewBase> OnClose {
			add {
				_onClose -= value;
				_onClose += value;
			}
			remove {
				_onClose -= value;
			}
		}
		[NonSerialized] Action<CUIViewBase> _onClose;

		[NonSerialized] protected UnityEvent OnOpenEvent;
		[NonSerialized] protected UnityEvent OnCloseEvent;

		[Inject][NonSerialized] protected CGameSettings _gameSettings;
		[Inject][NonSerialized] protected readonly CBlockingEventsManager _blockingEventsManager;
		[Inject][NonSerialized] protected CUINavigationManager _navigationManager;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void OnEnable() {
			UpdateEventSystemAndCheckForObjectSelection(_eventSystem.firstSelectedGameObject);
            if(_buttonReturn) _buttonReturn.OnClick += NavigationRequestedClose;
            _blockingEventsManager.OnMenuRetainable.Retain(this);
        }

		void LateUpdate()
		{
			if (_eventSystem == null || (_eventSystem.currentSelectedGameObject != null && _eventSystem.currentSelectedGameObject.GetComponent<CUIInteractable>() != null)) return;
			var toSelect = GetComponentInChildren<CUIInteractable>();
			if (toSelect == null) {
				Debug.LogError($"Could not find object to select with a '{nameof(CUIInteractable)}' in '{name}', this will lead to non functional UI on controllers.", this);
				return;
			}
			Debug.Log($"Auto selecting item '{toSelect.name}' on menu '{name}'", toSelect);
			_eventSystem.SetSelectedGameObject(toSelect.gameObject);
		}

		protected virtual void OnDisable() {
            _blockingEventsManager.OnMenuRetainable.Release(this);
            if(_buttonReturn) _buttonReturn.OnClick -= NavigationRequestedClose;
		}

        protected virtual void OnDestroy() { }

        protected virtual void Reset() {
            if (_eventSystem == null) {
                _eventSystem = GetComponentInChildren<EventSystem>();
            }
        }

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- Open / Close ---------->>

		public void Open(int sortOrder, CUIViewBase originUI, CUIInteractable originButton, bool canCloseByReturnButton = true) {
			Debug.Log($"Open UI {gameObject.name}");
			_previousUI = originUI;
			_previousButton = originButton;

            _canCloseByReturnButton = canCloseByReturnButton;

			_onOpen?.Invoke();

			UpdateCTime();

			OnOpenEvent?.Invoke();

			GetComponentInChildren<Canvas>(true).sortingOrder = sortOrder;

            gameObject.SetActive(true);
		}

        /// <summary>
        /// Close the menu.
        /// </summary>
        /// <returns>Returns TRUE if the menu closed without errors.</returns>
        public void NavigationRequestedClose() {
			Debug.Log($"Closing UI {gameObject.name}", this);
			_onClose?.Invoke(this);

			if (_previousUI != null) _previousUI.ShowIfHidden(_previousButton);

			OnCloseEvent?.Invoke();

			_blockingEventsManager.OnMenuRetainable.Release(this);

			#if UNITY_ADDRESSABLES_EXIST
			if (!CAssets.UnloadAsset(this.gameObject)) {
				Debug.LogError($"Error releasing instance of object '{this.gameObject.name}'", this);
			}
			#else
			gameObject.CDestroy();
			#endif
        }

		#endregion <<---------- Open / Close ---------->>




		#region <<---------- Visibility ---------->>

		void UpdateEventSystemAndCheckForObjectSelection(GameObject gameObjectToSelect) {
            if (gameObjectToSelect == null) return;
			UpdateEventSystemAndCheckForObjectSelection(gameObjectToSelect.GetComponent<CUIInteractable>());
        }


		void UpdateEventSystemAndCheckForObjectSelection(CUIInteractable interactableToSelect) {
            if (interactableToSelect == null) {
                Debug.LogError("Requested to select a null interactable! No object will be selected.");
                return;
            }

            EventSystem.current = _eventSystem;

            _eventSystem.SetSelectedGameObject(interactableToSelect.gameObject);
		}

		public void HideIfSet() {
			gameObject.SetActive(false);
		}

		public void ShowIfHidden(CUIInteractable buttonToSelect) {
			gameObject.SetActive(true);
			if(buttonToSelect) UpdateEventSystemAndCheckForObjectSelection(buttonToSelect);
			UpdateCTime();
		}

		#endregion <<---------- Visibility ---------->>




		#region <<---------- Time ---------->>

		void UpdateCTime() {
			CTime.TimeScale = ShouldPauseTheGame ? 0f : 1f;
		}
		
		#endregion <<---------- Time ---------->>
		
	}
}
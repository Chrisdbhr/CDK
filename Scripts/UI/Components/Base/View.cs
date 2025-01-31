using System;

using Reflex.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace CDK.UI {
	public abstract class View : CMonoBehaviour {

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

		[NonSerialized] View _previous;
		[NonSerialized] CUIInteractable _previousButton;

		public event Action<View> OpenEvent;
		public event Action<View> CloseEvent;

		[Inject][NonSerialized] protected UISoundsBankSO _soundsBank;
		[Inject][NonSerialized] protected readonly CBlockingEventsManager _blockingEventsManager;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void OnEnable() {
			UpdateEventSystemAndCheckForObjectSelection(_eventSystem.firstSelectedGameObject);
            if(_buttonReturn) _buttonReturn.ClickEvent += CloseView;
            _blockingEventsManager.MenuRetainable.Retain(this);
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
            _blockingEventsManager.MenuRetainable.Release(this);
            if(_buttonReturn) _buttonReturn.ClickEvent -= CloseView;
		}

        protected virtual void OnDestroy() { }

        protected virtual void Reset() {
            if (_eventSystem == null) {
                _eventSystem = GetComponentInChildren<EventSystem>();
            }
        }

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- Open / Close ---------->>

		public static View InstantiateAndOpen(View prefab, View previous = null, CUIInteractable originButton = null, bool canCloseByReturnButton = true)
		{
			var view = Object.Instantiate(prefab);
			view.Open(previous, originButton, canCloseByReturnButton);
			return view;
		}

		void Open(View previous, CUIInteractable originButton, bool canCloseByReturnButton = true) {
			Debug.Log($"Opening UI {gameObject.name}");
			_previous = previous;
			if(_previous != null) {
				GetComponentInChildren<Canvas>(true).sortingOrder = 1 + _previous.GetComponentInChildren<Canvas>(true).sortingOrder;
				_previous.gameObject.SetActive(false);
			}
			_previousButton = originButton;
            _canCloseByReturnButton = canCloseByReturnButton;
            CTime.TimeScale = ShouldPauseTheGame ? 0f : 1f;
			_blockingEventsManager.MenuRetainable.Retain(this);
			OpenEvent?.Invoke(this);
            gameObject.SetActive(true);
		}

		public void CloseView()
		{
			CloseInternal(false);
		}

		public void RecursiveCloseAllViews()
		{
			CloseInternal(true);
		}

        void CloseInternal(bool closeAll = false) {
			Debug.Log($"Closing UI {gameObject.name}", this);
			CloseEvent?.Invoke(this);
			if(_previous != null) {
				if(closeAll) _previous.RecursiveCloseAllViews();
				else _previous.ShowIfHidden(_previousButton);
			}
			else {
				CTime.TimeScale = 1f;
			}
			_blockingEventsManager.MenuRetainable.Release(this);
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

		public void ShowIfHidden(CUIInteractable buttonToSelect) {
			gameObject.SetActive(true);
			if(buttonToSelect) UpdateEventSystemAndCheckForObjectSelection(buttonToSelect);
			CTime.TimeScale = ShouldPauseTheGame ? 0f : 1f;
		}

		#endregion <<---------- Visibility ---------->>

	}
}
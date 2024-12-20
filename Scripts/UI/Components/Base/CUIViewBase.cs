using System;
using System.Collections;
using R3;
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

		public GameObject FirstSelectedObject => this._eventSystem.firstSelectedGameObject;
		
		[Header("Setup")]
		[NonSerialized, GetComponentInChildren] protected EventSystem _eventSystem;
        protected CUIButton ButtonReturn => _buttonReturn;
        [SerializeField] CUIButton _buttonReturn;

        public bool ShouldPauseTheGame = true;

		[Obsolete("Trigger audio using OnOpen/OnClose unity events")]
		[NonSerialized] protected bool _shouldPlayOpenAndCloseMenuSound;

		public bool CanCloseByReturnButton => this._canCloseByReturnButton;
        protected bool _canCloseByReturnButton = true;
		
		[NonSerialized] CUIViewBase _previousUI;
		[NonSerialized] CUIInteractable _previousButton;
		
		public event Action OnOpen {
			add {
				this._onOpen -= value;
				this._onOpen += value;
			}
			remove {
				this._onOpen -= value;
			}
		}
		[NonSerialized] Action _onOpen;
		
		public event Action<CUIViewBase> OnClose {
			add {
				this._onClose -= value;
				this._onClose += value;
			}
			remove {
				this._onClose -= value;
			}
		}
		[NonSerialized] Action<CUIViewBase> _onClose;

		[NonSerialized] protected UnityEvent OnOpenEvent;
		[NonSerialized] protected UnityEvent OnCloseEvent;

		[Inject][NonSerialized] protected CGameSettings _gameSettings;
		[Inject][NonSerialized] protected readonly CBlockingEventsManager _blockingEventsManager;
		[Inject][NonSerialized] protected CUINavigationManager _navigationManager;
		[NonSerialized] protected CompositeDisposable _disposeOnDisable = new ();

		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- MonoBehaviour ---------->>

		protected virtual void OnEnable() {
			this.UpdateEventSystemAndCheckForObjectSelection(this._eventSystem.firstSelectedGameObject);

            if(this._buttonReturn != null){
                this._buttonReturn.Button.OnClickAsObservable()
                    .Subscribe(_ => {
                        this._navigationManager.CloseLastMenu();
                    })
                    .AddTo(_disposeOnDisable);
            }
            
            this._blockingEventsManager.OnMenuRetainable.Retain(this);
        }

		void LateUpdate()
		{
			if (this._eventSystem == null || (this._eventSystem.currentSelectedGameObject != null && this._eventSystem.currentSelectedGameObject.GetComponent<CUIInteractable>() != null)) return;
			var toSelect = this.GetComponentInChildren<CUIInteractable>();
			if (toSelect == null) {
				Debug.LogError($"Could not find object to select with a '{nameof(CUIInteractable)}' in '{this.name}', this will lead to non functional UI on controllers.", this);
				return;
			}
			Debug.Log($"Auto selecting item '{toSelect.name}' on menu '{this.name}'", toSelect);
			this._eventSystem.SetSelectedGameObject(toSelect.gameObject);
		}

		protected virtual void OnDisable() {
			this._disposeOnDisable?.Dispose();
            this._blockingEventsManager.OnMenuRetainable.Release(this);
		}

        protected virtual void OnDestroy() { }

        protected virtual void Reset() {
            if (this._eventSystem == null) {
                this._eventSystem = this.GetComponentInChildren<EventSystem>();
            }
        }
		
		#endregion <<---------- MonoBehaviour ---------->>
		
		
		
		
		#region <<---------- Open / Close ---------->>

		public void Open(int sortOrder, CUIViewBase originUI, CUIInteractable originButton, bool canCloseByReturnButton = true) {
			Debug.Log($"Open UI {this.gameObject.name}");
			this._previousUI = originUI;
			this._previousButton = originButton;

            this._canCloseByReturnButton = canCloseByReturnButton;
            
			this._onOpen?.Invoke();

			this.UpdateCTime();

			OnOpenEvent?.Invoke();
            
			GetComponentInChildren<Canvas>(true).sortingOrder = sortOrder;

            this.gameObject.SetActive(true);
		}

        /// <summary>
        /// Close the menu.
        /// </summary>
        /// <returns>Returns TRUE if the menu closed without errors.</returns>
        public bool NavigationRequestedClose() {
			Debug.Log($"Closing UI {this.gameObject.name}", this);
			this._onClose?.Invoke(this);

			if (this._previousUI != null) this._previousUI.ShowIfHidden(this._previousButton);

			OnCloseEvent?.Invoke();

			this._blockingEventsManager.OnMenuRetainable.Release(this);
  
			#if UNITY_ADDRESSABLES_EXIST
			if (!CAssets.UnloadAsset(this.gameObject)) {
				Debug.LogError($"Error releasing instance of object '{this.gameObject.name}'", this);
			}
			#else
			this.gameObject.CDestroy();
			#endif
            return true;
        }
		
		#endregion <<---------- Open / Close ---------->>




		#region <<---------- Visibility ---------->>

        private void UpdateEventSystemAndCheckForObjectSelection(GameObject gameObjectToSelect) {
            if (gameObjectToSelect == null) return;
			this.UpdateEventSystemAndCheckForObjectSelection(gameObjectToSelect.GetComponent<CUIInteractable>());
        }


        private void UpdateEventSystemAndCheckForObjectSelection(CUIInteractable interactableToSelect) {
            if (interactableToSelect == null) {
                Debug.LogError("Requested to select a null interactable! No object will be selected.");
                return;
            }
            
            EventSystem.current = this._eventSystem;

            this._eventSystem.SetSelectedGameObject(interactableToSelect.gameObject);
		}
		
		public void HideIfSet() {
			this.gameObject.SetActive(false);
		}

		public void ShowIfHidden(CUIInteractable buttonToSelect) {
			this.gameObject.SetActive(true);
			if(buttonToSelect) this.UpdateEventSystemAndCheckForObjectSelection(buttonToSelect);
			UpdateCTime();
		}
		
		#endregion <<---------- Visibility ---------->>




		#region <<---------- Time ---------->>

		private void UpdateCTime() {
			CTime.TimeScale = this.ShouldPauseTheGame ? 0f : 1f;
		}
		
		#endregion <<---------- Time ---------->>
		
	}
}
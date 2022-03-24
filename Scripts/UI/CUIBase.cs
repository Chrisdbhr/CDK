using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if FMOD
using FMODUnity;
#endif

#if UnityAddressables
using UnityEngine.AddressableAssets;
#endif

namespace CDK.UI {
	public abstract class CUIBase : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		public GameObject FirstSelectedObject => this._eventSystem.firstSelectedGameObject;
		
		[Header("Setup")]
		[SerializeField] protected EventSystem _eventSystem;

		public bool ShouldPauseTheGame = true;
		
		private CUIBase _previousUI;
		private CUIInteractable _previousButton;
		private Canvas _canvas;
		
		public event Action OnOpen {
			add {
				this._onOpen -= value;
				this._onOpen += value;
			}
			remove {
				this._onOpen -= value;
			}
		}
		private Action _onOpen;
		
		public event Action<CUIBase> OnClose {
			add {
				this._onClose -= value;
				this._onClose += value;
			}
			remove {
				this._onClose -= value;
			}
		}
		private Action<CUIBase> _onClose;

		protected CGameSettings _gameSettings;
		protected CFader _fader;
		protected CBlockingEventsManager _blockingEventsManager;

		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			this._gameSettings = CDependencyResolver.Get<CGameSettings>();
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
			this._canvas = this.GetComponent<Canvas>();
			this._fader = CDependencyResolver.Get<CFader>();
		}

		protected virtual void Start() {
			
		}
		
		protected virtual void OnEnable() {
			this.UpdateEventSystemAndCheckForObjectSelection(this._eventSystem.firstSelectedGameObject);
		}
		
		protected virtual void OnDisable() {
			
		}

        protected virtual void Reset() {
            if (this._eventSystem == null) {
                this._eventSystem = this.GetComponentInChildren<EventSystem>();
            }
        }
		
		#endregion <<---------- MonoBehaviour ---------->>
		
		
		
		
		#region <<---------- Open / Close ---------->>

		public async Task Open(int sortOrder, CUIBase originUI, CUIInteractable originButton) {
			Debug.Log($"Open UI {this.gameObject.name}");
			this._previousUI = originUI;
			this._previousButton = originButton;

			this._onOpen?.Invoke();

			this._canvas.sortingOrder = sortOrder;
			
			#if FMOD
			RuntimeManager.PlayOneShot(this._gameSettings.SoundOpenMenu);
			#else
			Debug.LogError("'Play Open menu sound' not implemented without FMOD");
			#endif

			UpdateCTime();
		}
		public void Close() {
			Debug.Log($"Closing UI {this.gameObject.name}", this);
			this._onClose?.Invoke(this);

			if (this._previousUI != null) this._previousUI.ShowIfHidden(this._previousButton);

			#if UnityAddressables
			if (!CAssets.UnloadAsset(this.gameObject)) {
				Debug.LogError($"Error releasing instance of object '{this.gameObject.name}'", this);
			}
			#else
			this.gameObject.CDestroy();
			#endif
			
			#if FMOD
			RuntimeManager.PlayOneShot(this._gameSettings.SoundCloseMenu);
			#else
			Debug.LogError("'Play Close menu sound' not implemented without FMOD");
			#endif
		}
		
		#endregion <<---------- Open / Close ---------->>




		#region <<---------- Visibility ---------->>

		private void UpdateEventSystemAndCheckForObjectSelection(GameObject gameObjectToSelect) {
			EventSystem.current = this._eventSystem;

			if (CInputManager.ActiveInputType != CInputManager.InputType.JoystickController) return;
			
			// get interactable to auto select
			GameObject toSelect = null;
			if (gameObjectToSelect != null && gameObjectToSelect.activeInHierarchy) {
				toSelect = gameObjectToSelect;
			}
			else if (this.FirstSelectedObject.activeInHierarchy) {
				toSelect = this.FirstSelectedObject;
			}else {
				Debug.LogWarning($"Could not select default object on event system, will try to find a CUIInteractable.");
				var interactable = FindObjectOfType<CUIInteractable>();
				toSelect = interactable.gameObject;
			}
			this._eventSystem.SetSelectedGameObject(null);
			
			SelectGameObjectAsync(toSelect).CAwait();
		}
		
		public async Task SelectGameObjectAsync(GameObject toSelect) {
			if (toSelect == null) return;
			await Observable.NextFrame();
			var selectable = toSelect.GetComponent<Selectable>();
			if (selectable != null) selectable.Select();
			await Observable.TimerFrame(1);
			if (this._eventSystem != null && toSelect != null) this._eventSystem.SetSelectedGameObject(toSelect);
		}

		public void HideIfSet() {
			this.gameObject.SetActive(false);
		}

		public void ShowIfHidden(CUIInteractable buttonToSelect) {
			this.gameObject.SetActive(true);
			if(buttonToSelect) this.UpdateEventSystemAndCheckForObjectSelection(buttonToSelect.gameObject);
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

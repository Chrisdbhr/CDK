using System;
using System.Threading.Tasks;
using FMODUnity;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDK.UI {
	public abstract class CUIBase : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		public GameObject FirstSelectedObject => this._eventSystem.firstSelectedGameObject;
		
		[Header("Setup")]
		[SerializeField] protected EventSystem _eventSystem;
		
		[NonSerialized] private CUIBase _previousUI;
		[NonSerialized] private CUIInteractable _previousButton;
		[NonSerialized] private Canvas _canvas;

		public event Action OnOpen {
			add {
				this._onOpen -= value;
				this._onOpen += value;
			}
			remove {
				this._onOpen -= value;
			}
		}
		[NonSerialized] private Action _onOpen;
		
		public event Action<CUIBase> OnClose {
			add {
				this._onClose -= value;
				this._onClose += value;
			}
			remove {
				this._onClose -= value;
			}
		}
		[NonSerialized] private Action<CUIBase> _onClose;

		[NonSerialized] protected CGameSettings _gameSettings;
		[NonSerialized] protected CFader _fader;

		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			this._canvas = this.GetComponent<Canvas>();
			this._gameSettings = CDependencyResolver.Get<CGameSettings>();
			this._fader = CDependencyResolver.Get<CFader>();
		}

		protected virtual void Start() {
			
		}
		
		protected virtual void OnEnable() {
			this.UpdateEventSystemAndCheckForObjectSelection(this._eventSystem.firstSelectedGameObject);
		}
		
		protected virtual void OnDisable() {
			
		}
		
		#endregion <<---------- MonoBehaviour ---------->>
		
		
		
		
		#region <<---------- Open / Close ---------->>

		public async Task Open(int sortOrder, CUIBase originUI, CUIInteractable originButton) {
			Debug.Log($"Open UI {this.gameObject.name}");
			this._previousUI = originUI;
			this._previousButton = originButton;

			this._onOpen?.Invoke();

			this._canvas.sortingOrder = sortOrder;
			RuntimeManager.PlayOneShot(this._gameSettings.SoundOpenMenu);
		}
		public void Close() {
			Debug.Log($"Closing UI {this.gameObject.name}", this);
			this._onClose?.Invoke(this);

			if (this._previousUI != null) this._previousUI.ShowIfHidden(this._previousButton);

			if (!Addressables.ReleaseInstance(this.gameObject)) {
				Debug.LogError($"Error releasing instance of object '{this.gameObject.name}'", this);
			}
			RuntimeManager.PlayOneShot(this._gameSettings.SoundCloseMenu);

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
		}
		
		#endregion <<---------- Visibility ---------->>

	}
}

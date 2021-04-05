using System;
using System.Threading.Tasks;
using FMODUnity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

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

		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			this._canvas = this.GetComponent<Canvas>();
		}

		protected virtual void OnEnable() {
			this.UpdateEventSystem(this._eventSystem.firstSelectedGameObject);
		}
		
		protected virtual void OnDisable() {
			
		}
		
		#endregion <<---------- MonoBehaviour ---------->>
		
		
		
		
		#region <<---------- Open / Close ---------->>

		public async Task Open(int sortOrder, CUIBase originUI, CUIInteractable originButton) {
			this._previousUI = originUI;
			this._previousButton = originButton;

			this._onOpen?.Invoke();

			this._canvas.sortingOrder = sortOrder;
			RuntimeManager.PlayOneShot(CGameSettings.SoundOpenMenu);
		}
		public async Task Close() {
			this._onClose?.Invoke(this);

			if (this._previousUI != null) this._previousUI.ShowIfHidden(this._previousButton);

			RuntimeManager.PlayOneShot(CGameSettings.SoundCloseMenu);
			Addressables.ReleaseInstance(this.gameObject);
		}
		
		#endregion <<---------- Open / Close ---------->>




		#region <<---------- Visibility ---------->>

		private void UpdateEventSystem(GameObject gameObjectToSelect) {
			EventSystem.current = this._eventSystem;
			this._eventSystem.SetSelectedGameObject(gameObjectToSelect ? gameObjectToSelect : this.FirstSelectedObject);
		}
		
		public void HideIfSet() {
			this.gameObject.SetActive(false);
		}

		public void ShowIfHidden(CUIInteractable buttonToSelect) {
			this.gameObject.SetActive(true);
			this.UpdateEventSystem(this._eventSystem.firstSelectedGameObject); //buttonToSelect.gameObject);
		}
		
		#endregion <<---------- Visibility ---------->>

	}
}

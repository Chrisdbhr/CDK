using System;
using System.Threading.Tasks;
using FMODUnity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

namespace CDK.UI {
	public abstract class CUIBase : MonoBehaviour {

		#region <<---------- Singleton ---------->>
		
		public static CUIBase ActiveUI {
			get {
				return _activeUI;
			}
			set {
				Debug.Log($"Setting Active UI from '{(_activeUI != null ? _activeUI.name : null)}' to '{(value != null ? value.name : null)}'");
				_activeUI = value;
			}
		}
		private static CUIBase _activeUI;

		#endregion <<---------- Singleton ---------->>

		
		
		
		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private GameObject _firstSelected;
		[SerializeField] [EventRef] protected string _soundOpenMenu;
		[SerializeField] [EventRef] protected string _soundCloseMenu;
		
		[NonSerialized] private CUIBase _previousUI;
		[NonSerialized] private CUIButton _previousButton;
		[NonSerialized] private EventSystem _currentEventSystem;

		#endregion <<---------- Properties and Fields ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>
		protected virtual void Start() {
			this._currentEventSystem = EventSystem.current;
			if(this._currentEventSystem != null && this._firstSelected != null) this._currentEventSystem.SetSelectedGameObject(this._firstSelected);
		}
		
		protected virtual void Update() {
			this.CheckForSelection();
		}
		#endregion <<---------- MonoBehaviour ---------->>

		
		
		private void CheckForSelection() {
			if (ActiveUI != this) return;
			this._currentEventSystem = EventSystem.current;
			if (this._currentEventSystem == null) return;
			if (this._currentEventSystem.currentSelectedGameObject != null) return;
			if (this._firstSelected == null) return;
			Debug.Log($"Setting selected object to {this._firstSelected.name} because there was any another selected on Event System {this._currentEventSystem.name}");
			this._currentEventSystem.SetSelectedGameObject(this._firstSelected);
		}
		
		
		
		
		#region <<---------- Open / Close ---------->>
		
		/// <summary>
		/// Opens a menu, registering the button that opened it and returning the opened menu if opened.
		/// </summary>
		public virtual async Task<CUIBase> OpenMenu(CUIButton previousButton) {
			Debug.Log($"{this.name} received a OpenMenu request");
			if (previousButton == null) {
				CTime.SetTimeScale(0f);
			}
			else {
				this._previousButton = previousButton;
			}
			RuntimeManager.PlayOneShot(this._soundOpenMenu);
			
			this._previousUI = ActiveUI;
			ActiveUI = this;
			
			return this;
		}

		/// <summary>
		/// Closes a menu selecting the button that opened it if possible.
		/// </summary>
		/// <returns></returns>
		public virtual async Task CloseMenu() {			
			Debug.Log($"{this.name} received a CloseMenu request");
			if (this._previousButton != null) {
				var es = EventSystem.current;
				if (es) {
					es.SetSelectedGameObject(this._previousButton.gameObject);
				}
			}
			else {
				// this is the last menu in navigation
				CTime.SetTimeScale(1f);
				CBlockingEventsManager.IsOnMenu = false;
			}
			
			RuntimeManager.PlayOneShot(this._soundCloseMenu);
			
			ActiveUI = this._previousUI;

			Addressables.ReleaseInstance(this.gameObject);
			Destroy(this.gameObject);
		}
		
		#endregion <<---------- Open / Close ---------->>
	
	}
}

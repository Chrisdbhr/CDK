using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if FMOD
using FMOD.Studio;
using STOP_MODE = FMOD.Studio.STOP_MODE;
#endif

namespace CDK.UI {
	public class CUIInteractable : MonoBehaviour, ISelectHandler, ISubmitHandler, ICancelHandler,
	                               IPointerEnterHandler, IPointerClickHandler {

		[SerializeField] private bool _debug;
		
		#if FMOD
		private EventInstance _soundEventInstance;
		#endif
        private CGameSettings _gameSettings;
        protected CUINavigationManager _navigationManager;

		private void Awake() {
			this._gameSettings = CDependencyResolver.Get<CGameSettings>();
            this._navigationManager = CDependencyResolver.Get<CUINavigationManager>();
        }

		private void PlaySound(EventReference sound) {
			#if FMOD
			if (!sound.IsNull) {
				this._soundEventInstance.stop(STOP_MODE.IMMEDIATE);
				this._soundEventInstance = FMODUnity.RuntimeManager.CreateInstance(sound);
				this._soundEventInstance.start();
			}
			#else
			throw new NotImplementedException();
			#endif
		}

		public virtual void Selected(bool playSound = true) {
			if(this._debug) Debug.Log($"Selected: CUIInteractable '{this.gameObject.name}'", this);
			if(playSound) this.PlaySound(this._gameSettings.SoundSelect);
		}

		public virtual void Submited() {
			if(this._debug) Debug.Log($"SUBMIT: CUIInteractable '{this.gameObject.name}'", this);
			this.PlaySound(this._gameSettings.SoundSubmit);
		}

		public virtual void Canceled() {
			if(this._debug) Debug.Log($"CANCEL: CUIInteractable '{this.gameObject.name}'", this);
			this.PlaySound(this._gameSettings.SoundCancel);
		}
		
		#region <<---------- IHandlers ---------->>
		
		public void OnSelect(BaseEventData eventData) {
			if (!this.gameObject.activeInHierarchy) return;
			this.Selected();
		}

		public void OnSubmit(BaseEventData eventData) {
			if (!this.gameObject.activeInHierarchy) return;
			this.Submited();
		}
		
		public void OnCancel(BaseEventData eventData) {
			if (!this.gameObject.activeInHierarchy) return;
            this.Canceled();
            this._navigationManager.CloseCurrentMenu().CAwait();
		}

		// Pointer
		public void OnPointerEnter(PointerEventData eventData) {
			if (!this.gameObject.activeInHierarchy) return;
			var selectable = this.GetComponent<Selectable>();
			if (selectable == null) return;
			selectable.Select();
			this.Selected(false);
		}
		
		public void OnPointerClick(PointerEventData eventData) {
			if (!this.gameObject.activeInHierarchy) return;
			this.Submited();
		}
		
		#endregion <<---------- IHandlers ---------->>

		

	}
}

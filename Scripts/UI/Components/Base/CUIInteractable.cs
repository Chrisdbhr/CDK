using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if FMOD
using FMODUnity;
using FMOD.Studio;
using STOP_MODE = FMOD.Studio.STOP_MODE;
#endif

namespace CDK.UI {
	public class CUIInteractable : MonoBehaviour, ISelectHandler, ISubmitHandler, ICancelHandler,
	                               IPointerEnterHandler, IPointerClickHandler, IDeselectHandler {
       
        #region <<---------- Properties and Fields ---------->>

        [SerializeField] private bool _debug;
        [SerializeField] private bool _playInteractionSound = true;
        
		#if FMOD
        private EventInstance _soundEventInstance;
		#endif
        private CGameSettings _gameSettings;
        protected CUINavigationManager _navigationManager;
       
        protected readonly CompositeDisposable _disposeOnDisable = new CompositeDisposable();
        protected readonly CompositeDisposable _disposeOnDestroy = new CompositeDisposable();

        #endregion <<---------- Properties and Fields ---------->>


        
        
        #region <<---------- Mono Behaviour ---------->>

        protected virtual void Awake() {
            this._gameSettings = CDependencyResolver.Get<CGameSettings>();
            this._navigationManager = CDependencyResolver.Get<CUINavigationManager>();
        }

        protected virtual void OnDisable() {
            this._disposeOnDisable?.Dispose();
        }

        protected virtual void OnDestroy() {
            this._disposeOnDestroy?.Dispose();
        }

        #endregion <<---------- Mono Behaviour ---------->>


		#if FMOD
		private void PlaySound(EventReference sound) {
			if (this._playInteractionSound && !sound.IsNull) {
				this._soundEventInstance.stop(STOP_MODE.IMMEDIATE);
				this._soundEventInstance = FMODUnity.RuntimeManager.CreateInstance(sound);
				this._soundEventInstance.start();
			}
		}
		#endif

		public virtual void Selected(bool playSound = true) {
			if(this._debug) Debug.Log($"Selected: CUIInteractable '{this.gameObject.name}'", this);
			#if FMOD
			if(playSound) this.PlaySound(this._gameSettings.SoundSelect);	
			#endif
		}

		public virtual void Submited() {
			if(this._debug) Debug.Log($"SUBMIT: CUIInteractable '{this.gameObject.name}'", this);
			#if FMOD
            if (!(this is CUIButton b && !b.Button.interactable)) {
                this.PlaySound(this._gameSettings.SoundSubmit);
            }
			#endif
		}

		public virtual void Canceled() {
			if(this._debug) Debug.Log($"CANCEL: CUIInteractable '{this.gameObject.name}'", this);
			#if FMOD
			this.PlaySound(this._gameSettings.SoundCancel);
			#endif
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
            if (this._navigationManager.CloseLastMenu(true)) {
                this.Canceled();
            }
        }

		// Pointer
		public virtual void OnPointerEnter(PointerEventData eventData) {
			if (!this.gameObject.activeInHierarchy) return;
			var selectable = this.GetComponent<Selectable>();
			if (selectable == null) return;
			selectable.Select();
			this.Selected(false);
		}
        
        public virtual void OnDeselect(BaseEventData eventData) {
            
        }
		
		public void OnPointerClick(PointerEventData eventData) {
			if (!this.gameObject.activeInHierarchy) return;
            if (eventData.button == PointerEventData.InputButton.Right) return;
            this.Submited();
		}
		
		#endregion <<---------- IHandlers ---------->>
        
    }
}

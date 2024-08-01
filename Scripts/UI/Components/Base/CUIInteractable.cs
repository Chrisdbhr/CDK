using System;
using R3;
using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
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

        [SerializeField] bool _debug;
        [SerializeField] bool _playInteractionSound = true;
        
		#if FMOD
        private EventInstance _soundEventInstance;
		#endif

        protected readonly CompositeDisposable _disposeOnDisable = new CompositeDisposable();
        protected readonly CompositeDisposable _disposeOnDestroy = new CompositeDisposable();
        [Inject] protected CUINavigationManager _navigationManager;
        [Inject] protected CGameSettings _gameSettings;

        #endregion <<---------- Properties and Fields ---------->>


        
        
        #region <<---------- Mono Behaviour ---------->>

        protected virtual void Awake() {
	        gameObject.Inject();
        }

        protected virtual void OnEnable() { }
        
        protected virtual void OnDisable() {
            this._disposeOnDisable?.Dispose();
        }

        protected virtual void OnDestroy() {
            this._disposeOnDestroy?.Dispose();
        }

        #endregion <<---------- Mono Behaviour ---------->>


		#if FMOD
		private void PlaySound(EventReference sound) {
			try {
				if (this._playInteractionSound && !sound.IsNull) {
					this._soundEventInstance.stop(STOP_MODE.IMMEDIATE);
					this._soundEventInstance = FMODUnity.RuntimeManager.CreateInstance(sound);
					this._soundEventInstance.start();
				}
			}
			catch (Exception e) {
				Debug.LogError($"Error trying to PlaySound '{sound}': {e.Message}\n{e}");
			}
		}
		#endif

		public virtual void Selected(bool playSound = true) {
			if(this._debug) Debug.Log($"Selected: CUIInteractable '{this.gameObject.name}'", this);
			#if FMOD
			if(playSound) this.PlaySound(_gameSettings.SoundSelect);
			#endif
		}

		public virtual void Submited() {
			if(this._debug) Debug.Log($"SUBMIT: CUIInteractable '{this.gameObject.name}'", this);
			#if FMOD
            if (!(this is CUIButton b && !b.Button.interactable)) {
                this.PlaySound(_gameSettings.SoundSubmit);
            }
			#endif
		}

		public virtual void Canceled() {
			if(this._debug) Debug.Log($"CANCEL: CUIInteractable '{this.gameObject.name}'", this);
			#if FMOD
			this.PlaySound(_gameSettings.SoundCancel);
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
            if (_navigationManager.CloseLastMenu(true)) {
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

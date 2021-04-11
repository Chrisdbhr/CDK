using System;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace CDK.UI {
	public class CUIInteractable : MonoBehaviour, ISelectHandler, ISubmitHandler, ICancelHandler,
	                               IPointerEnterHandler, IPointerClickHandler {

		[NonSerialized] private EventInstance _soundEventInstance;

		
		
		
		private void PlaySound(string sound) {
			if (!sound.CIsNullOrEmpty()) {
				this._soundEventInstance.stop(STOP_MODE.IMMEDIATE);
				this._soundEventInstance = FMODUnity.RuntimeManager.CreateInstance(sound);
				this._soundEventInstance.start();
			}
		}

		public virtual void Selected() {
			Debug.Log($"Selected: CUIInteractable '{this.gameObject.name}'", this);
			this.PlaySound(CGameSettings.SoundSelect);
		}

		public virtual void Submited() {
			Debug.Log($"SUBMIT: CUIInteractable '{this.gameObject.name}'", this);
			this.PlaySound(CGameSettings.SoundSubmit);
		}

		public virtual void Canceled() {
			Debug.Log($"CANCEL: CUIInteractable '{this.gameObject.name}'", this);
			this.PlaySound(CGameSettings.SoundCancel);
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
			CUINavigation.get.CloseCurrentMenu().CAwait();
			this.Canceled();
		}

		// Pointer
		public void OnPointerEnter(PointerEventData eventData) {
			if (!this.gameObject.activeInHierarchy) return;
			var selectable = this.GetComponent<Selectable>();
			if (selectable == null) return;
			selectable.Select();
			this.Selected();
		}
		
		public void OnPointerClick(PointerEventData eventData) {
			if (!this.gameObject.activeInHierarchy) return;
			this.Submited();
		}
		
		#endregion <<---------- IHandlers ---------->>

		

	}
}

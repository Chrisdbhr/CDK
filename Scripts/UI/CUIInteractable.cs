using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace CDK.UI {
	public class CUIInteractable : MonoBehaviour, ISelectHandler, ISubmitHandler, ICancelHandler {
		
		
		[Header("Default Sounds")]
		[SerializeField] [EventRef] protected string _soundSelect;
		[SerializeField] [EventRef] protected string _soundSubmit;
		[SerializeField] [EventRef] protected string _soundCancel;
		[NonSerialized] private EventInstance _soundEventInstance;

		
		
		
		private void PlaySound(string sound) {
			if (!sound.CIsNullOrEmpty()) {
				this._soundEventInstance.stop(STOP_MODE.IMMEDIATE);
				this._soundEventInstance = FMODUnity.RuntimeManager.CreateInstance(sound);
				this._soundEventInstance.start();
			}
		}
		
		#region <<---------- IHandlers ---------->>
		
		public void OnSelect(BaseEventData eventData) {
			this.PlaySound(this._soundSelect);
		}

		public void OnSubmit(BaseEventData eventData) {
			this.PlaySound(this._soundSubmit);
		}
		
		public void OnCancel(BaseEventData eventData) {
			CUINavigation.get.CloseCurrentMenu().CAwait();
		}
		
		#endregion <<---------- IHandlers ---------->>


	}
}

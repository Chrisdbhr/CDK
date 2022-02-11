using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CDK.UI {
	public class CUIConfirmPopup : CUIBase {
		
		[Header("Confirmation")]
		[SerializeField] private CUIButton _buttonConfirm;
		[SerializeField] private CUIButton _buttonCancel;

		
		public void SetupPopup(Action onConfirm) {
			
			// confirm exit
			this._buttonConfirm.Button.interactable = true;
			this._buttonConfirm.Button.OnClickAsObservable().Subscribe(async _ => {
				onConfirm?.Invoke();
				Debug.Log($"SUBMIT: Confirm Popup '{this.gameObject.name}'", this);
				await CUINavigation.get.CloseCurrentMenu();
			});
					
			// cancel exit
			this._buttonCancel.Button.interactable = true;
			this._buttonCancel.Button.OnClickAsObservable().Subscribe(async _ => {
				Debug.Log($"CANCEL: Confirm Popup '{this.gameObject.name}'", this);
				await CUINavigation.get.CloseCurrentMenu();
			});
					
			EventSystem.current.SetSelectedGameObject(this._buttonConfirm.gameObject);
		}

	}
}

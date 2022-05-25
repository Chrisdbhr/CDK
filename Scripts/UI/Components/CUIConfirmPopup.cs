using System;
using UniRx;
using UnityEngine;

namespace CDK.UI {
	public class CUIConfirmPopup : CUIViewBase {
		
		[Header("Confirmation")]
		[SerializeField] private CUIButton _buttonConfirm;
		[SerializeField] private CUIButton _buttonCancel;

		
		public void SetupPopup(Action onConfirm) {
			
			// confirm exit
			this._buttonConfirm.Button.interactable = true;
			this._buttonConfirm.Button.OnClickAsObservable().Subscribe(_ => {
				onConfirm?.Invoke();
				Debug.Log($"SUBMIT: Confirm Popup '{this.gameObject.name}'", this);
				this._navigationManager.CloseCurrentMenu();
			});
					
			// cancel exit
			this._buttonCancel.Button.interactable = true;
			this._buttonCancel.Button.OnClickAsObservable().Subscribe(_ => {
				Debug.Log($"CANCEL: Confirm Popup '{this.gameObject.name}'", this);
				this._navigationManager.CloseCurrentMenu();
			});
					
			this._eventSystem.SetSelectedGameObject(this._buttonConfirm.gameObject);
		}

	}
}

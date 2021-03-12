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
				await this.CloseMenu();
			});
					
			// cancel exit
			this._buttonCancel.Button.interactable = true;
			this._buttonCancel.Button.OnClickAsObservable().Subscribe(async _ => {
				await this.CloseMenu();
			});
					
			EventSystem.current.SetSelectedGameObject(this._buttonConfirm.gameObject);
		}

	}
}

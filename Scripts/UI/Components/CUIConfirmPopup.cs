using System;
using TMPro;
using R3;
using UnityEngine;

namespace CDK.UI {
	public class CUIConfirmPopup : CUIViewBase {
		
		[Header("Confirmation")]
		[SerializeField] private CUIButton _buttonConfirm;
        [SerializeField] private TextMeshProUGUI _title;
        

		
		public void SetupPopup(Action onConfirm, string title) {
			// confirm exit
			this._buttonConfirm.Button.interactable = true;
			this._buttonConfirm.Button.OnClickAsObservable().Subscribe(_ => {
				onConfirm?.Invoke();
				Debug.Log($"SUBMIT: Confirm Popup '{this.gameObject.name}'", this);
				this._navigationManager.CloseLastMenu();
			});
					
			this._eventSystem.SetSelectedGameObject(this._buttonConfirm.gameObject);
            
            _title.text = title;
		}

	}
}

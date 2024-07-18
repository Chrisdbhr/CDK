using System;
using TMPro;
using R3;
using UnityEngine;

namespace CDK.UI {
	public class CUIConfirmPopup : CUIViewBase {
		
		[Header("Confirmation")]
		[SerializeField] CUIButton _buttonConfirm;
        [SerializeField] TextMeshProUGUI _title;
        

		
		public void SetupPopup(EventHandler onConfirm, string title) {
			// confirm exit
			this._buttonConfirm.Button.interactable = true;
			this._buttonConfirm.Button.OnClickAsObservable().Subscribe(_ => {
				Debug.Log($"SUBMIT: Confirm Popup '{this.gameObject.name}'", this);
				this._navigationManager.CloseLastMenu();
				onConfirm?.Invoke(this, EventArgs.Empty);
			}).AddTo(this);
					
			this._eventSystem.SetSelectedGameObject(this._buttonConfirm.gameObject);
            
            _title.text = title;
		}

	}
}

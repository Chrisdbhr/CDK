using System;
using TMPro;

using UnityEngine;

namespace CDK.UI {
	public class CUIConfirmPopup : CUIViewBase {
		
		[Header("Confirmation")]
		[SerializeField] CUIButton _buttonConfirm;
        [SerializeField] TextMeshProUGUI _title;
        

		
		public void SetupPopup(EventHandler onConfirm, string title) {
			// confirm exit
			_buttonConfirm.Button.interactable = true;
			_buttonConfirm.Button.OnClickAsObservable().Subscribe(_ => {
				Debug.Log($"SUBMIT: Confirm Popup '{gameObject.name}'", this);
				_navigationManager.CloseLastMenu();
				onConfirm?.Invoke(this, EventArgs.Empty);
			}).AddTo(this);
					
			_eventSystem.SetSelectedGameObject(_buttonConfirm.gameObject);
            
            _title.text = title;
		}

	}
}

using System;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK.UI {
	[RequireComponent(typeof(Button))]
	public class CUIButton : CUIInteractable {
		public Button Button;

		public event Action OnClick = delegate { };

		protected override void OnEnable()
		{
			base.OnEnable();
			Button.onClick.AddListener(ButtonOnClick);
		}

		protected virtual void OnDisable()
		{
			Button.onClick.RemoveListener(ButtonOnClick);
		}

		void ButtonOnClick()
		{
			OnClick?.Invoke();
		}

		#if UNITY_EDITOR
        void Reset() {
            if (Button == null && TryGetComponent(out Button)) {
                EditorUtility.SetDirty(Button);
            }
		}
        #endif
	}
	
}
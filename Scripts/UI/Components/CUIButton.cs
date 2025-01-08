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

		public bool Interactable {
			get => Button.interactable;
			set => Button.interactable = value;
		}

		public event Action ClickEvent = delegate { };

		protected override void Awake()
		{
			base.Awake();
			Button.CAssertIfNull("Button in a UIButton is null!");
		}

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
			ClickEvent?.Invoke();
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
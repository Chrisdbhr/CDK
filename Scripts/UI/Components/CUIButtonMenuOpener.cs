
using UnityEngine;

namespace CDK.UI {
    public class CUIButtonMenuOpener : CUIButton {

        [SerializeField] View _menuToOpen;


        protected override void OnEnable()
        {
            base.OnEnable();
            ClickEvent += OnOnClick;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ClickEvent -= OnOnClick;
        }

        void OnOnClick()
        {
            View.InstantiateAndOpen(_menuToOpen, GetComponentInParent<View>(), this);
        }
    }
}
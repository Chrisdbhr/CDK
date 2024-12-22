
using UnityEngine;

namespace CDK.UI {
    public class CUIButtonMenuOpener : CUIButton {

        [SerializeField] CUIViewBase _menuToOpen;


        protected override void OnEnable()
        {
            base.OnEnable();
            OnClick += OnOnClick;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnClick -= OnOnClick;
        }

        void OnOnClick()
        {
            _navigationManager.OpenMenu(_menuToOpen, GetComponentInParent<CUIViewBase>(), this);
        }
    }
}
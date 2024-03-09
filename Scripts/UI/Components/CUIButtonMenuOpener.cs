using R3;
using UnityEngine;

namespace CDK.UI {
    public class CUIButtonMenuOpener : CUIButton {

        [SerializeField] private CUIViewBase _menuToOpen;


        protected override void Awake() {
            base.Awake();
            this.Button.OnClickAsObservable()
            .Subscribe(_ => {
                this._navigationManager.OpenMenu(this._menuToOpen, this.GetComponentInParent<CUIViewBase>(), this);
            })
            .AddTo(this._disposeOnDestroy);
        }
    }
}
using CDK.Interaction;
using CDK.UI;
using Reflex.Attributes;
using UnityEngine;

namespace CDK {
    public class COpenViewOnInteract : CInteractable {

        [SerializeField] CUIViewBase _viewToOpen;
        [Inject] protected readonly CUINavigationManager navigationManager;
        
        
        
        public override bool OnInteract(Transform interactingTransform) {
            if (!base.OnInteract(interactingTransform)) return false;
            navigationManager.OpenMenu(this._viewToOpen, null, null);
            return true;
        }
        
    }
}
using CDK.Interaction;
using CDK.UI;
using UnityEngine;

namespace CDK {
    public class COpenViewOnInteract : CInteractable {

        [SerializeField] private CUIViewBase _viewToOpen;

        
        
        
        public override bool OnInteract(Transform interactingTransform) {
            if (!base.OnInteract(interactingTransform)) return false;
            CUINavigationManager.get.OpenMenu(this._viewToOpen, null, null);
            return true;
        }
        
    }
}
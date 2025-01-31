using CDK.Interaction;
using CDK.UI;
using Reflex.Attributes;
using UnityEngine;

namespace CDK {
    public class COpenViewOnInteract : CInteractable {

        [SerializeField] View _viewToOpen;
        
        
        
        public override bool OnInteract(Transform interactingTransform) {
            if (!base.OnInteract(interactingTransform)) return false;
            View.InstantiateAndOpen(_viewToOpen, null, null);
            return true;
        }
        
    }
}
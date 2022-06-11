using UnityEngine;

namespace CDK.Interaction {
    public interface ICInteractable {
        bool OnInteract(Transform interactingTransform);
        void OnLookTo(Transform lookingTransform);
    }
}
using UnityEngine;

namespace CDK.Interaction {
    public interface ICInteractable {
        bool CanBeInteractedWith();
        void OnBecameInteractionTarget(Transform lookingTransform);
        bool OnInteract(Transform interactingTransform);
        void OnStoppedBeingInteractionTarget(Transform lookingTransform);
        Vector3 GetInteractionPromptPoint();
    }
}
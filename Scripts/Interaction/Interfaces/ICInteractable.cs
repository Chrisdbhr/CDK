using UnityEngine;

namespace CDK.Interaction {
    public interface ICInteractable {
        bool OnInteract(Transform interactingTransform);
        void OnBecameInteractionTarget(Transform lookingTransform);
        void OnStoppedBeingInteractionTarget(Transform lookingTransform);
        Vector3 GetInteractionPromptPoint();
    }
}
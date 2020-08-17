using UnityEngine;

namespace CDK {
	public interface CIInteractable {
		void OnLookTo(Transform lookingTransform);
		void OnInteract(Transform interactingTransform);
	}
}

using UnityEngine;

namespace CDK {
	public class CInteractableObject : MonoBehaviour, CIInteractable {

		[SerializeField] private bool onlyWorkOneTimePerSceneLoad;
		[SerializeField] private CUnityEventTransform InteractEvent;

		private void OnEnable() {
			// show enable checkbox
		}
		

		public void OnInteract(Transform interactingTransform) {
			if (!this.enabled || !this.gameObject.activeInHierarchy || CBlockingEventsManager.IsAnyBlockingEventHappening) return;
			this.InteractEvent?.Invoke(interactingTransform);
			if (this.onlyWorkOneTimePerSceneLoad) {
				Destroy(this);
			}
		}
		
		public void OnLookTo(Transform lookingTransform) {
			if (!this.enabled || !this.gameObject.activeInHierarchy || CBlockingEventsManager.IsAnyBlockingEventHappening) return;
			if (lookingTransform == null) return;
			Debug.Log($"{lookingTransform.name} looked to {this.name} in its interactable range.");	
		}

	}
}

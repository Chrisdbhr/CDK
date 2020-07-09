using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CInteractableObject : MonoBehaviour, CIInteractable {

		[SerializeField] private bool onlyWorkOneTimePerSceneLoad;
		[SerializeField] private UnityEvent InteractEvent;

		public void OnInteract(Transform interactingTransform) {
			if (!this.enabled || !this.gameObject.activeInHierarchy || CBlockingEventsManager.get.IsBlockingEventHappening) return;
			this.InteractEvent?.Invoke();
			if (this.onlyWorkOneTimePerSceneLoad) {
				Destroy(this);
			}
		}

		#if UNITY_EDITOR
		private void Reset() {
			this.gameObject.layer = 15;
		}
		#endif
	}
}

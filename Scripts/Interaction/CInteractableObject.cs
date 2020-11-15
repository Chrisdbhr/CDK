using System;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CInteractableObject : MonoBehaviour, CIInteractable {

		[SerializeField] private bool onlyWorkOneTimePerSceneLoad;
		[SerializeField] private UnityEvent InteractEvent;

		private void OnEnable() {
			// show enable checkbox
		}
		

		public void OnInteract(Transform interactingTransform) {
			if (!this.enabled || !this.gameObject.activeInHierarchy || CBlockingEventsManager.IsBlockingEventHappening) return;
			this.InteractEvent?.Invoke();
			if (this.onlyWorkOneTimePerSceneLoad) {
				Destroy(this);
			}
		}
		
		public void OnLookTo(Transform lookingTransform) {
			if (!this.enabled || !this.gameObject.activeInHierarchy || CBlockingEventsManager.IsBlockingEventHappening) return;
			if (lookingTransform == null) return;
			Debug.Log($"{lookingTransform.name} looked to {this.name} in its interactable range.");	
		}

	}
}

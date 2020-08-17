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
			if (!this.enabled || !this.gameObject.activeInHierarchy || CBlockingEventsManager.get.IsBlockingEventHappening) return;
			this.InteractEvent?.Invoke();
			if (this.onlyWorkOneTimePerSceneLoad) {
				Destroy(this);
			}
		}
		
		public void OnLookTo(Transform lookingTransform) {
			if (!this.enabled || !this.gameObject.activeInHierarchy || CBlockingEventsManager.get.IsBlockingEventHappening) return;
			Debug.Log($"{this.name} looked at by {lookingTransform.name}");	
		}

	}
}

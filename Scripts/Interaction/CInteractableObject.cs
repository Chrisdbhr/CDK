using System;
using UnityEngine;

namespace CDK {
	public class CInteractableObject : MonoBehaviour, CIInteractable {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private bool onlyWorkOneTimePerSceneLoad;
		[SerializeField] private CUnityEventTransform InteractEvent;
		[NonSerialized] private CBlockingEventsManager _blockingEventsManager;
		
		#endregion <<---------- Properties and Fields ---------->>


		
		
		#region <<---------- MonoBehaviour ---------->>
		
		private void Awake() {
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
		}

		private void OnEnable() {
			// show enable checkbox
		}

		#endregion <<---------- MonoBehaviour ---------->>


		
		
		#region <<---------- CIInteractable ---------->>
		
		public virtual void OnInteract(Transform interactingTransform) {
			if (!this.enabled || !this.gameObject.activeInHierarchy || this._blockingEventsManager.IsAnyBlockingEventHappening) return;
			this.InteractEvent?.Invoke(interactingTransform);
			if (this.onlyWorkOneTimePerSceneLoad) {
				Destroy(this);
			}
		}
		
		public virtual void OnLookTo(Transform lookingTransform) {
			if (!this.enabled || !this.gameObject.activeInHierarchy || this._blockingEventsManager.IsAnyBlockingEventHappening) return;
			if (lookingTransform == null) return;
			Debug.Log($"{lookingTransform.name} looked to {this.name} in its interactable range.");	
		}

		#endregion <<---------- CIInteractable ---------->>

	}
}

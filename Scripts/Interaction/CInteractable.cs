using CDK.UI;
using Reflex.Attributes;
using UnityEngine;

namespace CDK.Interaction {
	public class CInteractable : MonoBehaviour, ICInteractable {

		#region <<---------- Properties and Fields ---------->>
		
        [SerializeField] protected bool _debug;
		[SerializeField] bool onlyWorkOneTimePerSceneLoad;
        [SerializeField] Transform _interactionPromptPoint;
		[SerializeField] CUnityEventTransform InteractEvent;
		[Inject] protected readonly CBlockingEventsManager _blockingEventsManager;
		[Inject] protected readonly CUINavigationManager _navigationManager;

		#endregion <<---------- Properties and Fields ---------->>


		
		
		#region <<---------- MonoBehaviour ---------->>
		
		protected virtual void Awake() { }

		protected virtual void OnEnable() {
			// show enable checkbox
		}
        
		#endregion <<---------- MonoBehaviour ---------->>


		
		
		#region <<---------- CIInteractable ---------->>

        public virtual bool CanBeInteractedWith() {
            return this != null && this.enabled;
        }

        /// <summary>
        /// Returns TRUE if interacted sucesfull.
        /// </summary>
		public virtual bool OnInteract(Transform interactingTransform) {
			if (!this.enabled || !this.gameObject || !this.gameObject.activeInHierarchy || this._blockingEventsManager.IsAnyHappening) return false;
			this.InteractEvent?.Invoke(interactingTransform);
			if (this.onlyWorkOneTimePerSceneLoad) {
				Destroy(this);
			}
            return true;
        }

        public virtual void OnBecameInteractionTarget(Transform lookingTransform) { }

        public virtual void OnStoppedBeingInteractionTarget(Transform lookingTransform) { }

        public Vector3 GetInteractionPromptPoint() {
            if(this == null)return Vector3.zero;
            return this._interactionPromptPoint != null ? this._interactionPromptPoint.position : this.transform.position;
        }

        #endregion <<---------- CIInteractable ---------->>

    }
}
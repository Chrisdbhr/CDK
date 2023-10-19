using CDK.UI;
using UnityEngine;

#if DOTWEEN
using DG.Tweening;
#endif

namespace CDK.Interaction {
	public class CInteractable : MonoBehaviour, ICInteractable {

		#region <<---------- Properties and Fields ---------->>
		
        [SerializeField] protected bool _debug;
		[SerializeField] private bool onlyWorkOneTimePerSceneLoad;
        [SerializeField] private Transform _interactionPromptPoint;
		[SerializeField] private CUnityEventTransform InteractEvent;
        protected CBlockingEventsManager _blockingEventsManager;
        protected CUINavigationManager _navigationManager;

		#endregion <<---------- Properties and Fields ---------->>


		
		
		#region <<---------- MonoBehaviour ---------->>
		
		protected virtual void Awake() {
			this._blockingEventsManager = CBlockingEventsManager.get;
            this._navigationManager = CUINavigationManager.get;
        }

		protected virtual void OnEnable() {
			// show enable checkbox
		}

        protected virtual void OnDisable() { }
        
		#endregion <<---------- MonoBehaviour ---------->>


		
		
		#region <<---------- CIInteractable ---------->>

        public virtual bool CanBeInteractedWith() {
            return true;
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
            return this._interactionPromptPoint != null ? this._interactionPromptPoint.position : this.transform.position;
        }

        #endregion <<---------- CIInteractable ---------->>

    }
}
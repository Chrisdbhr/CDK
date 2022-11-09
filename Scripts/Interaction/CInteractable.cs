using CDK.UI;
using UnityEngine;

#if DOTween
using DG.Tweening;
#endif

namespace CDK.Interaction {
	public class CInteractable : MonoBehaviour, ICInteractable {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private bool onlyWorkOneTimePerSceneLoad;
		[SerializeField] private CUnityEventTransform InteractEvent;
        protected CBlockingEventsManager _blockingEventsManager;
        protected CUINavigationManager _navigationManager;
		#if DOTween
        private Tween _rotateTween;
		#endif

		#endregion <<---------- Properties and Fields ---------->>


		
		
		#region <<---------- MonoBehaviour ---------->>
		
		protected virtual void Awake() {
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
            this._navigationManager = CDependencyResolver.Get<CUINavigationManager>();
        }

		protected virtual void OnEnable() {
			// show enable checkbox
		}

        protected virtual void OnDisable() {
      		#if DOTween
            this._rotateTween?.Kill(false);
     		#endif
        }
        
		#endregion <<---------- MonoBehaviour ---------->>


		
		
		#region <<---------- CIInteractable ---------->>
		
        /// <summary>
        /// Returns TRUE if interacted sucesfull.
        /// </summary>
		public virtual bool OnInteract(Transform interactingTransform) {
			if (!this.enabled || !this.gameObject || !this.gameObject.activeInHierarchy || this._blockingEventsManager.IsAnyBlockingEventHappening) return false;
			this.InteractEvent?.Invoke(interactingTransform);
			if (this.onlyWorkOneTimePerSceneLoad) {
				Destroy(this);
			}
            this.RotateTowardsInteraction(interactingTransform);
            return true;
        }
		
		public virtual void OnBecameInteractionTarget(Transform lookingTransform) { }

        public virtual void OnStoppedBeingInteractionTarget(Transform lookingTransform) { }
        
        #endregion <<---------- CIInteractable ---------->>



        
        #region <<---------- General ---------->>

        protected void RotateTowardsInteraction(Transform t) {
            #if DOTween
            this._rotateTween = t.DOLookAt(this.transform.position, 0.5f, AxisConstraint.Y);
            this._rotateTween.Play();
            #else
			t.LookAt(this.transform.position);
			#endif
        }

        #endregion <<---------- General ---------->>

    }
}

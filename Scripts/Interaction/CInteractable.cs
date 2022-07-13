using CDK.UI;
using DG.Tweening;
using UnityEngine;

namespace CDK.Interaction {
	public class CInteractable : MonoBehaviour, ICInteractable {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private bool onlyWorkOneTimePerSceneLoad;
		[SerializeField] private CUnityEventTransform InteractEvent;
        [SerializeField] private string _animationToTrigger = "doorInteract";
        protected CBlockingEventsManager _blockingEventsManager;
        protected CUINavigationManager _navigationManager;
        private Tween _rotateTween;

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
            this._rotateTween?.Kill(false);
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
            this.RotateAndAnimateCharacter(interactingTransform);
            return true;
        }
		
		public virtual void OnLookTo(Transform lookingTransform) {
			if (!this.enabled || !this.gameObject.activeInHierarchy || this._blockingEventsManager.IsAnyBlockingEventHappening) return;
			if (lookingTransform == null) return;
			Debug.Log($"{lookingTransform.name} looked to {this.name} in its interactable range.");	
		}

		#endregion <<---------- CIInteractable ---------->>



        #region <<---------- General ---------->>

        protected void RotateAndAnimateCharacter(Transform rootTransform) {
            this._rotateTween = rootTransform.transform.DOLookAt(this.transform.position, 0.5f, AxisConstraint.Y);
            this._rotateTween.Play();
			
            var anim = rootTransform.GetComponentInChildren<Animator>();
            if (anim == null) return;
            if(!this._animationToTrigger.CIsNullOrEmpty()) anim.CSetTriggerSafe(Animator.StringToHash(this._animationToTrigger));
        }

        #endregion <<---------- General ---------->>


	}
}

using System;
using UnityEngine;

namespace CDK {
	public class CAdditionalMovement : StateMachineBehaviour {

		[SerializeField] private AnimationCurve _movementIntensityOverAnimTime = AnimationCurve.Linear(0f,1f,1f,0f);
		[SerializeField] private float _intensityMultiplier = 1f;
        [SerializeField] [Range(0f,1f)] private float _normalizedTime;
        
        
        
        
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			base.OnStateEnter(animator, stateInfo, layerIndex);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			base.OnStateExit(animator, stateInfo, layerIndex);
            this._normalizedTime = stateInfo.normalizedTime % 1f;
            animator.transform.GetParentOrSelf().GetComponentInChildren<CCharacterBase>().CDoIfNotNull(c => {
                c.AdditionalMovementFromAnimator = Vector3.zero;
            });
		}

		public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			base.OnStateMove(animator, stateInfo, layerIndex);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			
            this._normalizedTime = stateInfo.normalizedTime % 1f;
            
            animator.transform.GetParentOrSelf().GetComponentInChildren<CCharacterBase>().CDoIfNotNull(c => {
                c.AdditionalMovementFromAnimator = animator.transform.forward * (this._intensityMultiplier * this._movementIntensityOverAnimTime.Evaluate(this._normalizedTime));
            });
		}
	}
}

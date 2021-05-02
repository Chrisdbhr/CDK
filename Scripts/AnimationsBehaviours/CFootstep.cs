using System;
using UnityEngine;

namespace CDK {
	public class CFootstep : StateMachineBehaviour {

		public float FootstepInterval = 0.4f;
		[NonSerialized] private float _nextFootstepTime;
		[NonSerialized] private CFootstepsSource.FootstepFeet _lastFeet;
    
		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			base.OnStateEnter(animator, stateInfo, layerIndex);
			this._nextFootstepTime = 0f;
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (!(Time.time >= this._nextFootstepTime)) return;
			this._nextFootstepTime = Time.time + this.FootstepInterval;
			this._lastFeet = this._lastFeet == CFootstepsSource.FootstepFeet.left ? CFootstepsSource.FootstepFeet.right : CFootstepsSource.FootstepFeet.left;
			var footstepsSource = animator.GetComponent<CFootstepsSource>();
			if (footstepsSource == null) return;
			footstepsSource.Footstep(this._lastFeet);
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		// override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		// {
		//     
		// }

		// OnStateMove is called right after Animator.OnAnimatorMove()
		//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    // Implement code that processes and affects root motion
		//}

		// OnStateIK is called right after Animator.OnAnimatorIK()
		//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    // Implement code that sets up animation IK (inverse kinematics)
		//}
	}

}
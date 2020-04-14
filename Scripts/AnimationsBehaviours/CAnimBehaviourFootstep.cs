using System;
using UnityEngine;

namespace CDK {
	public class CAnimBehaviourFootstep : StateMachineBehaviour {

		public float FootstepInterval = 0.4f;
		[NonSerialized] private float _nextFootstepTime;
		[NonSerialized] private CFootstepsManager.FootstepFeet _lastFeet;
    
		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			this._nextFootstepTime = 0f;
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			if (Time.timeSinceLevelLoad >= this._nextFootstepTime) {
				this._nextFootstepTime = Time.timeSinceLevelLoad + this.FootstepInterval;
				this._lastFeet = this._lastFeet == CFootstepsManager.FootstepFeet.left ? CFootstepsManager.FootstepFeet.right : CFootstepsManager.FootstepFeet.left;
				animator.SendMessage(nameof(CFootstepsManager.Footstep), this._lastFeet, SendMessageOptions.DontRequireReceiver);
			}
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
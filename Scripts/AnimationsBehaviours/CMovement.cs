using UnityEngine;

namespace CDK {
	public class CMovement : StateMachineBehaviour {

		[SerializeField] private AnimationCurve _movementIntensity = AnimationCurve.Linear(0f,1f,1f,0f);
		[SerializeField] private float _movementSpeed = 1f;
		[SerializeField] [Range(0f,1f)]private float normalized;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			base.OnStateEnter(animator, stateInfo, layerIndex);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			base.OnStateExit(animator, stateInfo, layerIndex);
		}

		public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			base.OnStateMove(animator, stateInfo, layerIndex);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			var baseChar = animator.GetComponent<CCharacterBase>();
			if (baseChar == null) return;
			normalized = stateInfo.normalizedTime;
			baseChar.AdditiveMovement = animator.transform.forward * (this._movementSpeed * this._movementIntensity.Evaluate(stateInfo.normalizedTime));
		}
	}
}

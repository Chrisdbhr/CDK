using System;
using UnityEngine;

namespace CDK {
	public class CFootIK : MonoBehaviour {
		#region <<---------- Properties and Fields ---------->>
		[SerializeField] private Animator animator;
		[SerializeField] private CharacterController _charController;

		[SerializeField] private Transform leftFoot;
		[SerializeField] private Transform rightFoot;
		[SerializeField] private float _footSize = 0.1f;

		[SerializeField, Range(0f, 1f)] private float rightFootIkWeight = 1f;
		[SerializeField, Range(0f, 1f)] private float leftFootIkWeight = 1f;

		#endregion <<---------- Properties and Fields ---------->>

		private void OnAnimatorIK(int layerIndex) {
			this.animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, this.leftFootIkWeight);
			this.animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, this.rightFootIkWeight);
			this.ProcessIk(this.leftFoot.position, AvatarIKGoal.LeftFoot);
			this.ProcessIk(this.rightFoot.position, AvatarIKGoal.RightFoot);
		}

		private void ProcessIk(Vector3 feetPos, AvatarIKGoal ikGoal) {
			float checkDistance = this._charController.stepOffset;

			var hits = new RaycastHit[1];

			if (Physics.RaycastNonAlloc(
				feetPos - (Vector3.down * this._footSize) + (Vector3.up * (checkDistance * 0.5f)),
				Vector3.down * checkDistance,
				hits,
				checkDistance,
				1,
				QueryTriggerInteraction.Ignore
			) > 0) {
				if (feetPos.y - this._footSize < hits[0].point.y) {
					this.animator.SetIKPosition(ikGoal, hits[0].point + (hits[0].normal.normalized * this._footSize));
				}
			}
		}
	}
}

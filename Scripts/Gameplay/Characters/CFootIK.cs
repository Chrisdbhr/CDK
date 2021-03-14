using System;
using UnityEngine;

namespace CDK {
	[Obsolete]
	public class CFootIK : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private Animator animator;
		[SerializeField] private CharacterController _charController;

		[SerializeField] private Transform leftFoot;
		[SerializeField] private Transform rightFoot;
		[SerializeField] private float _footSize = 0.1f;
		[SerializeField] private LayerMask _footCollisionLayers = 1;

		private float rightFootIkWeight {
			get { return this._rightFootIkWeight; }
			set { this._rightFootIkWeight = value.CClamp01(); }
		}
		[SerializeField, Range(0f, 1f)] private float _rightFootIkWeight = 1f;
		private float leftFootIkWeight {
			get { return this._leftFootIkWeight.CClamp01(); }
			set { this._leftFootIkWeight = value.CClamp01(); }
		}
		[SerializeField, Range(0f, 1f)] private float _leftFootIkWeight = 1f;

		[NonSerialized] private RaycastHit _hitInfo;
		[NonSerialized] private Transform _transform;
		
		#endregion <<---------- Properties and Fields ---------->>

		
		private void Awake() {
			this._transform = this.transform;
		}

		private void OnAnimatorIK(int layerIndex) {
			this.animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, this.leftFootIkWeight);
			this.animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, this.rightFootIkWeight);
			this.ProcessIk(this.leftFoot.position, AvatarIKGoal.LeftFoot);
			this.ProcessIk(this.rightFoot.position, AvatarIKGoal.RightFoot);
		}

		private void ProcessIk(Vector3 feetPos, AvatarIKGoal ikGoal) {
			float checkDistance = this._charController.stepOffset;

			if (Physics.SphereCast(
				feetPos - (Vector3.down * this._footSize) + (Vector3.up * (checkDistance * 0.5f)),
				this._footSize,
				Vector3.down * checkDistance,
				out this._hitInfo,
				checkDistance,
				this._footCollisionLayers,
				QueryTriggerInteraction.Ignore
			)) {
				if (feetPos.y - this._footSize < this._hitInfo.point.y) {

					if (ikGoal == AvatarIKGoal.LeftFoot) this.leftFootIkWeight += CTime.DeltaTimeScaled;
					else this.leftFootIkWeight -= CTime.DeltaTimeScaled;
					
					if (ikGoal == AvatarIKGoal.RightFoot) this.rightFootIkWeight += CTime.DeltaTimeScaled;
					else this.rightFootIkWeight -= CTime.DeltaTimeScaled;
					
					this.animator.SetIKPosition(ikGoal, this._hitInfo.point + (this._hitInfo.normal.normalized * this._footSize));
					this.animator.SetIKRotation(ikGoal,  Quaternion.LookRotation(this._transform.forward, this._hitInfo.normal));
				}
			}
		}
	}
}

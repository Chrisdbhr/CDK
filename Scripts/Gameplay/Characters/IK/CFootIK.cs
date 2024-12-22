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
			get { return _rightFootIkWeight; }
			set { _rightFootIkWeight = value.CClamp01(); }
		}
		[SerializeField, Range(0f, 1f)] private float _rightFootIkWeight = 1f;
		private float leftFootIkWeight {
			get { return _leftFootIkWeight.CClamp01(); }
			set { _leftFootIkWeight = value.CClamp01(); }
		}
		[SerializeField, Range(0f, 1f)] private float _leftFootIkWeight = 1f;

		[NonSerialized] private RaycastHit _hitInfo;
		[NonSerialized] private Transform _transform;
		
		#endregion <<---------- Properties and Fields ---------->>

		
		private void Awake() {
			_transform = transform;
		}

		private void OnAnimatorIK(int layerIndex) {
			animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootIkWeight);
			animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootIkWeight);
			ProcessIk(leftFoot.position, AvatarIKGoal.LeftFoot);
			ProcessIk(rightFoot.position, AvatarIKGoal.RightFoot);
		}

		private void ProcessIk(Vector3 feetPos, AvatarIKGoal ikGoal) {
			float checkDistance = _charController.stepOffset;

			if (Physics.SphereCast(
				feetPos - (Vector3.down * _footSize) + (Vector3.up * (checkDistance * 0.5f)),
				_footSize,
				Vector3.down * checkDistance,
				out _hitInfo,
				checkDistance,
				_footCollisionLayers,
				QueryTriggerInteraction.Ignore
			)) {
				if (feetPos.y - _footSize < _hitInfo.point.y) {

					if (ikGoal == AvatarIKGoal.LeftFoot) leftFootIkWeight += CTime.DeltaTimeScaled;
					else leftFootIkWeight -= CTime.DeltaTimeScaled;
					
					if (ikGoal == AvatarIKGoal.RightFoot) rightFootIkWeight += CTime.DeltaTimeScaled;
					else rightFootIkWeight -= CTime.DeltaTimeScaled;
					
					animator.SetIKPosition(ikGoal, _hitInfo.point + (_hitInfo.normal.normalized * _footSize));
					animator.SetIKRotation(ikGoal,  Quaternion.LookRotation(_transform.forward, _hitInfo.normal));
				}
			}
		}
	}
}

using System;
using UnityEngine;

namespace CDK {
	public class CHandIk : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>
		 
		[SerializeField] private Animator animator;		

		[Header("Hand")]
		[SerializeField] private Transform leftHandOrigin;
		[SerializeField] private Transform leftHand;
		[SerializeField] private Transform rightHandOrigin;
		[SerializeField] private Transform rightHand;
		[SerializeField, Range(0f, 1f)] private float _handIkWeight = 1f;

		// cache
		[NonSerialized] private Vector3 _direction;
		[NonSerialized] private RaycastHit[] _hits = new RaycastHit[1];
		
		#endregion <<---------- Properties and Fields ---------->>


		
		
		#region <<---------- MonoBehaviour ---------->>
		
		private void OnAnimatorIK(int layerIndex) {
			this.ProcessIk(this.leftHandOrigin.position, this.leftHand.position, AvatarIKGoal.LeftHand);
			this.ProcessIk(this.rightHandOrigin.position, this.rightHand.position, AvatarIKGoal.RightHand);
		}

		#endregion <<---------- MonoBehaviour ---------->>


		
		
		private void ProcessIk(Vector3 origin, Vector3 targetPos, AvatarIKGoal ikGoal) {
			float checkDistance = Vector3.Distance(origin, targetPos);
			this._direction = targetPos - origin;

			bool hitSomething  = Physics.RaycastNonAlloc(
				origin,
				this._direction,
				this._hits,
				checkDistance,
				1,
				QueryTriggerInteraction.Ignore
			) > 0; 
			if (hitSomething) {
				this.animator.SetIKPosition(ikGoal, this._hits[0].point);
			}
			this.animator.SetIKPositionWeight(ikGoal, hitSomething ? this._handIkWeight : 0f);
		}
	}
}

using System;
using R3;
using UnityEngine;

namespace CDK {
	[RequireComponent(typeof(Animator))]
	public class CRootMotionReceiver : MonoBehaviour {

        public SerializableReactiveProperty<Vector3> DeltaPositionRx { get; private set; }
        public SerializableReactiveProperty<Quaternion> DeltaRotationRx { get; private set; }
        private Animator _animator;


        

		private void Awake() {
			this._animator = this.CGetComponentInChildrenOrInParent<Animator>();
            
            this.DeltaPositionRx = new ();
            this.DeltaRotationRx = new ();
        }

		private void OnDestroy() {
            this.DeltaPositionRx?.Dispose();
            this.DeltaPositionRx = null;
            this.DeltaRotationRx?.Dispose();
            this.DeltaRotationRx = null;
		}

		private void OnAnimatorMove() {
            this.DeltaPositionRx.Value = this._animator.deltaPosition;
            this.DeltaRotationRx.Value = this._animator.deltaRotation;
		}
        
                
        #region <<---------- Animations State Machine Behaviours ---------->>
		
        public void SetAnimationRootMotionEnabledState(bool state) {
            if(this._animator) this._animator.applyRootMotion = state;
        }
		
        #endregion <<---------- Animations State Machine Behaviours ---------->>
		
	}
}

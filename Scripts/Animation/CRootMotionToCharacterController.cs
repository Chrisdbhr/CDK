using System;
using UnityEngine;

namespace CDK {
	[RequireComponent(typeof(Animator))]
	public class CRootMotionToCharacterController : MonoBehaviour {

		private Animator _targetAnimator;
		public event Action<Vector3> RootMotionDeltaPositionChanged {
			add {
				this._rootMotionDeltaPositionChanged -= value;
				this._rootMotionDeltaPositionChanged += value;
			}
			remove {
				this._rootMotionDeltaPositionChanged -= value;
			}
		}

		private Action<Vector3> _rootMotionDeltaPositionChanged;

		
		
		
		private void Awake() {
			_targetAnimator = this.GetComponent<Animator>();
		}

		private void OnDestroy() {
			this._rootMotionDeltaPositionChanged = null;
		}

		private void OnAnimatorMove() {
			if (_rootMotionDeltaPositionChanged == null) return;
			if (CTime.TimeScale == 0f) {
				_rootMotionDeltaPositionChanged(Vector3.zero);
				return;
			}

			if (this._targetAnimator == null) return;

			_rootMotionDeltaPositionChanged(this._targetAnimator.deltaPosition);
		}
        
                
        #region <<---------- Animations State Machine Behaviours ---------->>
		
        public void SetAnimationRootMotion(bool state) {
            if(this._targetAnimator) this._targetAnimator.applyRootMotion = state;
        }
		
        #endregion <<---------- Animations State Machine Behaviours ---------->>
		
	}
}

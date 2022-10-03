using UnityEngine;

namespace CDK {
    [RequireComponent(typeof(Animator))]
    public class CRootMotionToRigidbody : MonoBehaviour {
        
        [SerializeField] private Animator _sourceAnimator;
        public Rigidbody TargetRigidbody;


        #region <<---------- MonoBehaviour ---------->>
        
        private void Awake() {
            this.CheckReferences();
        }

        private void Reset() {
            this.CheckReferences();
        }

        private void OnValidate() {
            this.CheckReferences();
        }

        private void OnAnimatorMove() {
            if (!this._sourceAnimator.applyRootMotion) return;
            this.TargetRigidbody.MovePosition(this.TargetRigidbody.position + this._sourceAnimator.deltaPosition);
            this.TargetRigidbody.MoveRotation(this.TargetRigidbody.rotation * this._sourceAnimator.deltaRotation);
        }

        #endregion <<---------- MonoBehaviour ---------->>


        private void CheckReferences() {
            if (this._sourceAnimator == null) this._sourceAnimator = this.GetComponent<Animator>();
        }
    }
}
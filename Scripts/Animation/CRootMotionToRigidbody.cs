using UnityEngine;

namespace CDK {
    [RequireComponent(typeof(Animator))]
    public class CRootMotionToRigidbody : MonoBehaviour {
        
        [SerializeField] private Animator _sourceAnimator;
        public Rigidbody TargetRigidbody;


        #region <<---------- MonoBehaviour ---------->>
        
        private void Awake() {
            CheckReferences();
        }

        private void Reset() {
            CheckReferences();
        }

        private void OnValidate() {
            CheckReferences();
        }

        private void OnAnimatorMove() {
            if (!_sourceAnimator.applyRootMotion) return;
            TargetRigidbody.MovePosition(TargetRigidbody.position + _sourceAnimator.deltaPosition);
            TargetRigidbody.MoveRotation(TargetRigidbody.rotation * _sourceAnimator.deltaRotation);
        }

        #endregion <<---------- MonoBehaviour ---------->>


        private void CheckReferences() {
            if (_sourceAnimator == null) _sourceAnimator = GetComponent<Animator>();
        }
    }
}
using UnityEngine;

namespace CDK {
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class CChangeCollisionPrecisionOnContact3D : CPhysicsTrigger {

        [SerializeField] private RigidbodyInterpolation _targetInterpolateMode = RigidbodyInterpolation.Interpolate;
        [SerializeField] private CollisionDetectionMode _targetCollisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        private RigidbodyInterpolation _initialInterpolateMode;
        private CollisionDetectionMode _initialCollisionDetectionMode;
        

        private Rigidbody _rb;

        
        

        protected override void Awake() {
            base.Awake();

            this._rb = this.GetComponent<Rigidbody>();
            
            this._initialInterpolateMode = this._rb.interpolation;
            this._initialCollisionDetectionMode = this._rb.collisionDetectionMode;
        }

        protected override void StartedCollisionOrTrigger(Transform other) {
            base.StartedCollisionOrTrigger(other);
            this._rb.interpolation = this._targetInterpolateMode;
            this._rb.collisionDetectionMode = this._targetCollisionDetectionMode;
        }

        protected override void ExitedCollisionOrTrigger(Transform other) {
            base.ExitedCollisionOrTrigger(other);
            this._rb.interpolation = this._initialInterpolateMode;
            this._rb.collisionDetectionMode = this._initialCollisionDetectionMode;
        }
    }
}
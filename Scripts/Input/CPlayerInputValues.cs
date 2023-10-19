using UnityEngine;

namespace CDK {
    [System.Serializable]
    public class CPlayerInputValues {
        /// <summary>
        /// Can be relative to camera position. 
        /// </summary>
        public Vector2 RelativeMovement2D => new Vector2(this.RelativeMovement3D.x, this.RelativeMovement3D.z);
        /// <summary>
        /// Can be relative to camera position. 
        /// </summary>
        public Vector3 RelativeMovement3D { get; private set; }
        /// <summary>
        /// Movement not relative to camera position.
        /// </summary>
        public Vector3 RawMovement3D { get; private set; }
        public float MovementSqrMagnitude => this.RelativeMovement3D.sqrMagnitude;
        public bool IsDoingMovementInput => MovementSqrMagnitude > 0.01f;
        
        public bool Walk;
        public bool Run;
        public bool Aim;

        public void SetRawMovement3D(float x, float z) {
            var value = new Vector3(x, 0f, z);
            this.RawMovement3D = value.normalized;
        }

        public void SetMovement3DRelativeToCamera(Transform cameraTransform) {

            //camera forward and right vectors:
            var forward = cameraTransform.forward;
            var right = cameraTransform.right;

            //project forward and right vectors on the horizontal plane (y = 0)
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            this.RelativeMovement3D = (forward * this.RawMovement3D.z) + (right * this.RawMovement3D.x);
        }
    }
}
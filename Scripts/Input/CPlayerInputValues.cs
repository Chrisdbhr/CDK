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
            var value = this.RawMovement3D;
            value.x = x.CClamp(-1f, 1f);
            value.y = 0f;
            value.z = z.CClamp(-1f, 1f);
            this.RawMovement3D = value;
        }

        public void SetMovement3DRelativeToCamera(Transform cameraTransform) {
            var value = cameraTransform.rotation * this.RawMovement3D;
            value.x = value.x.CClamp(-1f, 1f);
            value.y = 0f;
            value.z = value.z.CClamp(-1f, 1f);
            this.RelativeMovement3D = value;
        }
    }
}
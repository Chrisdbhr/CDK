using UnityEngine;

namespace CDK {
    [System.Serializable]
    public class CPlayerInputValues {
        /// <summary>
        /// Can be relative to camera position. 
        /// </summary>
        public Vector2 Movement2D => new Vector2(Movement3D.x, Movement3D.z);
        /// <summary>
        /// Can be relative to camera position. 
        /// </summary>
        public Vector3 Movement3D;
        public float MovementSqrMagnitude => this.Movement3D.sqrMagnitude;
        public bool IsDoingMovementInput => MovementSqrMagnitude > 0.01f;
        
        public bool    Walk;
        public bool    Run;
        
        public bool    Jump;
        public bool    Aim;
    }
}
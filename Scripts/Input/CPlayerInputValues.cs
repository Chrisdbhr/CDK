using UnityEngine;

namespace CDK {
    public struct CPlayerInputValues {
        /// <summary>
        /// Relative to camera position.
        /// </summary>
        public Vector3 Movement;
        public bool    Walk;
        public bool    Run;
        public bool    Jump;
        public bool    Aim;
    }
}
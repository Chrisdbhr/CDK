using UnityEngine;

namespace CDK {
    public abstract class CPhysics {
        
        public const float DefaultGravityY = -9.81f;
        
        public static float GetDistanceFromDefaultGravity() {
            return (DefaultGravityY - Physics.gravity.y).CAbs();
        }

    }
}
using System.Linq;
using UnityEngine;

namespace CDK {
    public abstract class CPhysics {
        
        public const float DefaultGravityY = -9.81f;
        
        public static float GetDistanceFromDefaultGravity() {
            return (DefaultGravityY - Physics.gravity.y).CAbs();
        }
        
        public static RaycastHit[] RaycastAllOrdered(Ray ray, float maxDistance, int layerMask) {
            var hits = Physics.RaycastAll(ray.origin, ray.direction, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
            return hits.OrderBy(h => h.distance).ToArray();
        }

    }
}
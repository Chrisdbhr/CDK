using UnityEngine;

namespace CDK {
    [RequireComponent(typeof(Collider))]
    public class CFootstepSurfaceCollider : MonoBehaviour, CIFootstepSurfaceBase {
        
        public CFootstepInfo FootstepInfoData;

        public CFootstepInfo GetFootstepInfoFromRaycastHit(RaycastHit hit) {
            return FootstepInfoData;
        }
    }
}
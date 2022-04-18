using UnityEngine;

namespace CDK {
    public interface CIFootstepSurfaceBase {

        CFootstepInfo GetFootstepInfoFromRaycastHit(RaycastHit hit);
        
    }
}
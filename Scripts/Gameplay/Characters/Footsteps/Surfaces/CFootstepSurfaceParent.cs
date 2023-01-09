using Unity.Linq;
using UnityEngine;

namespace CDK {
    public class CFootstepSurfaceParent : MonoBehaviour, CIFootstepSurfaceBase {
        
        public CFootstepInfo FootstepInfoData;

        public CFootstepInfo GetFootstepInfoFromRaycastHit(RaycastHit hit) {
            return FootstepInfoData;
        }


        

        private void OnValidate() {
            if(this.gameObject.Children().CIsNullOrEmpty()) {
                Debug.LogError($"Footstep Surface Parent has not children!", this);
            }
        }
    }
}
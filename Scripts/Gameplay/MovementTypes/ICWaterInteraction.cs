using UnityEngine;

namespace CDK {
    public interface ICWaterInteraction {
        void OnEnterWater(Transform waterTransform);
        void OnExitWater(Transform waterTransform);
    }
}
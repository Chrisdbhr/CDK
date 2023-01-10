using UnityEngine;

namespace CDK {
    public interface ICWaterInteractor {
        void OnEnterWater(Transform waterTransform);
        void OnExitWater(Transform waterTransform);
    }
}
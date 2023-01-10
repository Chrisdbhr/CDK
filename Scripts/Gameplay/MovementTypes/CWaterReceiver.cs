using UnityEngine;

namespace CDK {
    [RequireComponent(typeof(Collider))]
    public class CWaterReceiver : MonoBehaviour, ICWaterInteractor {

        [SerializeField] private CUnityEventTransform _onEnterWater;
        [SerializeField] private CUnityEventTransform _onExitWater;
        
        
        
        
        public void OnEnterWater(Transform waterTransform) {
            this._onEnterWater?.Invoke(waterTransform);
        }

        public void OnExitWater(Transform waterTransform) {
            this._onExitWater?.Invoke(waterTransform);
        }
    }
}
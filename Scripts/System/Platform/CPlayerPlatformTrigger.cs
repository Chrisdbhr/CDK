using UnityEngine;
using UnityEngine.Events;

namespace CDK {
    public class CPlayerPlatformTrigger : CAutoTriggerCompBase {

        [SerializeField] private UnityEvent _isWebgl;
        
        
        
        
        protected override void TriggerEvent() {
            switch (Application.platform) {
                case RuntimePlatform.WebGLPlayer: {
                    this._isWebgl?.Invoke();
                    break;
                }
            }
        }
    }
}
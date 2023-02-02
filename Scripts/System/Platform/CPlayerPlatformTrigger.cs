using UnityEngine;

namespace CDK {
    public class CPlayerPlatformTrigger : CAutoTriggerCompBase {

        [SerializeField] private CUnityEventBool _isWebgl;
        [SerializeField] private CUnityEventBool _isMobile;
        [SerializeField] private CUnityEventBool _isConsole;
        
        
        
        
        protected override void TriggerEvent() {
            this._isWebgl?.Invoke(Application.platform == RuntimePlatform.WebGLPlayer);
            this._isMobile?.Invoke(IsMobilePlatform());
            this._isConsole?.Invoke(
                Application.platform == RuntimePlatform.XboxOne 
                || Application.platform == RuntimePlatform.PS4
                || Application.platform == RuntimePlatform.PS5
                || Application.platform == RuntimePlatform.Switch
                || Application.platform == RuntimePlatform.GameCoreXboxSeries
                || Application.platform == RuntimePlatform.GameCoreXboxOne
            );
        }

        public static bool IsMobilePlatform() {
            return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
        }
        
    }
}
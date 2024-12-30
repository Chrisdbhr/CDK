using UnityEngine;

namespace CDK {
    public class CPlayerPlatformTrigger : CAutoTriggerCompBase {

        [SerializeField] CUnityEvent _isWebgl;
        [SerializeField] CUnityEvent _isMobile;
        [SerializeField] CUnityEvent _isConsole;
        
        
        
        
        protected override void TriggerEvent() {
            if(Application.platform == RuntimePlatform.WebGLPlayer) _isWebgl?.Invoke();
            if(IsMobilePlatform()) _isMobile?.Invoke();
            if(IsConsolePlatform()) _isConsole?.Invoke();
        }

        public static bool IsMobilePlatform() {
            return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
        }
        
        static bool IsConsolePlatform() {
            return Application.platform == RuntimePlatform.XboxOne 
                || Application.platform == RuntimePlatform.PS4
                || Application.platform == RuntimePlatform.PS5
                || Application.platform == RuntimePlatform.Switch
                || Application.platform == RuntimePlatform.GameCoreXboxSeries
                || Application.platform == RuntimePlatform.GameCoreXboxOne;
        }
        
    }
}
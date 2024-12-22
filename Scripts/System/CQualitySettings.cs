using System.Threading;

using UnityEngine;

namespace CDK {
    public static class CQualitySettings {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitializeAfterSceneLoad() {
            #if UNITY_STANDALONE && !UNITY_EDITOR
            Debug.Log($"{nameof(CDK)}.{nameof(CQualitySettings)}: Starting monitoring for screen resolution smaller than 1024x768 to resize.");
            Observable.EveryUpdate(CApplication.QuittingCancellationTokenSource.Token).Subscribe(_ => {
                if (Screen.fullScreenMode == FullScreenMode.Windowed && (Screen.width < 1024 || Screen.height < 768)) {
                    Debug.Log($"{nameof(CDK)}.{nameof(CQualitySettings)}: Setting Screen Resolution to 1024x768 and fullscreen mode to false.");
                    Screen.SetResolution(1024, 768, false);
                }
            });
            #endif
        }

        public static bool GetVsync() {
            return QualitySettings.vSyncCount == 1;
        }

        public static void SetVsync(bool value) {
            QualitySettings.vSyncCount = value ? 1 : 0;
        }

    }
}
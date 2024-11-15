﻿using System.Threading;
using R3;
using UnityEngine;

namespace CDK {
    public static class CQualitySettings {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitializeAfterSceneLoad() {
            Observable.EveryUpdate(CApplication.QuittingCancellationTokenSource.Token).Subscribe(_ => {
                if (Screen.fullScreenMode == FullScreenMode.Windowed && (Screen.width < 1024 || Screen.height < 768)) {
                    Screen.SetResolution(1024, 768, false);
                }
            });
        }

        public static bool GetVsync() {
            return QualitySettings.vSyncCount == 1;
        }

        public static void SetVsync(bool value) {
            QualitySettings.vSyncCount = value ? 1 : 0;
        }

    }
}
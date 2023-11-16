using UnityEngine;

namespace CDK {
    public static class CQualitySettings {

        public static bool GetVsync() {
            return QualitySettings.vSyncCount == 1;
        }

        public static void SetVsync(bool value) {
            QualitySettings.vSyncCount = value ? 1 : 0;
        }

    }
}
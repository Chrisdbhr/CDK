using UnityEngine;

namespace CDK {
    public static class CUnityObjectExtensions {
        public static void CDestroy(this Object value, float time = 0f) {
            if (value == null) return;
            if (Application.isPlaying) {
                Object.Destroy(value, time > 0f ? time : 0f);
            }
            else {
                Object.DestroyImmediate(value);
            }
        }
    }
}

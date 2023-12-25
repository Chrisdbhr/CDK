using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CDK {
    public static class CUnityObjectExtensions {

        public static void CDoIfNotNull<T>(this T value, Action<T> actionToDo) {
            if (value is Object oValue) {
                if (oValue == null) return;
            }
            if (value == null) return;
            actionToDo?.Invoke(value);
        }

        public static void CDestroy(this Object value, bool shouldLog = false, float time = 0f) {
            if (value == null) return;
            if (shouldLog) {
                Debug.Log("Destroying " + value.name + " - " + value.GetType().Name + " - " + value.GetInstanceID(), value);
            }
            if (Application.isPlaying) {
                Object.Destroy(value, time > 0f ? time : 0f);
                return;
            }
            // is on editor
            Object.DestroyImmediate(value, false);
        }

        public static void CDestroyImmediate(this Object value, bool allowDestroyingAssets = false) {
            if (value == null) return;
            Object.DestroyImmediate(value, allowDestroyingAssets);
        }

    }
}
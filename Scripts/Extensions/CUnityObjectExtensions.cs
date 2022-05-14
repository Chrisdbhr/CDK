using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CDK {
    public static class CUnityObjectExtensions {
        
        public static void CDoIfNotNull<T>(this T value, Action<T> actionToDo) where T : Object {
            if (value == null) return;
            actionToDo?.Invoke(value);
        }
        
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

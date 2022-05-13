using System;
using Object = UnityEngine.Object;

namespace CDK {
    public static class CObjectExtensions {
        public static void CDoIfNotNull<T>(this T value, Action<T> actionToDo) {
            if (value == null) return;
            actionToDo?.Invoke(value);
        }
    }
}

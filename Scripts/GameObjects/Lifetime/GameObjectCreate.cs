using UnityEngine;

namespace CDK
{
    public static class GameObjectCreate
    {
        public static T WithComponent<T>(string name = null) where T : Component
        {
            var go = new GameObject(name ?? typeof(T).Name);
            #if UNITY_EDITOR
            go.hideFlags = HideFlags.DontSaveInEditor;
            #endif
            return go.AddComponent<T>();
        }

    }
}
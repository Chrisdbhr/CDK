using UnityEngine;

namespace CDK {
    public static class CSingletonHelper {
        
        public static T CreateInstance<T>(string gameObjectName) where T : MonoBehaviour {
            if (CannotCreateAnyInstance()) {
                return null;
            }
            return new GameObject(gameObjectName).CDontDestroyOnLoad().AddComponent<T>();
        }
        

        
        

        public static bool CannotCreateAnyInstance() {
            return CApplication.IsQuitting || !Application.isPlaying;
        }

    }
}
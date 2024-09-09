using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CDK {
    /// <summary>
    /// Dependency Injection helper.
    /// </summary>
    public static class DIHelper {

        public static void ResolveFromActiveScene<T>(out T resolvedObject) where T : class {
            resolvedObject = SceneManager.GetActiveScene().GetSceneContainer().Resolve<T>();
        }

        public static Container Inject(this GameObject gameObject) {
            if (gameObject == null) {
                Debug.LogError($"GameObject to inject is null, returning null Container");
                return default;
            }
            var scene = gameObject.scene;
            if (scene.name == CStrings.DontDestroyOnLoad) scene = SceneManager.GetActiveScene();
            var sceneContainer = scene.GetSceneContainer();
            GameObjectInjector.InjectObject(gameObject, sceneContainer);
            return sceneContainer;
        }
    }
}
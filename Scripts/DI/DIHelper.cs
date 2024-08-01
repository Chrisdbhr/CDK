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

        public static void Inject(this GameObject gameObject) {
            if (gameObject == null) return;
            var scene = gameObject.scene;
            if (scene.name == CStrings.DontDestroyOnLoad) scene = SceneManager.GetActiveScene();
            GameObjectInjector.InjectObject(gameObject, scene.GetSceneContainer());
        }
    }
}
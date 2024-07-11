using Reflex.Extensions;
using UnityEngine.SceneManagement;

namespace CDK.DI {
    /// <summary>
    /// Dependency Injection helper.
    /// </summary>
    public static class DIHelper {

        public static TContract ResolveFromActiveScene<TContract>() {
            return SceneManager.GetActiveScene().GetSceneContainer().Resolve<TContract>();
        }
        
    }
}
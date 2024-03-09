using System;
using System.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CDK {
    public static class CAssets {

        #region <<---------- Instantiation ---------->>

        public static T InstantiateAndGetComponent<T>(GameObject prefab, Transform parent = null) {
            return InstantiateAndGetComponent<T>(prefab, Vector3.zero, Quaternion.identity, parent);
        }
        
        public static T InstantiateAndGetComponent<T>(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null) {
            var createdGo = GameObject.Instantiate(prefab, position, rotation, parent);
            return createdGo == null ? default : createdGo.GetComponent<T>();
        }

        #endregion <<---------- Instantiation ---------->>

        


        #region <<---------- Load From Resources ---------->>

        /// <summary>
        /// Load a Resource asset from the Resources folder.
        /// </summary>
        public static T LoadResource<T>(string address) where T : UnityEngine.Object {
            return Resources.Load<T>(address);
        }

        /// <summary>
        /// Load a Resource from the Resources folder and instantiate it.
        /// </summary>
        public static T LoadResourceAndInstantiate<T>(string address, Vector3 position, Quaternion rotation, Transform parent = null) where T : UnityEngine.Component {
            if (!Application.isPlaying) {
                Debug.LogError($"Will not load from resources because application is not playing.");
                return null;
            }
            var resource = Resources.Load<GameObject>(address);
            if (resource == null) {
                Debug.LogError($"Could not {nameof(LoadResourceAndInstantiate)} from key '{address}'");
                return null;
            }
            return Object.Instantiate(resource, position, rotation, parent).GetComponent<T>();
        }
        
        /// <summary>
        /// Load a Resource from the Resources folder and instantiate it.
        /// </summary>
        public static T LoadResourceAndInstantiate<T>(string address, Transform parent = null) where T : UnityEngine.Component {
            if (!Application.isPlaying) {
                Debug.LogError($"Will not load from resources because application is not playing.");
                return null;
            }
            var resource = Resources.Load<GameObject>(address);
            if (resource == null) {
                Debug.LogError($"Could not {nameof(LoadResourceAndInstantiate)} from key '{address}'");
                return null;
            }
            return Object.Instantiate(resource, parent).GetComponent<T>();
        }
        
        #endregion <<---------- Load From Resources ---------->>


        

        #region <<---------- Load From Holder Scene ---------->>

        public static async Task<T> LoadFromHolderSceneAsync<T>(string sceneName, bool setObjectAsDontDestroy = false) where T : Component {
            var activeScene = SceneManager.GetActiveScene();
            var asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            await asyncOp;
            var loadedScene = SceneManager.GetSceneByName(sceneName);
            var rootGameObjects = loadedScene.GetRootGameObjects();
            if (rootGameObjects == null || rootGameObjects.Length <= 0) {
                Debug.LogError($"No objects inside scene: '{sceneName}'");
                await SceneManager.UnloadSceneAsync(sceneName);
                return default;
            }
            if (rootGameObjects.Length > 1) {
                Debug.LogError($"A holder scene should not have more than 1 root object ({sceneName})");
            }

            var go = rootGameObjects[0];

            if (!go.TryGetComponent<T>(out var comp)) {
                Debug.LogError($"Could not find Object '{nameof(T)}' from scene '{sceneName}'");
                await SceneManager.UnloadSceneAsync(sceneName);
                return default;
            }

            if (setObjectAsDontDestroy) {
                Object.DontDestroyOnLoad(go);
            }
            else if(SceneManager.GetActiveScene() == activeScene) {
                SceneManager.MoveGameObjectToScene(go, activeScene);
            }
            
            await SceneManager.UnloadSceneAsync(sceneName);
  
            return comp;
        } 

        #endregion <<---------- Load From Holder Scene ---------->>




        #region <<---------- Unloaders ---------->>

        public static void UnloadAsset(GameObject goToUnload, bool releaseAsset = false) {
            if (goToUnload == null) {
                Debug.LogError($"Can't unload a null asset.");
                return;
            }

            goToUnload.CDestroy();
            if (releaseAsset) {
                Resources.UnloadAsset(goToUnload);
            }
        }

        public static void UnloadAsset(Object obj) {
            if (obj == null) {
                Debug.LogError($"Will not unload a null asset.");
                return;
            }
            Debug.Log($"Unloading obj {obj.name}");
            Resources.UnloadAsset(obj);
        }

        #endregion <<---------- Unloaders ---------->>

	}
}
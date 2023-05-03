using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

#if UnityAddressables
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif

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

        [Obsolete("Try to not use Resources!")]
        public static T LoadResource<T>(string address) where T : UnityEngine.Object {
            return Resources.Load<T>(address);
        }

        [Obsolete("Try to not use Resources!")]
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
            
            return GameObject.Instantiate(resource, parent).GetComponent<T>();
        }

        #endregion <<---------- Load From Resources ---------->>


        

        #region <<---------- Load From Holder Scene ---------->>

        public static async Task<T> LoadFromHolderSceneAsync<T>(string sceneName, bool setObjectAsDontDestroy = false) where T : Component {
            var activeScene = SceneManager.GetActiveScene();
            var asyncOp = await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).AsAsyncOperationObservable().ToTask();
            var loadedScene = SceneManager.GetSceneByName(sceneName);
            var rootGameObjects = loadedScene.GetRootGameObjects();
            if (rootGameObjects == null || rootGameObjects.Length <= 0) {
                Debug.LogError($"No objects inside scene: '{sceneName}'");
                await SceneManager.UnloadSceneAsync(sceneName).AsAsyncOperationObservable().ToTask();
                return default;
            }
            if (rootGameObjects.Length > 1) {
                Debug.LogError($"A holder scene should not have more than 1 root object ({sceneName})");
            }

            var go = rootGameObjects[0];

            if (!go.TryGetComponent<T>(out var comp)) {
                Debug.LogError($"Could not find Object '{nameof(T)}' from scene '{sceneName}'");
                await SceneManager.UnloadSceneAsync(sceneName).AsAsyncOperationObservable().ToTask();
                return default;
            }

            if (setObjectAsDontDestroy) {
                Object.DontDestroyOnLoad(go);
            }
            else if(SceneManager.GetActiveScene() == activeScene) {
                SceneManager.MoveGameObjectToScene(go, activeScene);
            }
            
            await SceneManager.UnloadSceneAsync(sceneName).AsAsyncOperationObservable().ToTask();
  
            return comp;
        } 

        #endregion <<---------- Load From Holder Scene ---------->>




        #region <<---------- Load From Addressables ---------->>

        public static async Task<T> LoadAssetAsync<T>(AssetReference key) where T : Object {
            Debug.Log($"Loading asset with key '{key}'");
            #if UnityAddressables
            var asyncOp = Addressables.LoadAssetAsync<T>(key);
            while (!asyncOp.IsDone) {
                await Observable.NextFrame();
            }
            return asyncOp.Result;
			#else
			throw new NotImplementedException();
			#endif
        }
        
        public static async Task<T> LoadPrefabAsync<T>(string key) where T : Object {
            Debug.Log($"Loading prefab asset with key '{key}'");
			#if UnityAddressables
            var asyncOp = Addressables.LoadAssetAsync<GameObject>(key);
            while (!asyncOp.IsDone) {
                await Observable.NextFrame();
            }
            var go = asyncOp.Result;
            return go.GetComponent<T>();
			#else
			throw new NotImplementedException();
			#endif
        }

		#if UnityAddressables

        public static async Task<T> LoadPrefabAsync<T>(AssetReference key) where T : Object {
            return await LoadPrefabAsync<T>(key.RuntimeKey.ToString());
        }

        public static async Task<T> LoadAndInstantiateAsync<T>(AssetReference key, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool trackHandle = true) where T : Component {
            return await LoadAndInstantiateAsync<T>(key.RuntimeKey.ToString(), position, rotation, parent, trackHandle);
        }

        public static async Task<T> LoadAndInstantiateAsync<T>(string key, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool trackHandle = true) where T : Component {
            if (!Application.isPlaying) return null;
            Debug.Log($"Loading GameObject with key '{key}'{(parent != null ? $" on parent '{parent.name}'" : string.Empty)}");
            try {
                var instantiationParameters = new InstantiationParameters(position, rotation, parent);
                
                var asyncOp = Addressables.InstantiateAsync(key, instantiationParameters, trackHandle);
                
                while (!asyncOp.IsDone) {
                    await Observable.NextFrame();
                }
                var go = asyncOp.Result;
                if (CApplication.IsQuitting) {
                    CAssets.UnloadAsset(go);
                    return null;
                }
                if (go == null) {
                    throw new NullReferenceException($"Could not Instantiate object with key '{key}'.");
                }

                Debug.Log($"Loaded GameObject with key '{key}' ", go);
                return go.GetComponent<T>();
            }
            catch (Exception e) {
                Debug.LogError($"Exception trying to Load and Instantiate GameObject Async with key '{key}':\n" + e);
            }

            return null;
        }

        #endif

        #endregion <<---------- Load From Addressables ---------->>




        #region <<---------- Unloaders ---------->>

        public static bool UnloadAsset(GameObject goToUnload, bool destroyIfCantReleaseInstance = true) {
            if (goToUnload == null) {
                Debug.LogError($"Will not unload a null asset.");
                return false;
            }
			#if UnityAddressables
            bool success = Addressables.ReleaseInstance(goToUnload);
            if (success) return true;
			#endif
            if (destroyIfCantReleaseInstance) {
                Debug.Log($"GameObject '{goToUnload.name}' will be destroyed.", goToUnload);
                goToUnload.CDestroy();
            }

            return destroyIfCantReleaseInstance;
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

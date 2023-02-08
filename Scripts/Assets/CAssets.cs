using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

#if UnityAddressables
using UnityEngine.AddressableAssets;
#endif

#if UniTask
using Cysharp.Threading.Tasks;
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

        public static async Task<T> LoadFromHolderSceneAsync<T>(string sceneName, bool setObjectAsDontDestroy = true) where T : UnityEngine.Object {
            var asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            await asyncOp.AsObservable();
            var loadedScene = SceneManager.GetSceneByName(sceneName);
            var rootGameObjects = loadedScene.GetRootGameObjects();
            if (rootGameObjects == null || rootGameObjects.Length <= 0) {
                Debug.LogError($"No objects inside scene: '{sceneName}'");
                return default;
            }
            if (rootGameObjects.Length > 1) {
                Debug.LogError($"A holder scene should not have more than 1 root object ({sceneName})");
            }

            var go = rootGameObjects[0];

            if (!go.TryGetComponent<T>(out var comp)) {
                Debug.LogError($"Could not find Object '{nameof(T)}' from scene '{sceneName}'");
                return default;
            }
            
            if (setObjectAsDontDestroy) {
                Object.DontDestroyOnLoad(go);
            }
            
            return comp;
        } 

        #endregion <<---------- Load From Holder Scene ---------->>




        #region <<---------- Load From Addressables ---------->>

        public static async Task<T> LoadObjectAsync<T>(string key) {
            Debug.Log($"Loading asset key '{key}'");
			#if UnityAddressables && UniTask
            return await Addressables.LoadAssetAsync<T>(key);
			#else
			throw new NotImplementedException();
			#endif
        }

		#if UnityAddressables

        public static async Task<T> LoadObjectAsync<T>(AssetReference key) {
            return await LoadObjectAsync<T>(key.RuntimeKey.ToString());
        }

        public static async Task<GameObject> LoadAndInstantiateGameObjectAsync(AssetReference key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
            return await LoadAndInstantiateGameObjectAsync(key.RuntimeKey.ToString(), parent, instantiateInWorldSpace, trackHandle);
        }

        public static async Task<GameObject> LoadAndInstantiateGameObjectAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
            if (!Application.isPlaying) return null;
			#if UniTask
            Debug.Log($"Starting to load GameObject with key '{key}'{(parent != null ? $" on parent '{parent.name}'" : string.Empty)}");
            try {
                var asyncOp = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
                var go = await asyncOp.WithCancellation(CApplication.QuittingCancellationTokenSource.Token);
                if (go == null) {
                    throw new NullReferenceException($"Could not Instantiate object with key '{key}'.");
                }

                Debug.Log($"Loaded GameObject with key '{key}' ", go);
                return go;
            }
            catch (Exception e) {
                Debug.LogError($"Exception trying to Load and Instantiate GameObject Async with key '{key}':\n" + e);
            }
			#else
			Debug.Log($"UniTask is not installed in project with UnityAddressables, could not Load Asset.");
			#endif

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

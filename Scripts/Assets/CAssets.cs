using System;
using System.Threading.Tasks;
using UnityEngine;
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
            var createdGo = GameObject.Instantiate(prefab, parent);
            return createdGo == null ? default : createdGo.GetComponent<T>();
        }

        #endregion <<---------- Instantiation ---------->>




        #region <<---------- Load From Resources ---------->>

        public static T LoadResource<T>(string address) where T : UnityEngine.Object {
            return Resources.Load<T>(address);
        }

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
            Resources.UnloadAsset(obj);
        }

        #endregion <<---------- Unloaders ---------->>

	}
}

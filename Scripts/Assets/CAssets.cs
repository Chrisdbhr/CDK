using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CDK.UI;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

#if UnityAddressables
using UnityEngine.AddressableAssets;
#endif

#if UniTask
using Cysharp.Threading.Tasks;
#endif

namespace CDK {
	public class CAssets {

		#region <<---------- Properties ---------->>
		
		private CanvasGroup _loadingCanvas;
		private IDisposable _loadingCanvasTimer;

		#endregion <<---------- Properties ---------->>

        
        
        
		#region <<---------- Loaders ---------->>

		public static T LoadObject<T>(AssetReference key) {
			return LoadObject<T>(key.RuntimeKey.ToString());
		}
		
        public static T LoadObject<T>(string key) {
            var op = Addressables.LoadAssetAsync<T>(key);
            return op.WaitForCompletion();
        }
        
		public static async Task<T> LoadObjectAsync<T>(string key) {
			Debug.Log($"Loading asset key '{key}'");
			#if UnityAddressables && UniTask
			return await Addressables.LoadAssetAsync<T>(key);
			#else
			throw new NotImplementedException();
			#endif
		}

        public static T LoadAndInstantiateFromResources<T>(string key) where T : UnityEngine.Object {
            if (!Application.isPlaying) {
                Debug.LogError($"Will not load from resources because application is not playing.");
                return null;
            }
            var resource = Resources.Load<T>(key);
            if (resource == null) {
                Debug.LogError($"Could not {nameof(LoadAndInstantiateFromResources)} from key '{key}'");
                return null;
            }
            return Object.Instantiate(resource);
        }

		#if UnityAddressables
        
		public static async Task<T> LoadObjectAsync<T>(AssetReference key) {
			return await LoadObjectAsync<T>(key.RuntimeKey.ToString());
		}
		
		public static CUIViewBase LoadAndInstantiateUI(AssetReference key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
			return LoadAndInstantiateUI(key.RuntimeKey.ToString(), parent, instantiateInWorldSpace, trackHandle);
		}
		
		public static CUIViewBase LoadAndInstantiateUI(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
			#if UnityAddressables
            var ui = LoadAndInstantiate<CUIViewBase>(key, parent, instantiateInWorldSpace, trackHandle);
            Debug.Log($"Created UI menu: '{ui.name}'", ui);
            ui.name = $"[MENU] {ui.name}";
            return ui;
			#else
			throw new NotImplementedException();
			#endif
		}

        public static T LoadAndInstantiate<T>(AssetReference key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) where T : Component  {
            return LoadAndInstantiate<T>(key.RuntimeKey.ToString(), parent, instantiateInWorldSpace, trackHandle);
        }
        
        public static T LoadAndInstantiate<T>(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) where T : Component {
            var go = LoadAndInstantiateGameObject(key, parent, instantiateInWorldSpace, trackHandle);
            Debug.Log($"Loaded gameObject to instantiate '{(go != null ? go.name : "<color=red>is null!</color>")}'");
            return go != null ? go.GetComponent<T>() : default;
        }

        public static GameObject LoadAndInstantiateGameObject(AssetReference key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
            return LoadAndInstantiateGameObject(key.RuntimeKey.ToString(), parent, instantiateInWorldSpace, trackHandle);
        }
        
        public static GameObject LoadAndInstantiateGameObject(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
            var op = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
            return op.WaitForCompletion();
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
			} catch (Exception e) {
				Debug.LogError($"Exception trying to Load and Instantiate GameObject Async with key '{key}':\n" + e);
			}
			#else
			Debug.Log($"UniTask is not installed in project with UnityAddressables, could not Load Asset.");
			#endif

			return null;
		}
		
        #endif
		
		#endregion <<---------- Loaders ---------->>


		

		#region <<---------- Unloaders ---------->>

		public static bool UnloadAsset(GameObject goToUnload, bool destroyIfCantReleaseInstance = true) {
			if (goToUnload == null) {
				Debug.LogError($"Will not unload a null asset.");
				return false;
			}
			#if UnityAddressables
			bool success = Addressables.ReleaseInstance(goToUnload);
			if (success) return true;
			Debug.LogError($"There was something wrong ReleasingInstance of gameObject. '{goToUnload.name}'." + (destroyIfCantReleaseInstance ? " Object will be destroyed." : string.Empty), goToUnload);
			#endif
			goToUnload.CDestroy();
			return destroyIfCantReleaseInstance;
		}
		
		#endregion <<---------- Unloaders ---------->>

	}
}

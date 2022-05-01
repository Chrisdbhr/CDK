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

		public static CAssets get {
			get {
				if (_instance != null) return _instance;
				Debug.Log($"Creating new instance of {nameof(CAssets)}");
				CApplication.QuittingEvent += () => {
					_instance = null;
				};
				return _instance = new CAssets();
			}
		}
		private static CAssets _instance;
		
		private CanvasGroup _loadingCanvas;
		private IDisposable _loadingCanvasTimer;

		private readonly CGameSettings _gameSettings;
		private readonly CLoading _loading;
		
		#endregion <<---------- Properties ---------->>

		
		
		
		#region <<---------- Initializers ---------->>
		
		private CAssets() {
			this._gameSettings = CDependencyResolver.Get<CGameSettings>();
			this._loading = CDependencyResolver.Get<CLoading>();
		}

		#endregion <<---------- Initializers ---------->>

		
		

		#region <<---------- Loaders ---------->>

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
		#endif
		
		#if UnityAddressables
		public static async Task<CUIViewBase> LoadAndInstantiateUI(AssetReference key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
			return await LoadAndInstantiateUI(key.RuntimeKey.ToString(), parent, instantiateInWorldSpace, trackHandle);
		}
		#endif
		
		public static async Task<CUIViewBase> LoadAndInstantiateUI(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
			get._loading.LoadingCanvasRetain();

			try {
				#if UnityAddressables
				var uiGameObject = await LoadAndInstantiateGameObjectAsync(key, parent, instantiateInWorldSpace, trackHandle);
				if (uiGameObject == null) return null;

				var ui = uiGameObject.GetComponent<CUIViewBase>();
				if (ui != null) {
					uiGameObject.name = $"[MENU] {uiGameObject.name}";
					Debug.Log($"Created UI menu: {uiGameObject.name}", uiGameObject);
				}
				else {
					uiGameObject.name = $"[INVALID-MENU] {uiGameObject.name}";
					Debug.LogError($"Created UI gameobject {uiGameObject.name} but it does not inherit from {nameof(CUIViewBase)}", uiGameObject);
					return ui;
				}

				return ui;
				#else
				throw new NotImplementedException();
				#endif
			}
			catch (Exception e) {
				Debug.LogError(e);
			}
			finally {
				get._loading.LoadingCanvasRelease();
			}
			return null;
		}
		
		#if UnityAddressables
		public static async Task<GameObject> LoadAndInstantiateGameObjectAsync(AssetReference key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
			return await LoadAndInstantiateGameObjectAsync(key.RuntimeKey.ToString(), parent, instantiateInWorldSpace, trackHandle);
		}
		#endif
		
		#if UnityAddressables 
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

		public static T LoadAndInstantiateFromResources<T>(string key) where T : UnityEngine.Object {
            if (!Application.isPlaying) return null;
			var resource = Resources.Load<T>(key);
			if (resource == null) {
				Debug.LogError($"Could not {nameof(LoadAndInstantiateFromResources)} from key '{key}'");
				return null;
			}
			return Object.Instantiate(resource);
		}
		
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

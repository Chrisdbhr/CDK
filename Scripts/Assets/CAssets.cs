using System;
using System.Threading.Tasks;
using CDK.UI;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace CDK.Assets {
	public class CAssets {

		#region <<---------- Properties ---------->>

		private static CAssets Instance {
			get {
				if (_instance != null) return _instance;
				Debug.Log($"Creating new instance of {nameof(CAssets)}");
				return _instance = new CAssets();
			}
		}
		private static CAssets _instance;
		
		private static Canvas _loadingCanvas;
		private static CRetainable _loadingRetainable;
		private static TimeSpan TimeToShowLoadingIndicator = TimeSpan.FromSeconds(1);
		private static IDisposable _loadingTimer; 
		
		#endregion <<---------- Properties ---------->>

		
		
		
		#region <<---------- Initializers ---------->>
		
		private async Task CheckForInitialization() {
			await Instance.CheckForLoadingCanvas();
			_loadingRetainable = new CRetainable();
		}

		#endregion <<---------- Initializers ---------->>

		
		

		#region <<---------- Loadings ---------->>

		public static async Task<CUIBase> LoadAndInstantiateUI(AssetReference key) {
			return await LoadAndInstantiateUI(key.RuntimeKey.ToString());
		}
		
		public static async Task<CUIBase> LoadAndInstantiateUI(string key) {
			var uiGameObject = await LoadAndInstantiateGameObjectAsync(key);
			if (uiGameObject == null) return null;
			
			var ui = uiGameObject.GetComponent<CUIBase>();
			if (ui != null) {
				uiGameObject.name = $"[MENU] {uiGameObject.name}";
				Debug.Log($"Created UI menu: {uiGameObject.name}", uiGameObject);
			}
			else {
				uiGameObject.name = $"[INVALID-MENU] {uiGameObject.name}";
				Debug.LogError($"Created UI gameobject {uiGameObject.name} but it does not inherit from {nameof(CUIBase)}", uiGameObject);
				return ui;
			}
			return ui;
		}
		
		public static async Task<GameObject> LoadAndInstantiateGameObjectAsync(string key) {
			await Instance.CheckForInitialization();
			Debug.Log($"Loading asset '{key}'");

			Instance.LoadingRetain();
			
			var asyncOp = new AsyncOperationHandle<GameObject>();
			
			try {
				asyncOp = Addressables.LoadAssetAsync<GameObject>(key);
				await asyncOp.Task;
				if (asyncOp.Result == null) {
					Debug.LogError($"Requested Load Asset key '{key}' but async operation returned null!");
				}
			} catch (Exception e) {
				Debug.LogError(e);
			}
			finally {
				Instance.LoadingRelease();
			}

			var go = Object.Instantiate(asyncOp.Result);
			go.name = asyncOp.Result.name;
			return go;
		}

		#endregion <<---------- Loadings ---------->>

		
		
		
		#region <<---------- Retainables ---------->>
		
		private void LoadingRetain() {
			Debug.Log("Retaining to show loading indicator.");
			_loadingRetainable.Retain();
			_loadingTimer = Observable.Timer(TimeToShowLoadingIndicator).Subscribe(_ => {
				_loadingCanvas.enabled = true;
			});
		}
		private void LoadingRelease() {
			_loadingRetainable.Release();
			if (!_loadingRetainable.IsRetained()) {
				_loadingTimer?.Dispose();
				_loadingCanvas.enabled = false;
				Debug.Log("Released loading indicator.");
			}
		}

		#endregion <<---------- Retainables ---------->>




		#region <<---------- Loading Canvas ---------->>
		private async Task CheckForLoadingCanvas() {
			if (_loadingCanvas != null) return;
			var asyncOp = Addressables.LoadAssetAsync<GameObject>(CGameSettings.AssetRef_UiLoading);
			await asyncOp.Task;
			_loadingCanvas = Object.Instantiate(asyncOp.Result).GetComponent<Canvas>();
			_loadingCanvas.enabled = false;
			Object.DontDestroyOnLoad(_loadingCanvas.gameObject);
		}
		
		#endregion <<---------- Loading Canvas ---------->>

	}
}

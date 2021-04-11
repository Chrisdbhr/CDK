using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CDK.UI;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace CDK {
	public class CAssets {

		#region <<---------- Properties ---------->>

		public static CAssets get {
			get {
				if (_instance != null) return _instance;
				Debug.Log($"Creating new instance of {nameof(CAssets)}");
				CApplication.IsQuitting += () => {
					_instance = null;
				};
				return _instance = new CAssets();
			}
		}
		private static CAssets _instance;
		
		private Task _loadLoadingCanvasTask;
		private Canvas _loadingCanvas;
		private CRetainable _loadingCanvasRetainable;
		private TimeSpan TimeToShowLoadingIndicator = TimeSpan.FromSeconds(1);
		private IDisposable _loadingCanvasTimer; 
		
		#endregion <<---------- Properties ---------->>

		
		
		
		#region <<---------- Initializers ---------->>
		
		private CAssets() {
			this._loadingCanvasRetainable = new CRetainable();
			(this._loadLoadingCanvasTask = this.CheckForLoadingCanvas()).CAwait();
		}

		#endregion <<---------- Initializers ---------->>

		
		

		#region <<---------- Loadings ---------->>

		public static async Task<T> LoadObjectAsync<T>(string key) {
			Debug.Log($"Loading asset '{key}'");
			return await Addressables.LoadAssetAsync<T>(key);
		}

		public static async Task<T> LoadObjectAsync<T>(AssetReference key) {
			return await LoadObjectAsync<T>(key.RuntimeKey.ToString());
		}
		
		
		public static async Task<CUIBase> LoadAndInstantiateUI(AssetReference key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
			return await LoadAndInstantiateUI(key.RuntimeKey.ToString(), parent, instantiateInWorldSpace, trackHandle);
		}
		
		public static async Task<CUIBase> LoadAndInstantiateUI(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
			await get._loadLoadingCanvasTask;

			get.LoadingCanvasRetain();

			try {
				var uiGameObject = await LoadAndInstantiateGameObjectAsync(key, parent, instantiateInWorldSpace, trackHandle);
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
			catch (Exception e) {
				Debug.LogError(e);
			}
			finally {
				get.LoadingCanvasRelease();
			}
			return null;
		}
		
		
		public static async Task<GameObject> LoadAndInstantiateGameObjectAsync(AssetReference key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
			return await LoadAndInstantiateGameObjectAsync(key.RuntimeKey.ToString(), parent, instantiateInWorldSpace, trackHandle);
		}
		
		public static async Task<GameObject> LoadAndInstantiateGameObjectAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
			Debug.Log($"Loading GameObject with key '{key}'");

			try {
				return await Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
			} catch (Exception e) {
				Debug.LogError(e);
			}

			return null;
		}

		#endregion <<---------- Loadings ---------->>


		
		
		#region <<---------- Retainables ---------->>
		
		private void LoadingCanvasRetain() {
			Debug.Log("Retaining to show loading indicator.");
			this._loadingCanvasRetainable.Retain();
			this._loadingCanvasTimer = Observable.Timer(this.TimeToShowLoadingIndicator).Subscribe(_ => {
				if (this._loadingCanvas == null) return;
				this._loadingCanvas.enabled = true;
			});
		}
		private void LoadingCanvasRelease() {
			this._loadingCanvasRetainable.Release();
			if (!this._loadingCanvasRetainable.IsRetained()) {
				this._loadingCanvasTimer?.Dispose();
				this._loadingCanvas.enabled = false;
				Debug.Log("Released loading indicator.");
			}
		}

		#endregion <<---------- Retainables ---------->>




		#region <<---------- Loading Canvas ---------->>
		
		private async Task CheckForLoadingCanvas() {
			if (CApplication.Quitting) return;
			if (this._loadingCanvas != null) return;
			this._loadingCanvas = (await Addressables.InstantiateAsync(CGameSettings.AssetRef_UiLoading)).GetComponent<Canvas>();
			if (CApplication.Quitting) {
				this._loadingCanvas.CDestroy();
				return;
			}
			this._loadingCanvas.enabled = false;
			Object.DontDestroyOnLoad(this._loadingCanvas.gameObject);
		}
		
		#endregion <<---------- Loading Canvas ---------->>

	}
}

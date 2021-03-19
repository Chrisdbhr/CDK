using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CDK.UI;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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
		private CRetainable _loadingRetainable;
		private TimeSpan TimeToShowLoadingIndicator = TimeSpan.FromSeconds(1);
		private IDisposable _loadingTimer; 
		
		#endregion <<---------- Properties ---------->>

		
		
		
		#region <<---------- Initializers ---------->>
		
		private CAssets() {
			this._loadingRetainable = new CRetainable();
			(this._loadLoadingCanvasTask = this.CheckForLoadingCanvas()).CAwait();
		}

		#endregion <<---------- Initializers ---------->>

		
		

		#region <<---------- Loadings ---------->>
		
		public static async Task<CUIBase> LoadAndInstantiateUI(AssetReference key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
			return await LoadAndInstantiateUI(key.RuntimeKey.ToString(), parent, instantiateInWorldSpace, trackHandle);
		}
		
		public static async Task<CUIBase> LoadAndInstantiateUI(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
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
		
		
		public static async Task<GameObject> LoadAndInstantiateGameObjectAsync(AssetReference key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
			return await LoadAndInstantiateGameObjectAsync(key.RuntimeKey.ToString(), parent, instantiateInWorldSpace, trackHandle);
		}
		
		public static async Task<GameObject> LoadAndInstantiateGameObjectAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) {
			await get._loadLoadingCanvasTask;
			Debug.Log($"Loading asset '{key}'");

			get.LoadingRetain();
			
			try {
				return await Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
			} catch (Exception e) {
				Debug.LogError(e);
			}
			finally {
				get.LoadingRelease();
			}

			return null;
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
			if (this._loadingCanvas != null) return;
			this._loadingCanvas = (await Addressables.InstantiateAsync(CGameSettings.AssetRef_UiLoading)).GetComponent<Canvas>();
			this._loadingCanvas.enabled = false;
			Object.DontDestroyOnLoad(this._loadingCanvas.gameObject);
		}
		
		#endregion <<---------- Loading Canvas ---------->>

	}
}

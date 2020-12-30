using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IngameDebugConsole;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CDK {
	public static class CSceneManager {

		#region <<---------- Events ---------->>
		
		public static event Action NewSceneLoaded {
			add {
				_newSceneLoaded -= value;
				_newSceneLoaded += value;
			}
			remove {
				_newSceneLoaded -= value;
			}
		}
		private static event Action _newSceneLoaded;

		#endregion <<---------- Events ---------->>

		private static CSceneEntryPoint[] _sceneEntryPoints;
		

		#region <<---------- Initializers ---------->>
		
		/// <summary>
		/// ANTES da scene load.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeBeforeSceneLoad() {
			DebugLogConsole.AddCommandStatic( "load", "Load scene Single.", nameof(CSceneManager.LoadSceneSingle), typeof(CSceneManager));
		}
		
		/// <summary>
		/// DEPOIS da scene load.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void InitializeAfterSceneLoad() {
			UpdateSceneEntryPointsList();

		}

		#endregion <<---------- Initializers ---------->>


		public static void LoadSceneSingle(string sceneName) {
			SceneManager.LoadSceneAsync(sceneName);
		}

		public static async Task Teleport(string sceneToLoad, int entryPointNumber, IReadOnlyList<GameObject> gameObjectsToTeleport) {

			if (gameObjectsToTeleport == null || !gameObjectsToTeleport.Any()) {
				Debug.LogError($"List of null objects tried to Teleport, canceling operation.");
				return;
			}
			
			CTime.SetTimeScale(0f);
			CBlockingEventsManager.IsPlayingCutscene = true;

			Debug.Log($"Loading scene {sceneToLoad}");
			
			var allLoadedScenes = GetAllLoadedScenes();
			
			
			// Load scenes
			var sceneToLoadAsyncOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
			
			sceneToLoadAsyncOp.allowSceneActivation = false;
			
			await CFadeCanvas.FadeToBlack(0.5f);

			Debug.Log($"TODO implement loading screen");

			
			// Activate loaded scenes
			sceneToLoadAsyncOp.allowSceneActivation = true;

			do {
				await Observable.NextFrame();
			} while (sceneToLoadAsyncOp.progress < 1f);
			
			var targetScene = SceneManager.GetSceneByName(sceneToLoad);
			
			SceneManager.SetActiveScene(targetScene);
			
			foreach (var rootGo in gameObjectsToTeleport) {
				SceneManager.MoveGameObjectToScene(rootGo.gameObject, targetScene);
			}
			
			
			// unload last scenes
			foreach (var loadedScene in allLoadedScenes) {
				var unloadAsyncOp = SceneManager.UnloadSceneAsync(loadedScene, UnloadSceneOptions.None);
				do {
					await Observable.NextFrame();
				} while (unloadAsyncOp.progress < 1f);
			}

			UpdateSceneEntryPointsList();

			foreach (var rootGo in gameObjectsToTeleport) {
				SetTransformToSceneEntryPoint(rootGo.transform, entryPointNumber);
			}

			Debug.Log($"TODO remove loading screen");
			
			_newSceneLoaded?.Invoke();
			
			CBlockingEventsManager.IsPlayingCutscene = false;
			CTime.SetTimeScale(1f);
			await CFadeCanvas.FadeToTransparent(1f);
		}

		public static void SetTransformToSceneEntryPoint(Transform transformToMove, int entryPointNumber = 0) {
			if (transformToMove == null) {
				Debug.LogError("Cant move a null game object.");
				return;
			}

			Transform targetTransform = null;

			if (!_sceneEntryPoints.Any() || entryPointNumber >= _sceneEntryPoints.Length) {
				Debug.LogWarning($"Cant find any level entry point {entryPointNumber} OR it is invalid.");
			}
			else {
				var selectedEntryPoint = _sceneEntryPoints.FirstOrDefault(ep => ep.Number == entryPointNumber);
				if (selectedEntryPoint != null) {
					targetTransform = selectedEntryPoint.transform;
				}
			}

			if (targetTransform == null) {
				transformToMove.position = Vector3.zero;
				transformToMove.rotation = Quaternion.identity;
			}
			else {
				transformToMove.position = targetTransform.position;
				transformToMove.rotation = targetTransform.rotation;
			}
			
			Debug.Log($"Moving {transformToMove.name} to {nameof(entryPointNumber)} at position {transformToMove.position}");
		}

		public static void UpdateSceneEntryPointsList() {
			_sceneEntryPoints = GameObject.FindObjectsOfType<CSceneEntryPoint>();
		}
		

		#region <<---------- Scene All Objects ---------->>
		
		public static Scene[] GetAllLoadedScenes() {
			
			int countLoaded = SceneManager.sceneCount;
			Scene[] loadedScenes = new Scene[countLoaded];
 
			for (int i = 0; i < countLoaded; i++) {
				loadedScenes[i] = SceneManager.GetSceneAt(i);
			}

			return loadedScenes;
		}

		public static List<GameObject> GetAllScenesRootGameObjects() {
			var allLoadedScenes = GetAllLoadedScenes();
			var rootTransformObjects = new List<GameObject>();
			foreach (var loadedScene in allLoadedScenes) {
				rootTransformObjects.AddRange(loadedScene.GetRootGameObjects());
			}

			return rootTransformObjects;
		}
		
		#endregion <<---------- Scene All Objects ---------->>
	}
}

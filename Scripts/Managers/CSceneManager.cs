using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CDK {
	public static class CSceneManager {

		#region <<---------- Initializers ---------->>
		
		/// <summary>
		/// ANTES da scene load.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeBeforeSceneLoad() {
			//DebugLogConsole.AddCommandStatic( "load", "Load scene Single.", nameof(CSceneManager.LoadSceneSingle), typeof(CSceneManager));
		}

		#endregion <<---------- Initializers ---------->>


		public static AsyncOperation LoadSceneSingle(string sceneName) {
			return SceneManager.LoadSceneAsync(sceneName);
		}

		public static async Task Teleport(string sceneToLoad, int entryPointNumber, IReadOnlyList<GameObject> gameObjectsToTeleport) {

			if (gameObjectsToTeleport == null || !gameObjectsToTeleport.Any()) {
				Debug.LogError($"List of null objects tried to Teleport, canceling operation.");
				return;
			}
			
			CBlockingEventsManager.IsPlayingCutscene = true;

			Debug.Log($"Loading scene {sceneToLoad}");
			
			var allLoadedScenes = GetAllLoadedScenes();
			
			
			// Load scenes
			var sceneToLoadAsyncOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
			
			sceneToLoadAsyncOp.allowSceneActivation = false;
			
			await CFadeCanvas.FadeToBlack(0.3f, false);

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

			foreach (var rootGo in gameObjectsToTeleport) {
				SetTransformToSceneEntryPoint(rootGo.transform, entryPointNumber);
			}

			LightProbes.TetrahedralizeAsync();
			
			Debug.Log($"TODO remove loading screen");
			
			CBlockingEventsManager.IsPlayingCutscene = false;

			CSave.get.CurrentMap = targetScene.name;
			
			await Observable.Timer(TimeSpan.FromSeconds(1));

			await CFadeCanvas.FadeToTransparent(1f, false);
		}

		public static void SetTransformToSceneEntryPoint(Transform transformToMove, int entryPointNumber = 0) {
			if (transformToMove == null) {
				Debug.LogError("Cant move a null game object.");
				return;
			}

			Transform targetTransform = null;
			
			var sceneEntryPoints = GameObject.FindObjectsOfType<CSceneEntryPoint>();

			if (!sceneEntryPoints.Any() || entryPointNumber >= sceneEntryPoints.Length) {
				Debug.LogWarning($"Cant find any level entry point {entryPointNumber} OR it is invalid.");
			}
			else {
				var selectedEntryPoint = sceneEntryPoints.FirstOrDefault(ep => ep.Number == entryPointNumber);
				if (selectedEntryPoint != null) {
					targetTransform = selectedEntryPoint.transform;
				}
			}

			var offset = new Vector3(0f, 0.001f, 0f);
			if (targetTransform == null) {
				transformToMove.position = Vector3.zero + offset;
				transformToMove.rotation = Quaternion.identity;
			}
			else {
				transformToMove.position = targetTransform.position + offset;
				transformToMove.rotation = targetTransform.rotation;
			}
			
			Debug.Log($"Moving {transformToMove.name} to {nameof(entryPointNumber)} at position {transformToMove.position}");
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

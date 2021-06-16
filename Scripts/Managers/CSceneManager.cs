using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CDK {
	public class CSceneManager {

		#region <<---------- Initializers ---------->>
		
		/// <summary>
		/// ANTES da scene load.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeBeforeSceneLoad() {
			//DebugLogConsole.AddCommandStatic( "load", "Load scene Single.", nameof(CSceneManager.LoadSceneSingle), typeof(CSceneManager));
		}

		public CSceneManager() {
			this._fader = CDependencyResolver.Get<CFader>();
		}

		#endregion <<---------- Initializers ---------->>


		

		#region <<---------- Properties and Fields ---------->>

		private readonly CFader _fader;
		
		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- Static SceneManager Wrappers ---------->>
		
		public static AsyncOperation LoadSceneSingle(string sceneName) {
			return SceneManager.LoadSceneAsync(sceneName);
		}
		
		#endregion <<---------- Static SceneManager Wrappers ---------->>

		
		

		public async Task Teleport(string sceneToLoadName, int entryPointNumber, IReadOnlyList<GameObject> gameObjectsToTeleport) {

			if (gameObjectsToTeleport == null || !gameObjectsToTeleport.Any()) {
				Debug.LogError($"List of null objects tried to Teleport, canceling operation.");
				return;
			}
			
			CBlockingEventsManager.IsPlayingCutscene = true;

			Debug.Log($"Loading scene '{sceneToLoadName}'");
			
			var allLoadedScenes = GetAllLoadedScenes();
			
			// fade out
			float fadeOutTime = 0.3f;
			this._fader.FadeToBlack(fadeOutTime, false);
			await Observable.Timer(TimeSpan.FromSeconds(fadeOutTime));

			Debug.Log($"TODO implement loading screen");
			
			// move objects to temporary scene
			var tempHolderScene = SceneManager.CreateScene("Temp Holder Scene"); 
			foreach (var rootGo in gameObjectsToTeleport) {
				SceneManager.MoveGameObjectToScene(rootGo, tempHolderScene);
			}
			SceneManager.SetActiveScene(tempHolderScene);

			// unload scenes
			foreach (var sceneToUnload in allLoadedScenes) {
				var unloadAsyncOp = SceneManager.UnloadSceneAsync(sceneToUnload, UnloadSceneOptions.None);
				do {
					await Observable.NextFrame();
				} while (unloadAsyncOp.progress < 1f);
			}

			// background load scene async
			var loadAsyncOp = SceneManager.LoadSceneAsync(sceneToLoadName, LoadSceneMode.Additive);
			loadAsyncOp.allowSceneActivation = false;

			// Activate loaded scenes
			loadAsyncOp.allowSceneActivation = true;
			do {
				await Observable.NextFrame();
			} while (loadAsyncOp.progress < 1f);
			
			// teleport to target scene
			var sceneToTeleport = SceneManager.GetSceneByName(sceneToLoadName);
			SceneManager.SetActiveScene(sceneToTeleport);
			
			// move objects to loaded scene
			foreach (var rootGo in gameObjectsToTeleport) {
				SceneManager.MoveGameObjectToScene(rootGo, sceneToTeleport);
			}

			// move transform to scene entry points
			foreach (var rootGo in gameObjectsToTeleport) {
				SetTransformToSceneEntryPoint(rootGo.transform, entryPointNumber);
			}

			SceneManager.UnloadSceneAsync(tempHolderScene);

			LightProbes.TetrahedralizeAsync();
			
			Debug.Log($"TODO remove loading screen");
			
			CSave.get.CurrentMap = sceneToTeleport.name;

			// fade in
			float fadeInTime = 0.5f;
			this._fader.FadeToTransparent(fadeInTime, false);
			await Observable.NextFrame();

			CBlockingEventsManager.IsPlayingCutscene = false;
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
			
			Debug.Log($"Moving {transformToMove.name} to {nameof(entryPointNumber)}:'{entryPointNumber}' at position {transformToMove.position}", targetTransform);
		}

		
		
		
		#region <<---------- Extensions ---------->>

		public static bool IsSceneValid(string sceneName) {
			Debug.Log("TODO implement method to save scenes on build in a file and check using this file with list from BuildSettings.scenes");
			return true;
			
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
				var scene = SceneManager.GetSceneByBuildIndex(i);
				if (scene.name == sceneName) return true;
			}
			return false;
		}

		#endregion <<---------- Extensions ---------->>
	
		
		
		
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

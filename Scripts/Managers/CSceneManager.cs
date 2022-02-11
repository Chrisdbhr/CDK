using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

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
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
		}

		#endregion <<---------- Initializers ---------->>


		

		#region <<---------- Properties and Fields ---------->>

		private const float MINIMUM_LOADING_TIME = 1f;
		[NonSerialized] private readonly CFader _fader;
		[NonSerialized] private readonly CBlockingEventsManager _blockingEventsManager;

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
			
			this._blockingEventsManager.IsPlayingCutscene = true;

			Debug.Log($"Loading scene '{sceneToLoadName}'");
			
			var allLoadedScenes = GetAllLoadedScenes();
			
			// fade out
			float fadeOutTime = 0.3f;
			this._fader.FadeToBlack(fadeOutTime, false);
			await Observable.Timer(TimeSpan.FromSeconds(fadeOutTime));

			var minimumTimeToReturnFromLoading = Time.time + MINIMUM_LOADING_TIME;

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
			
			// Activate loaded scenes
			loadAsyncOp.allowSceneActivation = true;
			do {
				await Observable.NextFrame();
			} while (loadAsyncOp.progress < 1f);
			
			// teleport to target scene
			var sceneToTeleport = SceneManager.GetSceneByName(sceneToLoadName);
			SceneManager.SetActiveScene(sceneToTeleport);
			
			await Observable.NextFrame();
			// move objects to loaded scene
			foreach (var rootGo in gameObjectsToTeleport) {
				SceneManager.MoveGameObjectToScene(rootGo, sceneToTeleport);
			}
			await Observable.NextFrame();

			// move transform to scene entry points
			foreach (var rootGo in gameObjectsToTeleport) {
				SetTransformToSceneEntryPoint(rootGo.transform, entryPointNumber);
			}
			Physics.SyncTransforms();

			SceneManager.UnloadSceneAsync(tempHolderScene);
			 
			LightProbes.TetrahedralizeAsync();

			Debug.Log($"TODO remove loading screen");
			
			await this.WaitUntilMinimumTimeToReturnFromLoading(minimumTimeToReturnFromLoading);

			// fade in
			float fadeInTime = 0.5f;
			this._fader.FadeToTransparent(fadeInTime, false);

			this._blockingEventsManager.IsPlayingCutscene = false;
		}

		public static void SetTransformToSceneEntryPoint(Transform transformToMove, int entryPointNumber = 0) {
			if (transformToMove == null) {
				Debug.LogError("Cant move a null game object.");
				return;
			}

			Transform targetEntryPointTransform = CSceneEntryPoint.GetSceneEntryPointTransform(entryPointNumber);

			var offset = new Vector3(0f, 0.001f, 0f);
			if (targetEntryPointTransform == null) {
				transformToMove.position = Vector3.zero + offset;
				transformToMove.rotation = Quaternion.identity;
			}
			else {
				var character = transformToMove.GetComponent<CCharacterBase>();
				if (character != null) {
					character.TeleportToLocation(targetEntryPointTransform.position, targetEntryPointTransform.rotation);					
				}
				else {
					transformToMove.position = targetEntryPointTransform.position + offset;
					transformToMove.rotation = targetEntryPointTransform.rotation;
				}
			}
			
			Debug.Log($"Moving {transformToMove.name} to {nameof(entryPointNumber)}:'{entryPointNumber}' at position {transformToMove.position}", targetEntryPointTransform);
		}

		private async Task WaitUntilMinimumTimeToReturnFromLoading(float timeToReturnFromLoading) {
			var currentTime = Time.time;
			if (currentTime >= timeToReturnFromLoading) return;
			var secondsToWait = timeToReturnFromLoading - currentTime;
			Debug.Log($"Will wait {secondsToWait} more seconds to scene load.");
			await Observable.Timer(TimeSpan.FromSeconds(secondsToWait));
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

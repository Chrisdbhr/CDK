using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using IngameDebugConsole;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

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
			
			//var allEntryPoints = GameObject.FindObjectsOfType<CSceneEntryPoint>();
		
		}

		#endregion <<---------- Initializers ---------->>


		public static void LoadSceneSingle(string sceneName) {
			SceneManager.LoadSceneAsync(sceneName);
		}

		public static async Task Teleport(string sceneToLoad, int entryPointNumber, IEnumerable<GameObject> gameObjectsToTeleport) {

			CTime.SetTimeScale(0f);
			CBlockingEventsManager.IsPlayingCutscene = true;

			Debug.Log($"Loading scene {sceneToLoad}");
			
			// load scenes
			var allLoadedScenes = GetAllLoadedScenes();
			
			var sceneToLoadAsyncOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
			
			sceneToLoadAsyncOp.allowSceneActivation = false;
			
			await CFadeCanvas.FadeToBlack(1f);

			Debug.Log($"TODO implement loading screen");
			
			await sceneToLoadAsyncOp;
			
			
			
			foreach (var rootGo in gameObjectsToTeleport.Select(go => go.transform.root)) {
				PositionTransformToSceneEntryPoint(rootGo.transform, entryPointNumber);
			}

			sceneToLoadAsyncOp.allowSceneActivation = true;

			while (sceneToLoadAsyncOp.progress < 1f) {
				await Observable.NextFrame();
			}

			SceneManager.SetActiveScene(new Scene {name = sceneToLoad});

			// unload last scenes
			foreach (var loadedScene in allLoadedScenes) {
				await SceneManager.UnloadSceneAsync(loadedScene, UnloadSceneOptions.None);
			}

			Debug.Log($"TODO remove loading screen");
			
			_newSceneLoaded?.Invoke();
			
			CBlockingEventsManager.IsPlayingCutscene = false;
			CTime.SetTimeScale(1f);
			await CFadeCanvas.FadeToTransparent(1f);
		}

		public static void PositionTransformToSceneEntryPoint(Transform transf, int entryPointNumber = 0) {
			if (transf == null) {
				Debug.LogError("Cant move a null game object.");
				return;
			}

			var targetPos = Vector3.zero;

			var allEntryPoints = GameObject.FindObjectsOfType<CSceneEntryPoint>();
			if (!allEntryPoints.Any() || entryPointNumber >= allEntryPoints.Length) {
				Debug.LogWarning($"Cant find any level entry point {entryPointNumber} OR it is invalid.");
			}
			else {
				var selectedEntryPoint = allEntryPoints.FirstOrDefault(ep => ep.Number == entryPointNumber);
				if (selectedEntryPoint != null) {
					targetPos = selectedEntryPoint.transform.position;
				}
			}

			transf.position = targetPos;
			Debug.Log($"Moving {transf.name} to {nameof(entryPointNumber)} at position {targetPos}");
		}


		#region <<---------- Static ---------->>
		
		public static Scene[] GetAllLoadedScenes() {
			
			int countLoaded = SceneManager.sceneCount;
			Scene[] loadedScenes = new Scene[countLoaded];
 
			for (int i = 0; i < countLoaded; i++)
			{
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
		
		#endregion <<---------- Static ---------->>
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#if UNITY_2020
using UnityEditor.Experimental.SceneManagement;
#endif
#endif

namespace CDK {
	public class CSceneManager {

		#region <<---------- Initializers ---------->>
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void InitializeAfterSceneLoad() {
            
        }

		public CSceneManager() {
			this._fader = CDependencyResolver.Get<CFader>();
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
            LoadedSceneThisFrame = false;
            SceneManager.activeSceneChanged += ActiveSceneChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            
            LightProbes.needsRetetrahedralization += OnLightProbesNeedsRetetrahedralization;
        }
        
        ~CSceneManager() {
            this._boolSceneLoadedDisposable?.Dispose();
            SceneManager.activeSceneChanged -= ActiveSceneChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
           
            LightProbes.needsRetetrahedralization -= OnLightProbesNeedsRetetrahedralization;
        }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

        public static bool LoadedSceneThisFrame { get; private set; }
        private IDisposable _boolSceneLoadedDisposable;
        
		private const float MINIMUM_LOADING_TIME = 1f;
		private readonly CFader _fader;
		private readonly CBlockingEventsManager _blockingEventsManager;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- Static SceneManager Wrappers ---------->>
		
		public static AsyncOperation LoadSceneSingle(string sceneName) {
			return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
		}
		
		#endregion <<---------- Static SceneManager Wrappers ---------->>




        #region <<---------- General ---------->>

        public async Task Teleport(string sceneToLoadName, int entryPointNumber, IReadOnlyList<GameObject> gameObjectsToTeleport) {

			if (gameObjectsToTeleport == null || !gameObjectsToTeleport.Any()) {
				Debug.LogError($"List of null objects tried to Teleport, canceling operation.");
				return;
			}
			
			this._blockingEventsManager.PlayingCutsceneRetainable.Retain(this);

            var allLoadedScenes = GetAllLoadedScenes();

            CTime.TimeScale = 0f;

            // fade out
			float fadeOutTime = 0.4f;
			this._fader.FadeToBlack(fadeOutTime, true);
            await Task.Delay(TimeSpan.FromSeconds(fadeOutTime));
            
			var minimumTimeToReturnFromLoading = Time.realtimeSinceStartup + MINIMUM_LOADING_TIME;

			Debug.Log($"TODO implement loading screen");
			
			// move objects to temporary scene
			var tempHolderScene = SceneManager.CreateScene("Temp Holder Scene"); 
			foreach (var rootGo in gameObjectsToTeleport) {
				SceneManager.MoveGameObjectToScene(rootGo, tempHolderScene);
			}
			SceneManager.SetActiveScene(tempHolderScene);

            // unload scenes
			foreach (var sceneToUnload in allLoadedScenes) {
                await SceneManager.UnloadSceneAsync(sceneToUnload, UnloadSceneOptions.None).AsObservable();
			}

            // ONLY load scene after the previous one is fully unloaded because UnityBug.
            Debug.Log($"Loading scene '{sceneToLoadName}'");
            await SceneManager.LoadSceneAsync(sceneToLoadName, LoadSceneMode.Additive).AsObservable();
			
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

			await SceneManager.UnloadSceneAsync(tempHolderScene).AsObservable();
            
			Debug.Log($"TODO remove loading screen");
			
            await Observable.NextFrame();
			await this.WaitUntilMinimumTimeToReturnFromLoading(minimumTimeToReturnFromLoading);
            
            await Observable.NextFrame();
            
            CTime.TimeScale = 1f;

            await Observable.NextFrame();

            float fadeInTime = 0.8f;
            await Task.Delay(TimeSpan.FromSeconds(fadeInTime));

            // fade in
            this._fader.FadeToTransparent(fadeInTime, true);
            
            this._blockingEventsManager.PlayingCutsceneRetainable.Release(this);
        }

        public static void SetTransformToSceneEntryPoint(Transform transformToMove, int entryPointNumber = 0) {
            if (transformToMove == null) {
                Debug.LogError("Cant move a null game object.");
                return;
            }
            
            var targetEntryPointTransform = CSceneEntryPoint.GetSceneEntryPointTransformByNumber(entryPointNumber);
            var offset = new Vector3(0f, 0.01f, 0f);
            var targetPos = Vector3.zero + offset;
            var targetRotation = Quaternion.identity;
            float snapDistance = 1f;

            if (targetEntryPointTransform != null) {
                bool hitGround = Physics.Raycast(
                    targetEntryPointTransform.position + (Vector3.up * snapDistance),
                    Vector3.down,
                    out var hitInfo,
                    snapDistance * 2f,
                    1,
                    QueryTriggerInteraction.Ignore
                );
                targetPos = (hitGround ? hitInfo.point : targetEntryPointTransform.position);
                targetRotation = targetEntryPointTransform.rotation;
            }

            var character = transformToMove.GetComponent<CCharacter_Base>();
            if (character != null) {
                character.TeleportToLocation(targetPos, targetRotation);					
            }
            else {
                transformToMove.position = targetPos;
            }
            transformToMove.rotation = targetRotation;
			
            Debug.Log($"Moving {transformToMove.name} to {nameof(entryPointNumber)}:'{entryPointNumber}' at position {transformToMove.position}", targetEntryPointTransform);
        }

        private void CheckToRetainSceneChangedBool() {
            if (this._boolSceneLoadedDisposable != null) return;
            Debug.Log($"Setting {nameof(LoadedSceneThisFrame)} value to true.");
            LoadedSceneThisFrame = true;
            this._boolSceneLoadedDisposable = Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(_ => {
                Debug.Log($"Setting {nameof(LoadedSceneThisFrame)} value to false.");
                LoadedSceneThisFrame = false;
                this._boolSceneLoadedDisposable?.Dispose();
                Debug.Log($"{nameof(_boolSceneLoadedDisposable)} is '{this._boolSceneLoadedDisposable}'");
            });
        }
        
        #endregion <<---------- General ---------->>




        #region <<---------- Callbacks ---------->>

        private void ActiveSceneChanged(Scene oldScene, Scene newScene) {
            CheckToRetainSceneChangedBool();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            
        }
        
        private void OnSceneUnloaded(Scene scene) {    
            
        }
        
        private void OnLightProbesNeedsRetetrahedralization() {
            LightProbes.TetrahedralizeAsync();
        }

        #endregion <<---------- Callbacks ---------->>

        

        
        #region <<---------- Loading ---------->>

        private async Task WaitUntilMinimumTimeToReturnFromLoading(float timeToReturnFromLoading) {
			Debug.Log($"Waiting until minimum time to return from loading is reached.");
            while (Time.realtimeSinceStartup <= timeToReturnFromLoading) {
                await Observable.NextFrame();
            }
		}

        #endregion <<---------- Loading ---------->>

        


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




		#region <<---------- Editor ---------->>

		public static void EditorSetSceneExpanded(Scene scene, bool expand) {
			#if UNITY_EDITOR
			try {
				if (PrefabStageUtility.GetCurrentPrefabStage()) return;
				if (!scene.IsValid()) {
					Debug.LogWarning($"Cannot set expanded state of an invalid scene: {scene}");
					return;
				}
				foreach (var window in Resources.FindObjectsOfTypeAll<SearchableEditorWindow>()) {
					if (window != null && window.GetType().Name != "SceneHierarchyWindow") continue;

					var method = window.GetType().GetMethod("SetExpandedRecursive",
						System.Reflection.BindingFlags.Public |
						System.Reflection.BindingFlags.NonPublic |
						System.Reflection.BindingFlags.Instance, null,
						new[] { typeof(int), typeof(bool) }, null);

					if (method == null) {
						Debug.LogError("Could not find method 'UnityEditor.SceneHierarchyWindow.SetExpandedRecursive(int, bool)'.");
						return;
					}

					var field = scene.GetType().GetField("m_Handle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

					if (field == null) {
						Debug.LogError("Could not find field 'int UnityEngine.SceneManagement.Scene.m_Handle'.");
						return;
					}

					var sceneHandle = field.GetValue(scene);
					method.Invoke(window, new[] { sceneHandle, expand });
				}
				
			} catch (Exception e) {
				Debug.LogError(e);
			}
			#endif
		}

		#endregion <<---------- Editor ---------->>

	}
}

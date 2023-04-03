using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#if UNITY_2020
using UnityEditor.Experimental.SceneManagement;
#else
using UnityEditor.SceneManagement;
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
            this._loading = CLoadingCanvas.get;
          
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
        
		private readonly CFader _fader;
        private readonly CBlockingEventsManager _blockingEventsManager;
        private readonly CLoadingCanvas _loading;

        public bool IsTeleporting => this._isTeleporting;
        private bool _isTeleporting;

        public const float ExtraSecondsAfterTeleport = 1f;

        public const string TemporarySceneName = "Temp Holder Scene";

        private static GameObject _dontDestroyOnLoadHelperObject;
        
		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- Static SceneManager Wrappers ---------->>
		
		public static AsyncOperation LoadSceneSingle(string sceneName) {
			return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
		}
		
		#endregion <<---------- Static SceneManager Wrappers ---------->>




        #region <<---------- General ---------->>

        public bool Teleport(string sceneToLoadName, int entryPointNumber, IReadOnlyList<GameObject> gameObjectsToTeleport, CCameraTransitionType cameraTransitionType) {
            if (this._isTeleporting) {
                Debug.LogError($"[Teleport] Some script tried to Teleport while already teleporting!");
                return false;
            }
            TeleportAsync(sceneToLoadName, entryPointNumber, gameObjectsToTeleport, cameraTransitionType).CAwait();
            return true;
        }
        
        private async Task TeleportAsync(string sceneToLoadName, int entryPointNumber, IReadOnlyList<GameObject> gameObjectsToTeleport, CCameraTransitionType cameraTransitionType) {
            if (gameObjectsToTeleport.CIsNullOrEmpty()) {
				Debug.LogError($"[Teleport] Tried to Teleport a list of null objects, canceling operation.");
				return;
			}

            this._isTeleporting = true;

            try {
                Debug.Log($"[Teleport] Starting TeleportAsync");
                bool doTransition = cameraTransitionType != CCameraTransitionType.none;
			    
			    this._blockingEventsManager.PlayingCutsceneRetainable.Retain(this);

                var allLoadedScenes = GetAllLoadedScenes();

                CTime.TimeScale = 0f;

                float fadeOutTime = doTransition ? 0.4f : 0f;
                Debug.Log($"[Teleport] Will request fade to black");
                this._fader.FadeToBlack(fadeOutTime, true);
                if (doTransition) {
                    await Task.Delay(TimeSpan.FromSeconds(fadeOutTime));
                }
                
                Debug.Log($"[Teleport] Will wait next frame");
                await Observable.NextFrame();

			    // move objects to temporary scene
                Debug.Log($"[Teleport] Creating temporary holder scene");
			    var tempHolderScene = SceneManager.CreateScene(TemporarySceneName);
                MoveGameObjectsToScene(gameObjectsToTeleport, tempHolderScene);
			    SceneManager.SetActiveScene(tempHolderScene);

                // unload scenes
                Debug.Log($"[Teleport] Unloading all scenes");
			    foreach (var sceneToUnload in allLoadedScenes) {
                    await SceneManager.UnloadSceneAsync(sceneToUnload, UnloadSceneOptions.None).AsObservable();
			    }

                // IMPORTANT: Only load scene after the previous one is fully unloaded because UnityBug.
                
                Debug.Log($"[Teleport] Loading scene '{sceneToLoadName}'");

                // Load scene
                await this._loading.MonitorAsyncOperation(SceneManager.LoadSceneAsync(sceneToLoadName, LoadSceneMode.Additive)).AsObservable();
			    
			    // teleport to target scene
			    var sceneToTeleport = SceneManager.GetSceneByName(sceneToLoadName);
			    SceneManager.SetActiveScene(sceneToTeleport);
                
                // move objects to loaded scene
			    foreach (var rootGo in gameObjectsToTeleport) {
				    MoveGameObjectToScene(rootGo, sceneToTeleport);
			    }
                
                (new GameObject("--- Teleported Objects")).transform.SetAsLastSibling();

			    // move transform to scene entry points
			    foreach (var rootGo in gameObjectsToTeleport) {
                    var t = rootGo.transform;
				    SetTransformToSceneEntryPoint(t, entryPointNumber);
                    t.SetAsLastSibling();
			    }
                
                Debug.Log($"[Teleport] Calling Physics.SyncTransforms()");
                Physics.SyncTransforms();

			    await SceneManager.UnloadSceneAsync(tempHolderScene).AsObservable();
                
                CTime.TimeScale = 1f;

                var brains = GameObject.FindObjectsOfType<CinemachineBrain>(false);
                foreach (var b in brains) {
                    b.enabled = false;
                }
                
                Debug.Log($"[Teleport] Will wait next frame");
                await Observable.NextFrame();

                foreach (var b in brains) {
                    b.enabled = true;
                }
                
                // Debug.Log($"[Teleport] Will wait next frame");
                // await Observable.NextFrame();
                Debug.Log($"[Teleport] Will {ExtraSecondsAfterTeleport} seconds before fadein");
                await Observable.Timer(TimeSpan.FromSeconds(ExtraSecondsAfterTeleport)); 
                
                Debug.Log($"[Teleport] Will request fade to transparent");
                this._fader.FadeToTransparent(doTransition ? 0.8f : 0f, true);

                this._blockingEventsManager.PlayingCutsceneRetainable.Release(this);
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
            finally {
                this._isTeleporting = false;
            }
            Debug.Log($"[Teleport] Ended teleport task");
        }

        public static void SetTransformToSceneEntryPoint(Transform transformToMove, int entryPointNumber = 0) {
            if (transformToMove == null) {
                Debug.LogError("Cant move a null game object.");
                return;
            }
            
            var targetEntryPoint = CSceneEntryPoint.GetSceneEntryPointByNumber(entryPointNumber);
            targetEntryPoint.SetIsSelected();
            var targetEntryPointTransform = targetEntryPoint.transform;
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
            Resources.UnloadUnusedAssets();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            
        }
        
        private void OnSceneUnloaded(Scene scene) {    
            
        }
        
        private void OnLightProbesNeedsRetetrahedralization() {
            LightProbes.TetrahedralizeAsync();
        }

        #endregion <<---------- Callbacks ---------->>


        

		#region <<---------- Extensions ---------->>

        [Obsolete("Not implemented", true)]
		public static bool IsSceneValid(string sceneName) {
			Debug.Log("TODO implement method to save scenes on build in a file and check using this file with list from BuildSettings.scenes");
			return true;
			
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
				var scene = SceneManager.GetSceneByBuildIndex(i);
				if (scene.name == sceneName) return true;
			}
			return false;
		}

        public static Scene GetDontDestroyOnLoadScene() {
            if (_dontDestroyOnLoadHelperObject != null) {
                return _dontDestroyOnLoadHelperObject.scene;
            }
            if (CApplication.IsQuitting) {
                return default;
            }
            _dontDestroyOnLoadHelperObject = (new GameObject("Dont Destroy On Load - Helper"));
            Object.DontDestroyOnLoad(_dontDestroyOnLoadHelperObject);
            return _dontDestroyOnLoadHelperObject.scene;
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

        public static void MoveGameObjectsToScene(IReadOnlyList<GameObject> gameObjects, Scene scene) {
            foreach (var go in gameObjects) {
                MoveGameObjectToScene(go, scene);
            }
        }

        /// <summary>
        /// Returns TRUE if success moving object.
        /// </summary>
        public static bool MoveGameObjectToScene(GameObject go, Scene scene) {
            try { 
                if (go == null) {
                    return false;
                }
                if (go.transform.parent != null) {
                    Debug.LogWarning($"Will not Move '{go.name}' ToScene because its not root, its root is '{go.transform.parent.name}'");
                    return false;
                }
                SceneManager.MoveGameObjectToScene(go, scene);
            }
            catch (Exception e) {
                Debug.LogError(e);
                return false;
            }
            return true;
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

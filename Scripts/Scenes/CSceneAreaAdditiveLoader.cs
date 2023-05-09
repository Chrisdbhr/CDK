using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#if UNITY_2020
using UnityEditor.Experimental.SceneManagement;
#endif
#endif

namespace CDK {
	[ExecuteInEditMode]
	public class CSceneAreaAdditiveLoader : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private CSceneField _scene;
		[SerializeField][CTagSelector] private string _tag = "Player";

        private const string SceneNamePrefix = "A - ";

		private HashSet<Collider> _triggerObjectsInside = new HashSet<Collider>();
		
		private bool AnyTriggerObjectInside => this._triggerObjectsInside.Count > 0;

        [SerializeField] private UnityEvent OnSceneLoaded;
        [SerializeField] private UnityEvent OnSceneUnloaded;
		[SerializeField] private bool _pauseUnloadOnEditor;

		private AsyncOperation _loadAsyncOperation;
		private AsyncOperation _unloadAsyncOperation;

		private int _editorUpdateFrameSkips = 10;
		
		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>
		
		private void Awake() {
			this._triggerObjectsInside.Clear();
			this.EnsureColliderIsTrigger();
			#if UNITY_EDITOR
			if (!Application.isPlaying && this._scene != null && this._scene.sceneAsset != null) {
				EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(this._scene.sceneAsset), OpenSceneMode.AdditiveWithoutLoading);
			}
			#endif
		}

		private void OnEnable() {
			#if UNITY_EDITOR
			EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
			#endif
		}

		private void Update() {
			#if UNITY_EDITOR
			if (Time.frameCount % this._editorUpdateFrameSkips != 0) return;
			this.EditorCheckForObject();
			#endif
		}

		#if UNITY_EDITOR
		private void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange newState) {
			this.UnloadScene();
		}
		#endif

		private void OnDisable() {
			#if UNITY_EDITOR
			EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
			#endif
		}

		#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			var t = this.transform;
			var tPos = t.position;
			
			// color
			Gizmos.color = new Color(0f,200f,0f, 0.1f);
			Handles.Label(tPos, t.name);
		}

		void Reset() {
			this.EnsureColliderIsTrigger();
		}

		private void OnValidate() {
			this.EnsureColliderIsTrigger();
		}

		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>
		
		
		

		#region <<---------- General ---------->>
		
		#if UNITY_EDITOR
		void EditorCheckForObject() {
			if (Application.isPlaying || PrefabStageUtility.GetCurrentPrefabStage() || SceneView.lastActiveSceneView == null && SceneView.lastActiveSceneView.camera == null) return;
			var editorCameraTransform = SceneView.lastActiveSceneView.camera.transform;
			if (!this.TryGetComponent<Collider>(out var c)) return;
			if (c.bounds.Contains(editorCameraTransform.position)) {
				this.LoadScene();
			}
			else {
				this.UnloadScene();
			}
		}
		#endif
		
		void EnsureColliderIsTrigger() {
			if (!TryGetComponent<Collider>(out var c)) return;
			if (c.isTrigger) return;
			c.isTrigger = true;
			#if UNITY_EDITOR
			EditorUtility.SetDirty(c);
			#endif
		}
		
		void OnObjectInsideChanged() {
			if (this.AnyTriggerObjectInside) {
				this.LoadScene();
			}
			else {
				this.UnloadScene();
			}
		}

		[EasyButtons.Button]
		void LoadScene() {
			if (this._scene == null) return;
			#if UNITY_EDITOR
			if (!Application.isPlaying) {
				var scenePath = AssetDatabase.GetAssetOrScenePath(this._scene.sceneAsset);
				if (PrefabStageUtility.GetCurrentPrefabStage() != null || EditorSceneManager.GetSceneByPath(scenePath).isLoaded) return;
				var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
				if (scene.isLoaded) {
					Debug.Log($"Scene loaded: {scenePath}");
					CSceneManager.EditorSetSceneExpanded(scene, false);
				}
				return;
			}
			#endif
			
			if (this._loadAsyncOperation != null) return;
			this._loadAsyncOperation = SceneManager.LoadSceneAsync(this._scene, LoadSceneMode.Additive);
			if (this._loadAsyncOperation == null) return;
			this._loadAsyncOperation.completed += operation => {
				this.OnSceneLoaded?.Invoke();
				this._loadAsyncOperation = null;
			};
		}

		[EasyButtons.Button]
		void UnloadScene() {
			if (this._scene == null) return;
			#if UNITY_EDITOR
			if (!Application.isPlaying && !PrefabStageUtility.GetCurrentPrefabStage()) {
				if (this._pauseUnloadOnEditor) return;
				var loadedScene = EditorSceneManager.GetSceneByName(this._scene);
				if (!loadedScene.isLoaded) return;
				if (loadedScene.isDirty) {
					Debug.LogWarning($"Will not unload on editor scene '{loadedScene.name}' because it isDirty.", this);
					return;
				}

				if (EditorSceneManager.CloseScene(loadedScene, false)) {
					Debug.Log($"Scene closed: {this._scene.sceneAsset}");
				}
				return;
			}
			#endif

			if (this._unloadAsyncOperation != null) return;
			var scene = SceneManager.GetSceneByName(this._scene);
			if (!scene.isLoaded) return;
			this._unloadAsyncOperation = SceneManager.UnloadSceneAsync(scene);
			if (this._unloadAsyncOperation == null) return;
			this._unloadAsyncOperation.completed += operation => {
				this.OnSceneUnloaded?.Invoke();
				this._unloadAsyncOperation = null;
			};
		}

        #if UNITY_EDITOR
        [EasyButtons.Button]
        void CreateSceneAsset() {
            var activeScene = EditorSceneManager.GetActiveScene();
            var scenePrefix = SceneNamePrefix + activeScene.name;
            var sceneName = this.gameObject.name;
            if (!sceneName.StartsWith(scenePrefix)) {
                this.gameObject.name = scenePrefix + " - " + this.gameObject.name;
                EditorUtility.SetDirty(this.gameObject);
            }
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            try {
                scene.name = sceneName;
                var scenePath = this.gameObject.scene.path.Replace(this.gameObject.scene.name + ".unity", "");
                var scenePathAndName = scenePath + sceneName + ".unity";
                if (!EditorSceneManager.SaveScene(scene, scenePathAndName)) return;
                this._scene.sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePathAndName);
                if (this._scene.sceneAsset == null) return;
                Selection.activeObject = this._scene.sceneAsset;
                var list = EditorBuildSettings.scenes.ToList();
                list.Add(new EditorBuildSettingsScene {
                    path = scenePathAndName.Replace(".unity", ""), enabled = true, guid = AssetDatabase.GUIDFromAssetPath(scenePathAndName)
                });
                EditorUtility.SetDirty(this);
                AssetDatabase.Refresh();
            }
            catch (Exception e) {
                Debug.LogException(e);
                EditorSceneManager.CloseScene(scene, true);
            }
            finally {
                EditorSceneManager.SetActiveScene(activeScene);
            }
            
        }
        #endif
        
		#endregion <<---------- General ---------->>




		#region <<---------- Physics ---------->>
		
		private void OnTriggerEnter(Collider other) {
			if (!other.CompareTag(this._tag)) return;
			if(!this._triggerObjectsInside.Add(other)) return;
			OnObjectInsideChanged();
		}

		private void OnTriggerExit(Collider other) {
			if (!other.CompareTag(this._tag)) return;
			if(!this._triggerObjectsInside.Remove(other)) return;
			OnObjectInsideChanged();
		}
		
		#endregion <<---------- Physics ---------->>

	}
}

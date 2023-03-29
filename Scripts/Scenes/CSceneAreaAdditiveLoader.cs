using System;
using System.Linq;
using UnityEngine;
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

        enum CheckType {
            bounds, distance
        }
        
		[SerializeField] private LayerMask _triggerLayer = 1;
		[SerializeField] private CSceneField _scene;
		[SerializeField][CTagSelector] private string _tag = "Player";
        [SerializeField] private CheckType _checkType = CheckType.distance;

        private const string SceneNamePrefix = "A - ";

		private bool AnyTriggerObjectInside {
			get { return this._anyTriggerObjectInside; }
			set {
				if (value == this._anyTriggerObjectInside) return;
				this._anyTriggerObjectInside = value;
				OnObjectInsideChanged(value);
			}
		}
		[SerializeField] private bool _anyTriggerObjectInside;
		

		private AsyncOperation _loadAsyncOperation;
		private AsyncOperation _unloadAsyncOperation;

		[NonSerialized] private bool _editorSceneIsDirty;
		private int _updateFrameSkips = 5;
        private Collider[] _overlapResults; 
		
		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>
		
		private void Awake() {
			this._anyTriggerObjectInside = false;
			this._loadAsyncOperation = null;
			this._unloadAsyncOperation = null;
			#if UNITY_EDITOR
			if (!Application.isPlaying && this._scene != null && this._scene.sceneAsset != null) {
				EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(this._scene.sceneAsset), OpenSceneMode.AdditiveWithoutLoading);
			}
			#endif
            this.CheckForObject();
		}

		private void OnEnable() {
			#if UNITY_EDITOR
			EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
			#endif
		}

		private void Update() {
			#if !UNITY_EDITOR
			if (Time.frameCount % this._updateFrameSkips != 0) return;
			#endif
			this.CheckForObject();
		}

		#if UNITY_EDITOR
		private void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange newState) {
			this.UnloadScene();
		}
		#endif

		private void OnDisable() {
			this._anyTriggerObjectInside = false;
			
			#if UNITY_EDITOR
			EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
			#endif
		}

		#if UNITY_EDITOR
		private void OnDrawGizmos() {
			var t = this.transform;
			var tPos = t.position;
			
			// color
			Gizmos.color = new Color(0f,200f,0f, 0.1f);
			Handles.Label(tPos, t.name);
			if (this._editorSceneIsDirty) {
				Gizmos.color = Color.yellow;
			}

            switch (this._checkType) {
                case CheckType.bounds:
                    if (this._anyTriggerObjectInside) {
                        Gizmos.DrawWireCube(tPos, t.localScale);
                    }
                    else {
                        Gizmos.DrawCube(tPos, t.localScale);
                    }
                    break;
                case CheckType.distance:
                    if (this._anyTriggerObjectInside) {
                        Gizmos.DrawWireSphere(tPos, t.localScale.x);
                    }
                    else {
                        Gizmos.DrawSphere(tPos, t.localScale.x);
                    }
                    break;
            }
			
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>
		
		
		

		#region <<---------- General ---------->>
		
		void CheckForObject() {
			var t = this.transform;
				
			#if UNITY_EDITOR
			if (!Application.isPlaying && !PrefabStageUtility.GetCurrentPrefabStage() && SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null) {
				var editorCameraTransform = SceneView.lastActiveSceneView.camera.transform;
                var bounds = new Bounds(t.position, t.localScale);
				this.AnyTriggerObjectInside = bounds.Contains(editorCameraTransform.position);
				return;
			}
			#endif

            switch (this._checkType) {
                case CheckType.bounds:
                    var tRotation = t.rotation;
					this._overlapResults = Physics.OverlapBox(t.position, t.localScale*0.5f, tRotation, this._triggerLayer);                    
                    break;
                case CheckType.distance:
					this._overlapResults = Physics.OverlapSphere(t.position, t.localScale.x, this._triggerLayer);
                    break;
            }
            
            this.AnyTriggerObjectInside = this._overlapResults.Length > 0 && this._overlapResults.Any(c => c != null && c.CompareTag(_tag));
		}

		void OnObjectInsideChanged(bool isInside) {
			if (isInside) {
				this.LoadScene();
			}
			else {
				this.UnloadScene();
			}
		}
		
		void LoadScene() {
			#if UNITY_EDITOR
			if (!Application.isPlaying && !PrefabStageUtility.GetCurrentPrefabStage()) {
				var scene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(this._scene.sceneAsset), OpenSceneMode.Additive);
				CSceneManager.EditorSetSceneExpanded(EditorSceneManager.GetSceneByName(this._scene), false);
				return;
			}
			#endif
			
			if (this._loadAsyncOperation != null) return;
			this._loadAsyncOperation = SceneManager.LoadSceneAsync(this._scene, LoadSceneMode.Additive);
			if (this._loadAsyncOperation == null) return;
			this._loadAsyncOperation.completed += operation => {
				this._loadAsyncOperation = null;
			};
		}

		void UnloadScene() {
			#if UNITY_EDITOR
			if (!Application.isPlaying && !PrefabStageUtility.GetCurrentPrefabStage()) {
				var loadedScene = EditorSceneManager.GetSceneByName(this._scene);
				this._editorSceneIsDirty = loadedScene.isDirty;
				if (this._editorSceneIsDirty) {
					Debug.LogWarning($"Will not unload on editor scene '{loadedScene.name}' because it isDirty.", this);
					return;
				}
				EditorSceneManager.CloseScene(loadedScene, false);
				return;
			}
			#endif

			if (this._unloadAsyncOperation != null) return;
			var scene = SceneManager.GetSceneByName(this._scene);
			if (!scene.isLoaded) return;
			this._unloadAsyncOperation = SceneManager.UnloadSceneAsync(scene);
			if (this._unloadAsyncOperation == null) return;
			this._unloadAsyncOperation.completed += operation => {
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

	}
}

using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace CDK {
	[ExecuteInEditMode]
	public class CSceneAreaAdditiveLoader : MonoBehaviour {
		
		[SerializeField] private LayerMask _triggerLayer;
		[SerializeField] private CSceneField _scene;
		[SerializeField][TagSelector] private string _tag = "Player";
		

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
		private IDisposable _checkDisposable;
		

		#region <<---------- MonoBehaviour ---------->>
		
		private void Awake() {
			this._anyTriggerObjectInside = false;
			this._loadAsyncOperation = null;
			this._unloadAsyncOperation = null;
		}

		private void OnEnable() {
			_checkDisposable?.Dispose();
			_checkDisposable = Observable.Timer(TimeSpan.FromSeconds(0.1f),TimeSpan.FromSeconds(0.5f)).Subscribe(_ => {
				this.CheckForObject();
			});
			
			#if UNITY_EDITOR
			EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
			#endif
		}

		private void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange newState) {
			this.UnloadScene();
		}

		private void OnDisable() {
			_checkDisposable?.Dispose();
			this._anyTriggerObjectInside = false;
			
			#if UNITY_EDITOR
			EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
			#endif
		}
		
		private void OnDestroy() {
			_checkDisposable?.Dispose();
		}

		#if UNITY_EDITOR
		private void OnDrawGizmos() {
			var t = this.transform;
			var tPos = t.position;
			
			// color
			if(this._editorSceneIsDirty) Gizmos.color = Color.yellow;
			else if(this._anyTriggerObjectInside) Gizmos.color = Color.blue;
			else Gizmos.color = Color.green;
			
			Handles.Label(tPos, t.name);
			Gizmos.DrawWireCube(tPos, t.localScale);
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>
		
		
		

		#region <<---------- General ---------->>
		
		void CheckForObject() {
			if (this == null) return;
			var t = this.transform;
				
			#if UNITY_EDITOR
			if (!Application.isPlaying && !PrefabStageUtility.GetCurrentPrefabStage()) {
				var bounds = new Bounds(t.position, t.localScale);
				var editorCameraTransform = SceneView.lastActiveSceneView.camera.transform;
				this.AnyTriggerObjectInside = bounds.Contains(editorCameraTransform.position);
				return;
			}
			#endif
				
			var tRotation = t.rotation;
			
			
			 var colliders = Physics.OverlapBox(
				t.position, 
				t.localScale*0.5f,
				tRotation, 
				this._triggerLayer);

			 this.AnyTriggerObjectInside = colliders.Any(c => c.CompareTag(_tag));
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
				EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(this._scene.sceneAsset), OpenSceneMode.Additive);
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
			if (!scene.IsValid()) {
				Debug.LogWarning($"Will not try to unload scene '{this._scene}' since its is invalid.");
				return;
			}
			this._unloadAsyncOperation = SceneManager.UnloadSceneAsync(scene);
			if (this._unloadAsyncOperation == null) return;
			this._unloadAsyncOperation.completed += operation => {
				this._unloadAsyncOperation = null;
			};
		}
		
		#endregion <<---------- General ---------->>

	}
}

﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

#if UNITY_2020
using UnityEditor.Experimental.SceneManagement;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace CDK {
	[ExecuteInEditMode]
	public class CSceneAreaAdditiveLoader : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

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
		private int _updateFrameSkips = 5;
		
		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>
		
		private void Awake() {
			this._anyTriggerObjectInside = false;
			this._loadAsyncOperation = null;
			this._unloadAsyncOperation = null;
			#if UNITY_EDITOR
			if (!Application.isPlaying) {
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
			
			if (this._anyTriggerObjectInside) {
				Gizmos.DrawWireCube(tPos, t.localScale);
			}
			else {
				Gizmos.DrawCube(tPos, t.localScale);
			}
			
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>
		
		
		

		#region <<---------- General ---------->>
		
		void CheckForObject() {
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
		
		#endregion <<---------- General ---------->>

	}
}

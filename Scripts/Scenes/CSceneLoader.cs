using System;
using CDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace CDK {
	public class CSceneLoader : MonoBehaviour {
		
		public CSceneField sceneToLoad;
		[NonSerialized] private AsyncOperation asyncOp;
		[SerializeField] private bool loadIfAlreadyLoaded = false;
		[SerializeField] private LoadSceneMode loadSceneMode = LoadSceneMode.Additive;
		[FormerlySerializedAs("activeOnLoad"),SerializeField] private bool enableSceneOnLoad = true;
		[SerializeField] private bool setAsActive = true;
		[SerializeField] private bool unloadThisSceneOnComplete;
		
	
		[SerializeField] private CUnityEvent SceneLoadedEvent;
		[SerializeField] private CUnityEvent sceneEnabledEvent;

		
		
		
		public void LoadScene() {

			var a = SceneManager.GetSceneByName(this.sceneToLoad);
			if (a.isLoaded && !this.loadIfAlreadyLoaded) {
				Debug.Log($"Scene {this.sceneToLoad} is already loaded.");
				return;
			}
			
			this.asyncOp = SceneManager.LoadSceneAsync(this.sceneToLoad, this.loadSceneMode);

			this.asyncOp.allowSceneActivation = this.enableSceneOnLoad;
			this.asyncOp.completed += operation => {
				if (this.setAsActive && this.enableSceneOnLoad) {
					SceneManager.SetActiveScene(SceneManager.GetSceneByName(this.sceneToLoad));
				}
				this.SceneLoadedEvent?.Invoke();
				if (this.enableSceneOnLoad) {
					this.sceneEnabledEvent?.Invoke();
				}

				if (this != null && this.unloadThisSceneOnComplete) {
					SceneManager.UnloadSceneAsync(this.gameObject.scene);
				}
			};
		}

		public void ActivateLoadedScene() {
			this.asyncOp.allowSceneActivation = true;
			this.sceneEnabledEvent?.Invoke();
			
			if (this.setAsActive) {
				SceneManager.SetActiveScene(SceneManager.GetSceneByName(this.sceneToLoad));
			}
			
			if (this.unloadThisSceneOnComplete) {
				SceneManager.UnloadSceneAsync(this.gameObject.scene);
			}
			
		}
		
	}
}
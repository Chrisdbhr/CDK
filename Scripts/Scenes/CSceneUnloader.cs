using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CDK {
	public class CSceneUnloader : MonoBehaviour {

		[SerializeField] private bool unloadThisObjectSceneToo;
		
		public CSceneField sceneToUnload;
		[SerializeField] private CUnityEvent SceneUnloadedEvent;
		[NonSerialized] private AsyncOperation asyncOp;
		[NonSerialized] private AsyncOperation thisSceneAsyncOp;


		
		
		public void UnloadScene() {
			if (this.unloadThisObjectSceneToo) {
				this.thisSceneAsyncOp = SceneManager.UnloadSceneAsync(this.gameObject.scene, UnloadSceneOptions.None);
				this.thisSceneAsyncOp.completed += operation => {
					this.SceneUnloadedEvent?.Invoke();
					this.thisSceneAsyncOp = null;
				};
			}

			if (this.sceneToUnload != null) {
				this.asyncOp = SceneManager.UnloadSceneAsync(this.sceneToUnload, UnloadSceneOptions.None);
				this.asyncOp.completed += operation => {
					this.SceneUnloadedEvent?.Invoke();
					this.asyncOp = null;
				};
			}
		}
		
	}
}
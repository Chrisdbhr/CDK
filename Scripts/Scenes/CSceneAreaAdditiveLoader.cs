using CDK;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CDK {
	public class CSceneAreaAdditiveLoader : MonoBehaviour {

		
		[SerializeField][TagSelector] private string _tag = "Player";
		[SerializeField] private bool _asyncLoad = true;
		[SerializeField] private CSceneField _scene;

		private AsyncOperation _loadAsyncOperation;


		#region <<---------- MonoBehaviour ---------->>
		
		private void OnTriggerEnter(Collider other) {
			if (!other.CompareTag(_tag)) return;
			this.LoadScene();
		}

		private void OnTriggerExit(Collider other) {
			if (!other.CompareTag(_tag)) return;
			this.UnloadScene();
		}
		
		#endregion <<---------- MonoBehaviour ---------->>


		

		#region <<---------- General ---------->>
		
		void LoadScene() {
			if (this._asyncLoad) {
				this._loadAsyncOperation = SceneManager.LoadSceneAsync(this._scene, LoadSceneMode.Additive);
			}
			else {
				SceneManager.LoadScene(this._scene, LoadSceneMode.Additive);
			}
		}

		void UnloadScene() {
			if (this._loadAsyncOperation != null && !this._loadAsyncOperation.isDone) {
				this._loadAsyncOperation.completed += _ => {
					SceneManager.UnloadSceneAsync(this._scene);
				};
				return;
			}

			SceneManager.UnloadSceneAsync(this._scene);
		}
		
		#endregion <<---------- General ---------->>

	}
}

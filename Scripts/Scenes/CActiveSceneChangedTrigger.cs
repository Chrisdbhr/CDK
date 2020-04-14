using UnityEngine.SceneManagement;

namespace CDK {
	public class CActiveSceneChangedTrigger : CEventTrigger{

		public void OnEnable() {
			SceneManager.activeSceneChanged += this.ActiveSceneChanged;
		}

		public void OnDisable() {
			SceneManager.activeSceneChanged -= this.ActiveSceneChanged;
		}

		private void ActiveSceneChanged(Scene oldScene, Scene newScene) {
			this.TriggerEvent();
		}

	}
}
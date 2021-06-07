using UnityEngine;

namespace CDK {
	public class CAwakeSetActiveState : MonoBehaviour {

		[SerializeField] private bool _activeStateOnAwake;

		private void Awake() {
			this.gameObject.SetActive(this._activeStateOnAwake);
			this.CDestroy();
		}
		
	}
}
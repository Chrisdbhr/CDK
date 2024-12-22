using UnityEngine;

namespace CDK {
	public class CAwakeSetActiveState : MonoBehaviour {

		[SerializeField] bool _activeStateOnAwake;

		void Awake() {
			gameObject.SetActive(_activeStateOnAwake);
			this.CDestroy();
		}
		
	}
}
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CStartTrigger : MonoBehaviour{

		[SerializeField] private UnityEvent Event;

		private void Start() {
			this.Event?.Invoke();
		}
	}
}
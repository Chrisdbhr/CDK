using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CEnableTrigger : MonoBehaviour {
        [SerializeField, Min(0f)] private float _delayInSeconds;
		[SerializeField] private UnityEvent TriggerEvent;
		
		private void OnEnable() {
            if (this._delayInSeconds > 0f) {
                this.CStartCoroutine(this.EnableRoutine());
                return;
            }
            this.TriggerEvent?.Invoke();
		}

        IEnumerator EnableRoutine() {
            yield return new WaitForSecondsRealtime(this._delayInSeconds);
            this.TriggerEvent?.Invoke();
        }
		
	}
}
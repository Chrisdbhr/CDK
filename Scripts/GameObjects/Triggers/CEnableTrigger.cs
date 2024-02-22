using System.Collections;
using UnityEngine;
using UnityEngine.Events;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace CDK {
	public class CEnableTrigger : MonoBehaviour {
        [SerializeField, Min(0f)] private float _delayInSeconds;
        #if ODIN_INSPECTOR
        [ShowIf("@_delayInSeconds > 0f")]
        #endif
        [SerializeField] private bool _ignoreTimescale = true;
		[SerializeField] private UnityEvent TriggerEvent;
		
		private void OnEnable() {
            if (this._delayInSeconds > 0f) {
                this.CStartCoroutine(this.EnableRoutine());
                return;
            }
            this.TriggerEvent?.Invoke();
		}

        IEnumerator EnableRoutine() {
            yield return (_ignoreTimescale ? new WaitForSecondsRealtime(_delayInSeconds) : new WaitForSeconds(_delayInSeconds));
            this.TriggerEvent?.Invoke();
        }
		
	}
}
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace CDK {
	public class CEnableTrigger : MonoBehaviour {
        [SerializeField, Min(0f)] float _delayInSeconds;
        #if ODIN_INSPECTOR
        [ShowIf("@_delayInSeconds > 0f")]
        #endif
        [SerializeField]
        bool _ignoreTimescale = true;
		[SerializeField] UnityEvent TriggerEvent;

		void OnEnable() {
            if (_delayInSeconds > 0f) {
                this.CStartCoroutine(EnableRoutine());
                return;
            }
            TriggerEvent?.Invoke();
		}

        IEnumerator EnableRoutine() {
            yield return (_ignoreTimescale ? new WaitForSecondsRealtime(_delayInSeconds) : new WaitForSeconds(_delayInSeconds));
            TriggerEvent?.Invoke();
        }
		
	}
}
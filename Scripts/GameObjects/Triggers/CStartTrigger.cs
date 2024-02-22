using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace CDK {
	public class CStartTrigger : MonoBehaviour{
        
        [SerializeField, Min(0f)] private float _delay;
        #if ODIN_INSPECTOR
        [ShowIf("@_delay > 0f")]
        #endif
        [SerializeField] private bool _ignoreTimescale = true;
		[SerializeField] private UnityEvent Event;
        private bool alreadyCalled;

        IEnumerator Start() {
            if(_delay > 0f) yield return (_ignoreTimescale ? new WaitForSecondsRealtime(_delay) : new WaitForSeconds(_delay));
            Event?.Invoke();
            yield break;
        }

        private void OnEnable() {
            if (this.alreadyCalled && _delay > 0f) {
                Debug.LogWarning($"This {nameof(CStartTrigger)} has already been called its Start() and has a delay. Its possible that the {this.Event.GetPersistentEventCount()} events have been not triggered.");
            }
        }

        private void OnDisable() {
            alreadyCalled = true;
        }
    }
}
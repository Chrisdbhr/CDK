using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace CDK {
	public class CEventTrigger : MonoBehaviour {

		[SerializeField] float delayToTriggerEvent;
		[SerializeField] bool _triggerOnlyOneTime;
		[SerializeField] UnityEvent eventToTrigger;
		[NonSerialized] bool _triggered;

		void OnEnable() { } // exposing to allow enable/disable component.

		#if ODIN_INSPECTOR
		[Button]
		#endif
		public virtual void TriggerEvent() {
			if (!enabled || _triggered) return;

			if(_triggerOnlyOneTime) _triggered = true;
			
			if (delayToTriggerEvent > 0f) {
				this.CStartCoroutine(DelayedTrigger(delayToTriggerEvent));
				return;
			}
			// trigger now
			eventToTrigger?.Invoke();
		}

		IEnumerator DelayedTrigger(float delayToTriggerEvent)
		{
			yield return new WaitForSeconds(delayToTriggerEvent);
			eventToTrigger?.Invoke();
		}

	}
}
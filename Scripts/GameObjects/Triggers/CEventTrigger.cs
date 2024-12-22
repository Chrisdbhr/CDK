using System;
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
				Observable.Timer(TimeSpan.FromSeconds(delayToTriggerEvent)).Subscribe(_ => {
					eventToTrigger?.Invoke();
				});
				
				return;
			}
			// trigger now
			eventToTrigger?.Invoke();
		}

		System.Collections.IEnumerator WaitTime(float timeInSeconds, Action onFinish) {
			yield return new WaitForSeconds(timeInSeconds);
			onFinish?.Invoke();
		}
		
	}
}
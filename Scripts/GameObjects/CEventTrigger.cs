using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CEventTrigger : MonoBehaviour {
		[SerializeField] private float delayToTriggerEvent;
		[SerializeField] private bool _triggerOnlyOneTime;

		[SerializeField] private UnityEvent eventToTrigger;
		
		[NonSerialized] private bool _triggered;
		
		
		

		private void OnEnable() { } // exposing to allow enable/disable component.


		public virtual void TriggerEvent() {
			if (!this.enabled || this._triggered) return;

			if(this._triggerOnlyOneTime) this._triggered = true;
			
			if (this.delayToTriggerEvent > 0f) {
				Observable.Timer(TimeSpan.FromSeconds(this.delayToTriggerEvent)).Subscribe(_ => {
					this.eventToTrigger?.Invoke();
				});
				
				return;
			}
			// trigger now
			this.eventToTrigger?.Invoke();
		}

		private System.Collections.IEnumerator WaitTime(float timeInSeconds, Action onFinish) {
			yield return new WaitForSeconds(timeInSeconds);
			onFinish?.Invoke();
		}
		
	}
}
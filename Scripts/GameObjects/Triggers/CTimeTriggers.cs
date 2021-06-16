using UnityEngine;

namespace CDK {
	public class CTimeTriggers : MonoBehaviour {

		[Header("Booleans")]
		[SerializeField] private CUnityEventBool _timeStoppedEvent;
		[SerializeField] private CUnityEventBool _timeResumedEvent;
		
		[Header("Floats")]
		[SerializeField] private CUnityEventFloat _timeScaleValueEvent;
		

		private void OnEnable() {
			CTime.OnTimeScaleChanged += this.TimeScaleChanged;
		}
		private void OnDisable() {
			CTime.OnTimeScaleChanged -= this.TimeScaleChanged;
		}


		private void TimeScaleChanged(float oldTimeScale, float newTimeScale) {
			bool timeStopped = newTimeScale <= 0f;

			this._timeResumedEvent?.Invoke(!timeStopped);
			this._timeStoppedEvent?.Invoke(timeStopped);
			
			this._timeScaleValueEvent?.Invoke(newTimeScale);
		}
	}
}

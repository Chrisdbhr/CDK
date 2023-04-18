using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDK {
	
	/// <summary>
	/// All values are normalized.
	/// </summary>
	public class CFillBarManager : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private Image _imageCurrentHealth;
		[SerializeField] private Image _imageDelayedHealth;
		[SerializeField] private float _decayDelay = 4f;
		[SerializeField] private float _decayTime = 4f;

		private float CurrentValue {
			get {
				return this._currentValue;
			}
			set {
				if (value == this._currentValue) return;

				if (value < this._currentValue) {
					this.CStopCoroutine(this._routineDelayedBar);
					this._routineDelayedBar = this.CStartCoroutine(this.UpdateDelayedBar(this._currentValue, value));
				}
				if (this._imageCurrentHealth != null) this._imageCurrentHealth.fillAmount = value;
				
				this._currentValue = value;
			}
		}
		private float _currentValue = 1f;
		
		private Coroutine _routineDelayedBar;

		#endregion <<---------- Properties and Fields ---------->>




		public void SetValueNormalized(float newValue) {
			this.CurrentValue = newValue.CClamp01();
		}

		private IEnumerator UpdateDelayedBar(float initialValue, float targetValue) {
			this.SetDelayedBarValue(initialValue, 0f);
			yield return new WaitForSeconds(this._decayDelay);
			while (!this.SetDelayedBarValue(targetValue, this._decayTime)) {
				yield return null;
			}
		}

		/// <summary>
		/// Returns TRUE if finished setting the value.
		/// </summary>
		private bool SetDelayedBarValue(float targetValue, float time) {
			if (this._imageDelayedHealth == null) return true;
			if (time <= 0f) {
				this._imageDelayedHealth.fillAmount = targetValue;
				return true;
			}
			float currentFill = this._imageDelayedHealth.fillAmount;
			float step = CTime.DeltaTimeScaled / time;
			float nextValue = currentFill - step;
			if (nextValue <= targetValue) {
				this._imageDelayedHealth.fillAmount = targetValue;
				return true;
			}
			this._imageDelayedHealth.fillAmount = nextValue;
			return false;
		}
		
	}
}

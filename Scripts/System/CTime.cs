using System;
using UnityEngine;

namespace CDK {
	public static class CTime {
		
		/// <summary>
		/// Returns Time.deltaTime scaled with Time.timeScale.
		/// </summary>
		public static float DeltaTimeScaled {
			get {
				return Time.deltaTime * Time.timeScale;
			}
		}

		/// <summary>
		/// Returns Time.fixedDeltaTime scaled with Time.timeScale.
		/// </summary>
		public static float FixedDeltaTimeScaled {
			get {
				return Time.fixedDeltaTime * Time.timeScale;
			}
		}

		/// <summary>
		/// Set time scale optionally logging on console.
		/// </summary>
		public static void SetTimeScale(float targetTimeScale, bool supressLog = false) {
			if(!supressLog) Debug.Log($"Setting time scale to {targetTimeScale}");

			_onTimeScaleChanged?.Invoke(targetTimeScale);
			
			Time.timeScale = targetTimeScale;
		}

		/// <summary>
		/// Notify time scaled changed (oldTimeScale, newTimeScale)
		/// </summary>
		public static event Action<float> OnTimeScaleChanged {
			add {
				_onTimeScaleChanged -= value;
				_onTimeScaleChanged += value;
			}
			remove {
				_onTimeScaleChanged -= value;
			}
		}
		private static Action<float> _onTimeScaleChanged;
	}
}

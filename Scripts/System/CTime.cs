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
		/// Set time scale and invoke a event if changed.
		/// </summary>
		public static float TimeScale {
			get { return Time.timeScale; }
			set {
				var oldTimeScale = Time.timeScale;
				if (value == oldTimeScale) return;

				Time.timeScale = value;
				_onTimeScaleChanged?.Invoke(oldTimeScale, value);
			}
		}

		/// <summary>
		/// Notify time scaled changed (oldTimeScale, newTimeScale)
		/// </summary>
		public static event Action<float, float> OnTimeScaleChanged {
			add {
				_onTimeScaleChanged -= value;
				_onTimeScaleChanged += value;
			}
			remove {
				_onTimeScaleChanged -= value;
			}
		}
		private static Action<float, float> _onTimeScaleChanged;
	}
}

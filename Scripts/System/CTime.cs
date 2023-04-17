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
		/// Set time scale and invoke a event if changed.
		/// </summary>
		public static float TimeScale {
			get { return Time.timeScale; }
			set {
				var oldTimeScale = Time.timeScale;
				if (value == oldTimeScale) return;
                _onTimePaused?.Invoke(value == 0f);
                Time.timeScale = value;
				_onTimeScaleChanged?.Invoke(oldTimeScale, value);
			}
		}

        public static bool IsPaused => Time.timeScale == 0f;

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
        
        public static event Action<bool> OnTimePaused {
            add {
                _onTimePaused -= value;
                _onTimePaused += value;
            }
            remove {
                _onTimePaused -= value;
            }
        }
        private static Action<bool> _onTimePaused;
	}
}

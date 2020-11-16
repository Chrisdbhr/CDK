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
		/// Set time scale logging this action on console.
		/// </summary>
		public static void SetTimeScale(float targetTimeScale) {
			Debug.Log($"Settings time scale to {targetTimeScale}");
			Time.timeScale = targetTimeScale;
		}
	}
}

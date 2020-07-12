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
	}
}

using UnityEngine;

namespace CDK {
	public static class CApplication {
			
		/// <summary>
		/// ANTES da scene load.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeBeforeSceneLoad() {
			Application.backgroundLoadingPriority = ThreadPriority.Low;
		}
	}
}

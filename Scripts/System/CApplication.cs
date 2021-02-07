using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CDK {
	public static class CApplication {
			
		/// <summary>
		/// ANTES da scene load.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeBeforeSceneLoad() {
			Application.backgroundLoadingPriority = ThreadPriority.Low;
			Debug.Log("CApplication calling Addressables.InitializeAsync()");
			Addressables.InitializeAsync();

			Application.quitting += IsQuitting;
		}

		public static event Action IsQuitting;
	}
}

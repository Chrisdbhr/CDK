using IngameDebugConsole;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CDK {
	public static class CLevelManager {
		
		/// <summary>
		/// ANTES da scene load.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeBeforeSceneLoad() {
			DebugLogConsole.AddCommandStatic( "load", "Load a level.", nameof(LoadLevel), typeof(CLevelManager));
		}

		public static void LoadLevel(string name) {
			SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
		}
		
	}
}

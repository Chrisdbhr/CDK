using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	[DefaultExecutionOrder(-50000)]
	public static class CApplication {
			
		/// <summary>
		/// ANTES da scene load.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeBeforeSceneLoad() {
			InitializeApplication().CAwait();
		}

		private static async Task InitializeApplication() {
			Application.backgroundLoadingPriority = ThreadPriority.High;

			Application.targetFrameRate = 60;

			await AddressablesInitialize();
			await DetectLanguage();

			Quitting = false;
			Application.quitting -= IsQuitting;
			Application.quitting += IsQuitting;
			Application.quitting += () => {
				Debug.Log("CApplication is quitting...");
				Quitting = true;
			};
			
			ApplicationInitialized?.Invoke();
			
			Application.backgroundLoadingPriority = ThreadPriority.Low;
		}

		private static async Task AddressablesInitialize() {
			Debug.Log("CApplication initializing Addressables");
			await Addressables.InitializeAsync().Task;
		}
		
		private static async Task DetectLanguage() {
			await LocalizationSettings.InitializationOperation.Task;
			var systemCulture = System.Globalization.CultureInfo.CurrentCulture;
			foreach (var locale in LocalizationSettings.AvailableLocales.Locales) {
				var currentCulture = locale.Identifier.CultureInfo;
				if (Equals(currentCulture, systemCulture) ||
					Equals(currentCulture, systemCulture.Parent)) {
					Debug.Log($"Detected {systemCulture} and auto selected language {currentCulture}");
					LocalizationSettings.SelectedLocale = locale;
					return;
				}
			}
			Debug.Log($"Could not auto select language for {systemCulture}");
		}

		
		public static event Action IsQuitting;
		public static event Action ApplicationInitialized;
		
		public static bool Quitting { get; private set; }


		public static void Quit() {
			Debug.Log("Requesting Application.Quit()");
			
			#if UNITY_EDITOR
			Time.timeScale = 1f;
			EditorApplication.isPlaying = false;
			#else
			Application.Quit();			
			#endif
			
		}
		
		
	}
}

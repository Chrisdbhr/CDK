using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	[DefaultExecutionOrder(-100)]
	public abstract class CApplication {

		#region <<---------- Initialization ---------->>
		
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

			InitializeDependecyContainerAndBinds();
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

		#endregion <<---------- Initialization ---------->>




		#region <<---------- Dependencies ---------->>

		private static void InitializeDependecyContainerAndBinds() {
			
			CDependencyResolver.Initialize();
			
			CDependencyResolver.Bind<CGameSettings>(() => Resources.Load<CGameSettings>("GameSettings"));
			
			CDependencyResolver.Bind<CFootstepDatabase>(() => Resources.Load<CFootstepDatabase>("FootstepsDatabase"));
			
			CDependencyResolver.Bind<CCursorManager>(() => new CCursorManager());
			CDependencyResolver.Get<CCursorManager>(); // force create instance
			
			CDependencyResolver.Bind<CFader>(()=> new CFader());
			
			CDependencyResolver.Bind<CSceneManager>(()=> new CSceneManager());
		}

		#endregion <<---------- Dependencies ---------->>
		
		
		

		#region <<---------- Addressables ---------->>
		
		private static async Task AddressablesInitialize() {
			Debug.Log("CApplication initializing Addressables");
			await Addressables.InitializeAsync().Task;
		}
		
		#endregion <<---------- Addressables ---------->>


		
		
		#region <<---------- Language ---------->>
		
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

		#endregion <<---------- Language ---------->>


		

		#region <<---------- Application Quit ---------->>
		
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
		
		#endregion <<---------- Application Quit ---------->>

	}
}

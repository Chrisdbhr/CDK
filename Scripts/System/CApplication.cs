using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ThreadPriority = UnityEngine.ThreadPriority;

#if UnityAddressables
using UnityEngine.AddressableAssets;
#endif

#if UnityLocalization
using UnityEngine.Localization.Settings;
#endif

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
			Application.backgroundLoadingPriority = ThreadPriority.High; // high to load fast first assets.

			Application.targetFrameRate = 60;

			InitializeDependecyContainerAndBinds();
			
			#if UnityAddressables
			await AddressablesInitialize();
			#endif
			
			#if UnityLocalization
			await LocalizationInitialize();
			#endif

			// app quit
			IsQuitting = false;
			QuittingCancellationTokenSource = new CancellationTokenSource();
			Application.quitting -= QuittingEvent;
			Application.quitting += QuittingEvent;
			Application.quitting += () => {
				Debug.Log("<color=red>CApplication is quitting...</color>");
				IsQuitting = true;
				QuittingCancellationTokenSource.Cancel();
			};
			
			ApplicationInitialized?.Invoke();
			
			Application.backgroundLoadingPriority = ThreadPriority.Low;
		}

		#endregion <<---------- Initialization ---------->>



		#region <<---------- Properties and Fields ---------->>
		
		public static event Action QuittingEvent;
		public static event Action ApplicationInitialized;
		public static bool IsQuitting { get; private set; }
		public static CancellationTokenSource QuittingCancellationTokenSource;
		
		#endregion <<---------- Properties and Fields ---------->>
		
		

		#region <<---------- Platform Features ---------->>

		void PlatformSpecificFeatures() {
			var supportsComputeShaders = SystemInfo.supportsComputeShaders;
			
		}
		
		#endregion <<---------- Platform Features ---------->>


		

		#region <<---------- Dependencies ---------->>

		private static void InitializeDependecyContainerAndBinds() {
			
			CDependencyResolver.Bind<CGameSettings>(() => Resources.Load<CGameSettings>("GameSettings"));
			CDependencyResolver.Bind<CLoading>(() => new CLoading());
			CDependencyResolver.Bind<CBlockingEventsManager>(() => new CBlockingEventsManager());
			CDependencyResolver.Bind<CGamePlayerManager>(() => new CGamePlayerManager());

			CDependencyResolver.Bind<CFootstepDatabase>(() => Resources.Load<CFootstepDatabase>("FootstepsDatabase"));
			
			CDependencyResolver.Bind<CCursorManager>(() => new CCursorManager());
			CDependencyResolver.Get<CCursorManager>(); // force create instance
			
			CDependencyResolver.Bind<CFader>(()=> new CFader());
			
			CDependencyResolver.Bind<CSceneManager>(()=> new CSceneManager());
		}

		#endregion <<---------- Dependencies ---------->>
		
		
		

		#region <<---------- Addressables ---------->>

		#if UnityAddressables
		
		private static async Task AddressablesInitialize() {
			Debug.Log("CApplication initializing Addressables");
			await Addressables.InitializeAsync().Task;
		}
		
		#endif

		#endregion <<---------- Addressables ---------->>


		
		
		#region <<---------- Language ---------->>

		#if UnityLocalization

		private static async Task LocalizationInitialize() {
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

		#endif

		#endregion <<---------- Language ---------->>


		

		#region <<---------- Application Quit ---------->>
		
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

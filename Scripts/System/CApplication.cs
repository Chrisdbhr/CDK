using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CDK.UI;
using UnityEngine;
using ThreadPriority = UnityEngine.ThreadPriority;

#if UnityAddressables
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
#endif

#if UnityLocalization
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
#endif

#if Rewired
using Rewired;
#endif

namespace CDK {
    [DefaultExecutionOrder(-100)]
    public abstract class CApplication {

        #region <<---------- Initialization ---------->>

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void InitializeBeforeSceneLoad() {
            Debug.Log($"[{nameof(CApplication)}] Initializing Application.");
            
            CreatePersistentDataPath();

            // app quit
            IsQuitting = false;
            Application.quitting -= QuittingEvent;
            Application.quitting += QuittingEvent;
            Application.quitting += () => {
                Debug.Log("<color=red>CApplication is quitting...</color>");
                IsQuitting = true;
                QuittingCancellationTokenSource?.Cancel();
            };

            InitializeInputManager();

            InitializeDependencyContainerAndBinds();

            InitializeApplicationAsync().CAwait();

            COutOfBoundsDestroyer.CreateMonitor();
        }

        private static async Task InitializeApplicationAsync() {
            Application.backgroundLoadingPriority = ThreadPriority.High; // high to load fast first assets.
            Application.targetFrameRate = 15;

            #if UnityAddressables
            var resourceLocator = await AddressablesInitializeAsync();
            Debug.Log($"Resource Locator Id: '{(resourceLocator != null ? resourceLocator.LocatorId : "null")}'");
			#endif

			#if UnityLocalization
            await LocalizationInitializeAsync();
			#endif

            ApplicationInitialized?.Invoke();

            Application.targetFrameRate = 60;
            Application.backgroundLoadingPriority = ThreadPriority.Low;
        }

        #endregion <<---------- Initialization ---------->>




        #region <<---------- Properties and Fields ---------->>

        public static event Action QuittingEvent;
        public static event Action ApplicationInitialized;
        public static bool IsQuitting { get; private set; }
        public static CancellationTokenSource QuittingCancellationTokenSource = new CancellationTokenSource();

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Dependencies ---------->>

        private static void InitializeDependencyContainerAndBinds() {
            CDependencyResolver.Bind<CGameSettings>(() => Resources.Load<CGameSettings>("GameSettings"));
            CDependencyResolver.Bind<CBlockingEventsManager>(() => new CBlockingEventsManager());
            CDependencyResolver.Bind<CUINavigationManager>(() => new CUINavigationManager());
            CDependencyResolver.Bind<CGamePlayerManager>(() => new CGamePlayerManager());

            CDependencyResolver.Bind<CCursorManager>(() => new CCursorManager());
            CDependencyResolver.Get<CCursorManager>(); // force create instance

            CDependencyResolver.Bind<CFader>(() => new CFader());

            CDependencyResolver.Bind<CSceneManager>(() => new CSceneManager());
        }

        #endregion <<---------- Dependencies ---------->>




        #region <<---------- Input ---------->>

        private static void InitializeInputManager() {
            #if Rewired
            var rInputManager = GameObject.FindObjectOfType<InputManager_Base>();
            if (rInputManager) {
                Debug.Log("Will not Instantiate a new <b>Rewired Input Manager</b> because one is already in the scene.");
                return;
            }

            var rw = CAssets.LoadResourceAndInstantiate<InputManager_Base>("Rewired Input Manager");
            if (!rw) {
                Debug.LogError("<b>Rewired Input Manager</b> could not be Instantiated.");
                return;
            }

            Debug.Log("<b>Rewired Input Manager</b> instantiated.");
            rw.name = rw.name.Replace("(Clone)", string.Empty) + " (Instantiated BeforeSplashScreen)";
            #else
            Debug.LogError("No input manager setup on initialization.");
			#endif
        }

        #endregion <<---------- Input ---------->>




        #region <<---------- Addressables ---------->>

		#if UnityAddressables

        private static async Task<IResourceLocator> AddressablesInitializeAsync() {
            Debug.Log("CApplication initializing Addressables");
            var op = Addressables.InitializeAsync();
            var resourceLocator = await op.Task;
            return resourceLocator;
        }

		#endif

        #endregion <<---------- Addressables ---------->>




        #region <<---------- Language ---------->>

		#if UnityLocalization

        private static async Task LocalizationInitializeAsync() {
            Debug.Log("Initializing Localization System.");
            await LocalizationSettings.InitializationOperation.Task;
            var prefs = CPlayerPrefs.Current;
            if (prefs.Language.CIsNullOrEmpty()) AutoSetLanguage();
            else if (!SetLocaleByString(prefs.Language)) {
                prefs.Language = null;
                CPlayerPrefs.SaveCurrent();
            }
        }


        private static void AutoSetLanguage() {
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
        

        private static bool SetLocaleByString(string localeStr) {
            var locale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(localeStr));
            if (locale == null) {
                Debug.LogError($"Could not set locale from string '{localeStr}'");
                return false;
            }
            Debug.Log($"Setting Locale to '{localeStr}'.");
            LocalizationSettings.SelectedLocale = locale;
            return true;
        }
        
		#endif

        #endregion <<---------- Language ---------->>
        
        
        
        
        #region <<---------- Paths ---------->>
        
        private static void CreatePersistentDataPath() {
            try {
                if (Directory.Exists(Application.persistentDataPath)) return;
                Directory.CreateDirectory(Application.persistentDataPath);
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }
        
        #endregion <<---------- Paths ---------->>




		#region <<---------- Application Quit ---------->>
		
		public static void Quit() {
			Debug.Log("Requesting Application.Quit()");
			
			#if UNITY_EDITOR
			Time.timeScale = 1f;
            UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();			
			#endif
			
		}
		
		#endregion <<---------- Application Quit ---------->>

	}
}

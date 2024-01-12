using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ThreadPriority = UnityEngine.ThreadPriority;

#if UNITY_ADDRESSABLES_EXIST
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
#endif

#if REWIRED
using Rewired;
#endif

#if FMOD
using FMODUnity;
#endif

namespace CDK {
    [DefaultExecutionOrder(-100)]
    public static class CApplication {

        #region <<---------- Initialization ---------->>

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void InitializeBeforeSceneLoad() {
            Debug.Log($"CDK v{CDK.VERSION} -> Initializing Application");

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

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

            InitializeApplicationAsync().CAwait();
        }

        private static async Task InitializeApplicationAsync() {
            Application.backgroundLoadingPriority = ThreadPriority.High; // high to load fast first assets.
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 18;
            
            #if UNITY_ADDRESSABLES_EXIST
            ResourceLocator = await AddressablesInitializeAsync();
            Debug.Log($"Resource Locator Id: '{(ResourceLocator != null ? ResourceLocator.LocatorId : "null")}'");
			#endif
            
            #if FMOD
            try {
                RuntimeManager.LoadBank("Master");
                RuntimeManager.LoadBank("Master.strings");
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
            #endif

            await InitializeInputManagerAsync();

            ApplicationInitialized?.Invoke();

            var isMobile = CPlayerPlatformTrigger.IsMobilePlatform();
            if (isMobile) {
                ScalableBufferManager.ResizeBuffers(0.7f, 0.7f);
            }
            
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = isMobile ? 30 : 60;
            Application.backgroundLoadingPriority = ThreadPriority.Low;
        }

        #endregion <<---------- Initialization ---------->>




        #region <<---------- Properties and Fields ---------->>

        public static event Action QuittingEvent;
        public static event Action ApplicationInitialized;
        public static bool IsQuitting { get; private set; }
        public static CancellationTokenSource QuittingCancellationTokenSource = new CancellationTokenSource();

        public static IResourceLocator ResourceLocator;

        public static Version Version {
            get {
                if (_version != null) return _version;
                if (Version.TryParse(Application.version, out _version)) {
                    return _version;
                }
                return _version = new Version(0, 0, 0, 0);
            }
        }
        private static Version _version;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Input ---------->>

        private static async Task InitializeInputManagerAsync() {
            #if REWIRED
            var rInputManager = GameObject.FindObjectOfType<InputManager_Base>();
            if (rInputManager) {
                Debug.Log("Will not Instantiate a new <b>Rewired Input Manager</b> because one is already in the scene.");
                return;
            }

            var rw = CAssets.LoadResourceAndInstantiate<InputManager_Base>("System/Input Manager");
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

		#if UNITY_ADDRESSABLES_EXIST

        private static async Task<IResourceLocator> AddressablesInitializeAsync() {
            var op = Addressables.InitializeAsync();
            var resourceLocator = await op.Task;
            return resourceLocator;
        }

		#endif

        #endregion <<---------- Addressables ---------->>

        
        
        
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




		#region <<---------- Application ---------->>

        public static bool IsEditorOrDevelopment() {
            return Application.isEditor || Debug.isDebugBuild;
        }
        
		public static void Quit() {
			Debug.Log("Requesting Application.Quit()");
			
			#if UNITY_EDITOR
			Time.timeScale = 1f;
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_WEBGL
			Application.OpenURL("https://chrisjogos.com");
			#else
			Application.Quit();			
			#endif
			
		}

		#endregion <<---------- Application ---------->>

        /// <summary>
        /// Log and try Open URL.
        /// </summary>
        public static void OpenURL(string urlToOpen) {
            Debug.Log($"Requested to open url {urlToOpen}");
            Application.OpenURL(urlToOpen);
        }
    }
}
using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

#if FMOD
using FMODUnity;
#endif

#if Rewired
using Rewired;
#endif

#if UnityLocalization
using UnityEngine.Localization.Settings;
#endif

namespace CDK {
    public class CAppInitializeBeforeTheSplashTrigger : MonoBehaviour {

        [SerializeField] private CanvasGroup _loadingCanvas;
        [SerializeField] private CSceneField _nextScene;
        private Coroutine _initializationRoutine;
        private IDisposable _disposeOnDisable;



        #region <<---------- MonoBehaviour ---------->>
        
        private void Awake() {
            if(this._loadingCanvas) this._loadingCanvas.alpha = 0f;
            this._initializationRoutine = this.CStartCoroutine(Initialize());
        }

        private void OnEnable() {
            this._disposeOnDisable = Observable.Timer(TimeSpan.FromSeconds(1f))
            .Subscribe(_ => {
                if (this._loadingCanvas) {
                    this._loadingCanvas.alpha = 1f;
                }
            });
        }

        private void OnDisable() {
            this._disposeOnDisable?.Dispose();
            this.CStopCoroutine(this._initializationRoutine);
        }
        
        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- General ---------->>
        
        private IEnumerator Initialize() {
            Debug.Log("Initializing scene that preloads initial assets.");
            
            #if Rewired
            Debug.Log("Waiting for Rewired to get ready.");
            while (!ReInput.isReady) yield return null;
            Debug.Log("Rewired is ready.");
            #endif
            #if FMOD
            Debug.Log("Waiting for FMOD to Load Master Banks.");
            while (!RuntimeManager.HaveMasterBanksLoaded) yield return null;
            Debug.Log("FMOD Master Banks are loaded.");
            #endif

            #if UnityLocalization
            yield return LocalizationSettings.InitializationOperation;
            #endif

            yield return null;
            
            Debug.Log($"Starting to load next scene: '{this._nextScene}'.");
            var asyncOp = SceneManager.LoadSceneAsync(this._nextScene, LoadSceneMode.Single);
            asyncOp.allowSceneActivation = true;
        }

        #endregion <<---------- General ---------->>

    }
}
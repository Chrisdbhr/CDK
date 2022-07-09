﻿using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

#if FMOD
using FMODUnity;
#endif

#if Rewired
using Rewired;
#endif

namespace CDK {
    public class CAppInitializeBeforeTheSplashTrigger : MonoBehaviour {

        [SerializeField] private CanvasGroup _loadingCanvas;
        [SerializeField] private CSceneField _nextScene;
        private Coroutine _initializationRoutine;




        #region <<---------- MonoBehaviour ---------->>
        
        private void Awake() {
            if(this._loadingCanvas) this._loadingCanvas.alpha = 0f;
            this._initializationRoutine = this.CStartCoroutine(Initialize());
        }

        private void OnEnable() {
            Observable.Timer(TimeSpan.FromSeconds(2f)).TakeUntilDisable(this).Subscribe(_ => {
                if(this._loadingCanvas) this._loadingCanvas.alpha = 1f;
            });
        }

        private void OnDisable() {
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
            yield return null;
            
            Debug.Log($"Starting to load next scene: '{this._nextScene}'.");
            var asyncOp = SceneManager.LoadSceneAsync(this._nextScene, LoadSceneMode.Single);
            asyncOp.allowSceneActivation = true;
        }

        #endregion <<---------- General ---------->>

    }
}
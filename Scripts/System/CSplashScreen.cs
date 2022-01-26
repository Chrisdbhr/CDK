﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace CDK {
    public class CSplashScreen : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>
        
        [SerializeField] private CSceneField _sceneToLoad;
        [SerializeField] private PlayableDirector _playableDirector;
        
        [NonSerialized] private bool _splashEnded;

        #endregion <<---------- Properties and Fields ---------->>

        
        
        
        #region <<---------- MonoBehaviour ---------->>
        
        private IEnumerator Start() {
            this._playableDirector.Play();
            this._playableDirector.stopped += OnPlayableDirectorStopped;
            
            var asyncOp = SceneManager.LoadSceneAsync(this._sceneToLoad, LoadSceneMode.Single);
            asyncOp.allowSceneActivation = false;

            while (!FMODUnity.RuntimeManager.HasBanksLoaded) yield return null;
            while (!this._splashEnded) yield return null;

            asyncOp.allowSceneActivation = true;
        }

        private void OnPlayableDirectorStopped(PlayableDirector pd) {
            Debug.Log("OnPlayableDirectorStopped.");
            this._splashEnded = true;
        }

        private void Update() {
            if (Input.anyKeyDown) {
                this._playableDirector.Stop();
            }
        }

        #endregion <<---------- MonoBehaviour ---------->>

    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

#if Rewired
using Rewired;
#endif

#if FMOD
using FMODUnity;
#endif

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
            this._playableDirector.Pause();
            #if Rewired
            while (!ReInput.isReady) yield return null;
            #endif
            #if FMOD
            while (!RuntimeManager.HaveMasterBanksLoaded) yield return null;
            #endif
            yield return null;
            this._playableDirector.Play();
            this._playableDirector.stopped += OnPlayableDirectorStopped;
            
            var asyncOp = SceneManager.LoadSceneAsync(this._sceneToLoad, LoadSceneMode.Single);
            asyncOp.allowSceneActivation = false;

            #if FMOD
            while (!FMODUnity.RuntimeManager.HaveMasterBanksLoaded) yield return null;
            #endif
            while (!this._splashEnded) yield return null;

            asyncOp.allowSceneActivation = true;
        }

        private void OnPlayableDirectorStopped(PlayableDirector pd) {
            Debug.Log("OnPlayableDirectorStopped.");
            this._splashEnded = true;
        }

        private void Reset() {
            if (this._playableDirector == null) this._playableDirector = this.GetComponent<PlayableDirector>();
        }

        #endregion <<---------- MonoBehaviour ---------->>

    }
}

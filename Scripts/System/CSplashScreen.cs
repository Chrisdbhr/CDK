using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

#if REWIRED
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
        [SerializeField] private GameObject _noHardwareAccelerationWarning;
        [NonSerialized] private bool _splashEnded;

        #endregion <<---------- Properties and Fields ---------->>

        
        
        
        #region <<---------- MonoBehaviour ---------->>
        
        private IEnumerator Start() {
            yield return null;

            #if UNITY_WEBGL
            if (SystemInfo.graphicsDeviceName == "GDI Generic") {
                this._noHardwareAccelerationWarning.SetActive(true);
                yield break;
            }
            #endif
            
            CCursorManager.get.ShowMouseIfNeeded();
            
            this._playableDirector.Play();
            this._playableDirector.stopped += OnPlayableDirectorStopped;
            
            var asyncOp = SceneManager.LoadSceneAsync(this._sceneToLoad, LoadSceneMode.Single);
            asyncOp.allowSceneActivation = false;

            while (!this._splashEnded) yield return null;

            asyncOp.allowSceneActivation = true;
        }

        private void Update() {
            if (Input.anyKeyDown) {
                this._playableDirector.Stop();
            }
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
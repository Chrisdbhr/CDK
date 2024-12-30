using System;
using System.Collections;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

#if FMOD
using FMODUnity;
#endif

namespace CDK {
    [RequireComponent(typeof(PlayableDirector))]
    public class CSplashScreen : CMonoBehaviour {

        #region <<---------- Properties and Fields ---------->>
        
        [SerializeField] CSceneField _sceneToLoad;
        [SerializeField] GameObject _noHardwareAccelerationWarning;
        [NonSerialized] [GetComponent] PlayableDirector _playableDirector;
        [NonSerialized] [Inject] readonly CCursorManager cursorManager;
        [NonSerialized] bool _splashEnded;

        #endregion <<---------- Properties and Fields ---------->>

        
        
        
        #region <<---------- MonoBehaviour ---------->>
        
        IEnumerator Start() {
            yield return null;

            #if UNITY_WEBGL
            if (SystemInfo.graphicsDeviceName == "GDI Generic") {
                this._noHardwareAccelerationWarning.SetActive(true);
                yield break;
            }
            #endif
            
            cursorManager.ShowMouseIfNeeded();
            
            _playableDirector.Play();
            _playableDirector.stopped += OnPlayableDirectorStopped;
            
            var asyncOp = SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Single);
            asyncOp.allowSceneActivation = false;

            while (!_splashEnded) yield return null;

            asyncOp.allowSceneActivation = true;
        }

        void Update() {
            if (Input.anyKeyDown) {
                _playableDirector.Stop();
            }
        }

        private void OnPlayableDirectorStopped(PlayableDirector pd) {
            Debug.Log("OnPlayableDirectorStopped.");
            _splashEnded = true;
        }

        private void Reset() {
            if (_playableDirector == null) TryGetComponent(out _playableDirector);
        }

        #endregion <<---------- MonoBehaviour ---------->>

    }
}
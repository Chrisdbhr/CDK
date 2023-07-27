using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

#if FMOD
using FMODUnity;
#endif

namespace CDK {
    public class CCutsceneSkipper : MonoBehaviour {

        #region <<---------- Initializers ---------->>

        public void Initialize(PlayableDirector p) {
            this._playableDirector = p;
        }

        #endregion <<---------- Initializers ---------->>

        
        
        
        #region <<---------- Properties and Fields ---------->>


        [SerializeField] private Canvas _progressCanvas;
        [SerializeField] private Image _progressImage;
        #if FMOD
        [SerializeField] private EventReference _soundOnSkip;
        #endif

        public float InputHoldSeconds {
            get => this._inputHoldSeconds;
            set {
                this._inputHoldSeconds = value.CClamp(0f, this._secondsToSkip);
                this._progressCanvas.enabled = (this._inputHoldSeconds > 0f);
                this._progressImage.fillAmount = this._inputHoldSeconds / this._secondsToSkip;
                if (this._inputHoldSeconds >= this._secondsToSkip) {
                    this.SkipCutscene();
                }
            }
        }

        private float _inputHoldSeconds;
        [Range(0.1f, 2f)] private float _secondsToSkip = 1.3f;
        private bool _isPlayingCutscene;
        private bool _skipped;
        private Coroutine _skipCoroutine;
        private PlayableDirector _playableDirector;
        

        #endregion <<---------- Properties and Fields ---------->>

        
        
        
        #region <<---------- Mono Behaviour ---------->>

        private void Awake() {
            this.InputHoldSeconds = 0f;
        }

        private void OnEnable() {
            this._skipCoroutine = this.CStartCoroutine(SkipRoutine());
        }

        private void OnDisable() {
            this.CStopCoroutine(this._skipCoroutine);
        }

        #endregion <<---------- Mono Behaviour ---------->>




        #region <<---------- General ---------->>

        IEnumerator SkipRoutine() {
            yield return new WaitForSeconds(1f);
            while (!_skipped) {
                yield return null;
                if (this._playableDirector.time <= 0 || this._playableDirector.time >= this._playableDirector.duration) {
                    this.InputHoldSeconds = 0f;
                }
                else {
                    if (Input.anyKey) {
                        this.InputHoldSeconds += CTime.DeltaTimeScaled;
                    }
                    else if (this.InputHoldSeconds > 0f) {
                        this.InputHoldSeconds -= CTime.DeltaTimeScaled;
                    }
                }
            }
        }

        public void SkipCutscene() {
            if (this._skipped) return;
            this._progressCanvas.enabled = false;
            this._playableDirector.time = this._playableDirector.duration;
            #if FMOD
            if(!this._soundOnSkip.IsNull) RuntimeManager.PlayOneShot(this._soundOnSkip);
            #endif
            this._skipped = true;
        }

        #endregion <<---------- General ---------->>

    }
}
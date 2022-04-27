using FMODUnity;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace CDK {
    public class CCutsceneSkipper : MonoBehaviour {

        [SerializeField] private PlayableDirector _cutsceneToSkip;
        [Space]
        [SerializeField] private Canvas _progressCanvas;
        [SerializeField] private Image _progressImage;
        [SerializeField] private EventReference _soundOnSkip;
        

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
        
        
        private void Awake() {
            this.InputHoldSeconds = 0f;
        }

        private void Update() {
            if (_skipped) return;
            if (this._cutsceneToSkip.time <= 0 || this._cutsceneToSkip.time >= this._cutsceneToSkip.duration) {
                this.InputHoldSeconds = 0f;
                return;
            }
            if (Input.anyKey) {
                this.InputHoldSeconds += Time.deltaTime;
            }
            else if (this.InputHoldSeconds > 0f) {
                this.InputHoldSeconds -= Time.deltaTime;
            }
        }



        public void SkipCutscene() {
            if (this._skipped) return;
            this._progressCanvas.enabled = false;
            this._cutsceneToSkip.time = this._cutsceneToSkip.duration;
            this._cutsceneToSkip.Stop();
            if(!this._soundOnSkip.IsNull) RuntimeManager.PlayOneShot(this._soundOnSkip);
            this._skipped = true;
        }
        
    }
}
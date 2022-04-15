using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace CDK {
    public class CCutsceneSkipper : MonoBehaviour {

        [SerializeField] private PlayableDirector _cutsceneToSkip;
        [Space]
        [SerializeField] private Canvas _progressCanvas;
        [SerializeField] private Image _progressImage;

        public float InputHoldSeconds {
            get => this._inputHoldSeconds;
            set {
                this._inputHoldSeconds = value.CClamp(0f, this._secondsToSkip);
                this._progressCanvas.enabled = (this._inputHoldSeconds > 0f);
                this._progressImage.fillAmount = this._inputHoldSeconds / this._secondsToSkip;
                if (this._inputHoldSeconds >= this._secondsToSkip) {
                    this._cutsceneToSkip.time = this._cutsceneToSkip.duration;
                    this._cutsceneToSkip.Stop();
                }
            }
        }

        private float _inputHoldSeconds;
        [Range(0.1f, 2f)] private float _secondsToSkip = 1.5f;
        private bool _isPlayingCutscene;
        
        
        
        private void Awake() {
            this.InputHoldSeconds = 0f;
        }

        private void Update() {
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
        
    }
}
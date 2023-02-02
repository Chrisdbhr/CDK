using UnityEngine;

namespace CDK.Audio {
    public class CMusicStopWhenInsideTrigger : CPhysicsTrigger {
       
        [SerializeField] private bool _allowFadeOut = true;
        [SerializeField] private bool _resumeWhenLeave = true;
        private CMusicForScene _musicForScene;
        
        
        
        
        protected override void StartedCollisionOrTrigger(Transform other) {
            base.StartedCollisionOrTrigger(other);
            this.FindAndTryStopMusic();
        }

        protected override void ExitedCollisionOrTrigger(Transform other) {
            base.ExitedCollisionOrTrigger(other);
            if (!_resumeWhenLeave || !this._musicForScene) return;
            this._musicForScene.Play();
        }

        public void FindAndTryStopMusic() {
            this._musicForScene = FindObjectOfType<CMusicForScene>();
            if (!this._musicForScene) {
                Debug.LogError($"Cant find '{nameof(CMusicForScene)}' to pause");
                return;
            }
            this._musicForScene.Stop(this._allowFadeOut);
        }
        
    }
}
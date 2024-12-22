using System;
using UnityEngine;
using UnityEngine.Playables;

namespace CDK {
    [RequireComponent(typeof(Animator))]
    public class CAnimatorFrameRate : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>

        [SerializeField][Min(1)] 
        int _delayFrames = 3;
        
        float _lastUpdateTime;
        Animator _animator;

        #endregion <<---------- Properties and Fields ---------->>


        

        #region <<---------- Mono Behaviour ---------->>

        private void Awake() {
            _animator = GetComponent<Animator>();
            _animator.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        }

        private void Update() {
            _animator.Update(0);
            if (Time.frameCount % ((int)_delayFrames) != 0) return;
            _animator.Update(Time.realtimeSinceStartup - _lastUpdateTime);
            //this._animator.playableGraph.Evaluate(Time.realtimeSinceStartup - _lastUpdateTime);
            _lastUpdateTime = Time.realtimeSinceStartup;
        }

        #endregion <<---------- Mono Behaviour ---------->>

    }
}
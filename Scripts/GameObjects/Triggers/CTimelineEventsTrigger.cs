using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace CDK {
	public class CTimelineEventsTrigger : MonoBehaviour {

		#region <<---------- Properties ---------->>

        [SerializeField] private bool _isPlayingCutscene;
		[SerializeField] protected PlayableDirector _playableDirector;
		[SerializeField] protected bool _autoSetIsPlayingCutsceneOnBlockingEventsManager = true;
		[SerializeField] protected UnityEvent _cutscenePlayed;
        [SerializeField] protected UnityEvent _cutsceneStopped;
        [SerializeField] protected CUnityEventBool _cutscenePlayingStateChanged;
		
        protected CBlockingEventsManager _blockingEventsManager;

		#endregion <<---------- Properties ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
            this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
            if (this._playableDirector.extrapolationMode != DirectorWrapMode.None) {
                Debug.LogError($"PlayableDirector {this._playableDirector.name} extrapolationMode must be set to None.");
            }
        }

        protected virtual void OnEnable() {
            this._playableDirector.played += OnCutscenePlayed;
			this._playableDirector.stopped += OnCutsceneStopped;
		}
		
        protected virtual void OnDisable() {
			this._playableDirector.played -= OnCutscenePlayed;
			this._playableDirector.stopped -= OnCutsceneStopped;
		}
		
		#if UNITY_EDITOR
		protected virtual void Reset() {
			if(!this._playableDirector) this._playableDirector = this.GetComponent<PlayableDirector>();
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		#region <<---------- Callbacks ---------->>
		
        protected virtual void OnCutscenePlayed(PlayableDirector playableDirector) {
			if(_autoSetIsPlayingCutsceneOnBlockingEventsManager) _blockingEventsManager.IsPlayingCutscene = true;
            this._cutscenePlayed?.Invoke();
            this._cutscenePlayingStateChanged?.Invoke(true);
            _isPlayingCutscene = true;
        }

        protected virtual void OnCutsceneStopped(PlayableDirector playableDirector) {
			if(_autoSetIsPlayingCutsceneOnBlockingEventsManager) _blockingEventsManager.IsPlayingCutscene = false;
            this._cutsceneStopped?.Invoke();
            this._cutscenePlayingStateChanged?.Invoke(false);
            _isPlayingCutscene = false;
		}
		
		#endregion <<---------- Callbacks ---------->>
		
	}
}

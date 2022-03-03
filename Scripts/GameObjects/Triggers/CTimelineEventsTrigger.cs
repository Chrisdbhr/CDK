using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace CDK {
	public class CTimelineEventsTrigger : MonoBehaviour {

		#region <<---------- Properties ---------->>
		
		[SerializeField] private PlayableDirector _playableDirector;
		[SerializeField] private bool _autoSetIsPlayingCutsceneOnBlockingEventsManager = true;
		[SerializeField] private UnityEvent _cutscenePlayed;
		[SerializeField] private UnityEvent _cutsceneStopped;
		
		private CBlockingEventsManager _blockingEventsManager;

		#endregion <<---------- Properties ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			_blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
		}

		private void OnEnable() {
			this._playableDirector.played += OnCutscenePlayed;
			this._playableDirector.stopped += OnCutsceneStopped;
		}
		
		private void OnDisable() {
			this._playableDirector.played -= OnCutscenePlayed;
			this._playableDirector.stopped -= OnCutsceneStopped;
		}
		
		#if UNITY_EDITOR
		void Reset() {
			this._playableDirector = this.GetComponent<PlayableDirector>();
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		#region <<---------- Callbacks ---------->>
		
		private void OnCutscenePlayed(PlayableDirector playableDirector) {
			if(_autoSetIsPlayingCutsceneOnBlockingEventsManager) _blockingEventsManager.IsPlayingCutscene = true;
			_cutscenePlayed?.Invoke();
		}

		void OnCutsceneStopped(PlayableDirector playableDirector) {
			if(_autoSetIsPlayingCutsceneOnBlockingEventsManager) _blockingEventsManager.IsPlayingCutscene = false;
			_cutsceneStopped?.Invoke();
		}
		
		#endregion <<---------- Callbacks ---------->>
		
	}
}

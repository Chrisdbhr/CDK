using System;
using R3;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	[Obsolete("Use "+nameof(CBlockingEventsTrigger)+" instead")]
	public class CIsPlayingCutsceneTrigger : MonoBehaviour {
		
		[SerializeField] UnityEvent _isPlayingEvent;
		[SerializeField] UnityEvent _isNotPlayingEvent;
		[SerializeField] CUnityEventBool _isPlayingTriggerEvent;
		[SerializeField] CUnityEventBool _isNotPlayingTriggerEvent;
		[Inject] readonly CBlockingEventsManager _blockingEventsManager;
        IDisposable _disposeOnDisable;


		private void OnEnable() {
			this._disposeOnDisable = this._blockingEventsManager.PlayingCutsceneRetainable.IsRetainedAsObservable()
            .Subscribe(this.PlayingStateChanged);
		}

        private void OnDisable() {
            this._disposeOnDisable?.Dispose();
        }

        void PlayingStateChanged(bool isPlaying) {
			if(isPlaying) _isPlayingEvent?.Invoke();
			else _isNotPlayingEvent?.Invoke();
			
			this._isPlayingTriggerEvent?.Invoke(isPlaying);
			this._isNotPlayingTriggerEvent?.Invoke(!isPlaying);
		}

	}
}

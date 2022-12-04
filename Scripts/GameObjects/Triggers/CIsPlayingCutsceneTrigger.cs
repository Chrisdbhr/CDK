using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CIsPlayingCutsceneTrigger : MonoBehaviour {
		
		[SerializeField] private UnityEvent _isPlayingEvent;
		[SerializeField] private UnityEvent _isNotPlayingEvent;
		[SerializeField] private CUnityEventBool _isPlayingTriggerEvent;
		[SerializeField] private CUnityEventBool _isNotPlayingTriggerEvent;
		private CBlockingEventsManager _blockingEventsManager;

		private void Awake() {
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
		}

		private void OnEnable() {
			this._blockingEventsManager.PlayingCutsceneRetainable.IsRetainedRx.DistinctUntilChanged().TakeUntilDisable(this).Subscribe(this.PlayingStateChanged);
		}

		void PlayingStateChanged(bool isPlaying) {
			if(isPlaying) _isPlayingEvent?.Invoke();
			else _isNotPlayingEvent?.Invoke();
			
			this._isPlayingTriggerEvent?.Invoke(isPlaying);
			this._isNotPlayingTriggerEvent?.Invoke(!isPlaying);
		}

	}
}

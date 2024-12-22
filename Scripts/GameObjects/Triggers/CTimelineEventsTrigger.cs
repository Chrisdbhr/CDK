using System;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace CDK {
    [Obsolete("Callbacks not working as expected.")]
	public class CTimelineEventsTrigger : MonoBehaviour {

		#region <<---------- Properties ---------->>

		bool IsPlayingState {
            set {
                if (value == _isPlayingCutscene) return;
                _isPlayingCutscene = value;
                if (_isPlayingCutscene) {
                    OnCutscenePlayed();
                }
            }
        }
        [SerializeField] bool _isPlayingCutscene;
		[SerializeField] protected PlayableDirector _playableDirector;
		[SerializeField] [Obsolete("Not working as expected.")] protected bool _autoSetIsPlayingCutsceneOnBlockingEventsManager = true;
		[SerializeField] protected UnityEvent _cutscenePlayed;
        [SerializeField] protected UnityEvent _cutsceneStopped;
        [SerializeField] protected CUnityEventBool _cutscenePlayingStateChanged;
		
        [Inject] protected readonly CBlockingEventsManager _blockingEventsManager;

		#endregion <<---------- Properties ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
            if (_playableDirector.extrapolationMode != DirectorWrapMode.None) {
                Debug.LogError($"PlayableDirector {_playableDirector.name} extrapolationMode must be set to None.");
            }
        }

        protected virtual void OnEnable() {
			_playableDirector.stopped += OnCutsceneStopped;
		}
		
        protected virtual void OnDisable() {
			_playableDirector.stopped -= OnCutsceneStopped;
		}

		void Update() {
            IsPlayingState = _playableDirector.time > 0;
        }

		#if UNITY_EDITOR
		protected virtual void Reset() {
            if (!_playableDirector) {
                _playableDirector = GetComponent<PlayableDirector>();
            }
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		#region <<---------- Callbacks ---------->>
		
        protected virtual void OnCutscenePlayed() {
            if (_autoSetIsPlayingCutsceneOnBlockingEventsManager) {
                _blockingEventsManager.PlayingCutsceneRetainable.Retain(this);
            }
            _cutscenePlayed?.Invoke();
            _cutscenePlayingStateChanged?.Invoke(true);
            _isPlayingCutscene = true;
            Debug.Log($"Played cutscene {name}");
        }

        protected virtual void OnCutsceneStopped(PlayableDirector playableDirector) {
            if (_autoSetIsPlayingCutsceneOnBlockingEventsManager) {
                _blockingEventsManager.PlayingCutsceneRetainable.Release(this);
            }
            _cutsceneStopped?.Invoke();
            _cutscenePlayingStateChanged?.Invoke(false);
            _isPlayingCutscene = false;
            Debug.Log($"Stopped cutscene {name}");
		}
		
		#endregion <<---------- Callbacks ---------->>
		
	}
}

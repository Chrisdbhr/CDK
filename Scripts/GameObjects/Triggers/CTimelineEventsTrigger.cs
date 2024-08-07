﻿using System;
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

        private bool IsPlayingState {
            set {
                if (value == this._isPlayingCutscene) return;
                this._isPlayingCutscene = value;
                if (this._isPlayingCutscene) {
                    OnCutscenePlayed();
                }
            }
        }
        [SerializeField] private bool _isPlayingCutscene;
		[SerializeField] protected PlayableDirector _playableDirector;
		[SerializeField] [Obsolete("Not working as expected.")] protected bool _autoSetIsPlayingCutsceneOnBlockingEventsManager = true;
		[SerializeField] protected UnityEvent _cutscenePlayed;
        [SerializeField] protected UnityEvent _cutsceneStopped;
        [SerializeField] protected CUnityEventBool _cutscenePlayingStateChanged;
		
        [Inject] protected readonly CBlockingEventsManager _blockingEventsManager;

		#endregion <<---------- Properties ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
            if (this._playableDirector.extrapolationMode != DirectorWrapMode.None) {
                Debug.LogError($"PlayableDirector {this._playableDirector.name} extrapolationMode must be set to None.");
            }
        }

        protected virtual void OnEnable() {
			this._playableDirector.stopped += OnCutsceneStopped;
		}
		
        protected virtual void OnDisable() {
			this._playableDirector.stopped -= OnCutsceneStopped;
		}

        private void Update() {
            this.IsPlayingState = this._playableDirector.time > 0;
        }

		#if UNITY_EDITOR
		protected virtual void Reset() {
            if (!this._playableDirector) {
                this._playableDirector = this.GetComponent<PlayableDirector>();
            }
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		#region <<---------- Callbacks ---------->>
		
        protected virtual void OnCutscenePlayed() {
            if (_autoSetIsPlayingCutsceneOnBlockingEventsManager) {
                this._blockingEventsManager.PlayingCutsceneRetainable.Retain(this);
            }
            this._cutscenePlayed?.Invoke();
            this._cutscenePlayingStateChanged?.Invoke(true);
            _isPlayingCutscene = true;
            Debug.Log($"Played cutscene {this.name}");
        }

        protected virtual void OnCutsceneStopped(PlayableDirector playableDirector) {
            if (_autoSetIsPlayingCutsceneOnBlockingEventsManager) {
                this._blockingEventsManager.PlayingCutsceneRetainable.Release(this);
            }
            this._cutsceneStopped?.Invoke();
            this._cutscenePlayingStateChanged?.Invoke(false);
            _isPlayingCutscene = false;
            Debug.Log($"Stopped cutscene {this.name}");
		}
		
		#endregion <<---------- Callbacks ---------->>
		
	}
}

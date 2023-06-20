using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if FMOD
using FMODUnity;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	[RequireComponent(typeof(PlayableDirector))]
	public class CPlayableHelper : MonoBehaviour {
		private PlayableDirector _playable;

		[SerializeField] private UnityEvent _onCutsceneStarted;
		[SerializeField] private UnityEvent _onCutsceneEnded;
		[SerializeField] private bool _canSkipCutscene = true;
		[SerializeField] private bool _destroyGameObjectOnFinished;
		[SerializeField] private bool _preventAutoFadeOutOnExit;
		[SerializeField] private GameObject[] _disableOnExit;
		private CCutsceneSkipper _cutsceneSkipperSpawned;
        [SerializeField, Range(-1, 5f)] private float _startFadeInTime = -1f;
        
		
		public bool IsPlaying {
			get {
				return this._isPlaying;
			}
			set {
				if (!Application.isPlaying || CApplication.IsQuitting) return;
				if (value == this._isPlaying) return;
				this._isPlaying = value;
				
				if (this._isPlaying) {
					// cutscene started
					this._blockingEventsManager.PlayingCutsceneRetainable.Retain(this);
					this._onCutsceneStarted?.Invoke();
					this.CreateCutsceneSkipper();
                    if (this._startFadeInTime >= 0f) {
                        CFader.get.FadeToTransparent(this._startFadeInTime);
                    }
				}
				else {
					// cutscene ended
					if (!this._preventAutoFadeOutOnExit) {
						CFader.get.FadeToTransparent(1f);
					}
					this._blockingEventsManager.PlayingCutsceneRetainable.Release(this);
					this._onCutsceneEnded?.Invoke();
					if (_disableOnExit.Length > 0) {
						foreach (var go in _disableOnExit) {
							if (go == null) continue;
							Debug.Log($"Disabling {go.name} after finishing cutscene {this.name}");
							go.SetActive(false);
						}
					}
					if (this._destroyGameObjectOnFinished) {
						this.gameObject.CDestroy();
					}

					if (this._cutsceneSkipperSpawned != null) {
						this._cutsceneSkipperSpawned.gameObject.CDestroy();
					}
				}
			}
		}
		private bool _isPlaying;
		private CBlockingEventsManager _blockingEventsManager;
		
		
		
		
		
		#if UNITY_EDITOR
		[MenuItem("CONTEXT/PlayableDirector/Add Playable Helper")]
		private static void RenameGameObjectWithThisComponentName(MenuCommand data) {
			var p = data.context as PlayableDirector;
			if (p == null) return;
			Undo.RecordObject(p.gameObject, "Add component");
			p.gameObject.CGetOrAddComponent<CPlayableHelper>();
		}
		#endif

		#if UNITY_EDITOR
		private void Reset() {
			Undo.RecordObject(this.gameObject, "Add component");
			this.gameObject.CGetOrAddComponent<SignalReceiver>();
            OnValidate();
        }

		private void OnValidate() {
            this._playable = this.GetComponent<PlayableDirector>();
			if (this._playable != null && this._playable.playOnAwake) {
				Debug.LogError($"No cutscene should be set as playOnAwake since it interferes in event handling.");
				this._playable.playOnAwake = false;
				EditorUtility.SetDirty(this._playable);
			}
		}

		#endif

		private void Awake() {
			this._playable = this.GetComponent<PlayableDirector>();
			this._blockingEventsManager = CBlockingEventsManager.get;

			if (this._playable.state == PlayState.Playing) {
				PlayableStarted(this._playable);
			}
			else {
				this._playable.played += PlayableStarted; 
			}

            #if FMOD
			// unmute FMOD tracks
			if (_playable.playableAsset is TimelineAsset timeline) {
				foreach (var track in timeline.GetOutputTracks().Where(t => t.GetType() == typeof(FMODEventTrack))) {
					Debug.Log($"Unmuting FMOD track '{track.name}'");
					track.muted = false;
				}	
			}
            #endif
		}


		private void OnDestroy() {
			this._playable.played -= PlayableStarted; 
		}

		private void PlayableStarted(PlayableDirector p) {
			IsPlaying = true;
			this._playable.stopped += PlayableStopped;
		}

		private void PlayableStopped(PlayableDirector p) {
			IsPlaying = false;
			this._playable.stopped -= PlayableStopped;
		}

		public void FadeToBlack(float time) {
			CFader.get.FadeToBlack(time);
		}
		
		public void FadeToTransparent(float time) {
			CFader.get.FadeToTransparent(time);
		}

		private void CreateCutsceneSkipper() {
			if (!this._canSkipCutscene || this._cutsceneSkipperSpawned != null) return;
			this._cutsceneSkipperSpawned = CAssets.LoadResourceAndInstantiate<CCutsceneSkipper>("Cutscene Skipper", this.transform);
			this._cutsceneSkipperSpawned.Initialize(this._playable);
		}
	}
}

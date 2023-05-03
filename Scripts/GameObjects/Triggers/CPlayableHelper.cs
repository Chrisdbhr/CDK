using System;
using System.Linq;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	[RequireComponent(typeof(PlayableDirector))]
	public class CPlayableHelper : MonoBehaviour {
		private PlayableDirector _playable;

		[SerializeField] private UnityEvent _onCutsceneStarted;
		[SerializeField] private UnityEvent _onCutsceneEnded;
		[SerializeField] private bool _destroyGameObjectOnFinished;
		[SerializeField] private bool _preventAutoFadeOutOnExit;
		[SerializeField] private GameObject[] _disableOnExit;
		
		public bool IsPlaying {
			get {
				return this._isPlaying;
			}
			set {
				if (value == this._isPlaying) return;
				this._isPlaying = value;
				if (this._isPlaying) {
					this._blockingEventsManager.PlayingCutsceneRetainable.Retain(this);
					this._onCutsceneStarted?.Invoke();
				}
				else {
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
		}
		#endif

		private void Awake() {
			this._playable = this.GetComponent<PlayableDirector>();
			this._blockingEventsManager = CBlockingEventsManager.get;
			
			this._playable.played += PlayableStarted; 

			// unmute FMOD tracks
			if (_playable.playableAsset is TimelineAsset timeline) {
				foreach (var track in timeline.GetOutputTracks().Where(t => t.GetType() == typeof(FMODEventTrack))) {
					track.muted = false;
				}	
			}
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
	}
}

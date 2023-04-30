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
	public class CPlayableSetPlayingCutscene : MonoBehaviour {
		private PlayableDirector _playable;

		[SerializeField] private UnityEvent _onCutsceneEnded;
		[SerializeField] private bool _destroyGameObjectOnFinished;
		
		public bool IsPlaying {
			get {
				return this._isPlaying;
			}
			set {
				if (value == this._isPlaying) return;
				this._isPlaying = value;
				CFader.get.FadeToTransparent(1f);
				if (this._isPlaying) {
					this._blockingEventsManager.PlayingCutsceneRetainable.Retain(this);
				}
				else {
					this._blockingEventsManager.PlayingCutsceneRetainable.Release(this);
					this._onCutsceneEnded?.Invoke();
					if (this._destroyGameObjectOnFinished) {
						this.gameObject.CDestroy();
					}
				}
			}
		}
		private bool _isPlaying;
		private CBlockingEventsManager _blockingEventsManager;
		
		
		
		
		
		#if UNITY_EDITOR
		[MenuItem("CONTEXT/PlayableDirector/Add IsPlayingCutscene Monitor")]
		private static void RenameGameObjectWithThisComponentName(MenuCommand data) {
			var p = data.context as PlayableDirector;
			if (p == null) return;
			Undo.RecordObject(p.gameObject, "Add component");
			p.gameObject.CGetOrAddComponent<CPlayableSetPlayingCutscene>();
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

			// unmute FMOD tracks
			if (_playable.playableAsset is TimelineAsset timeline) {
				foreach (var track in timeline.GetOutputTracks().Where(t => t.GetType() == typeof(FMODEventTrack))) {
					track.muted = false;
				}	
			}
		}

		private void LateUpdate() {
			double time = this._playable.time;
			IsPlaying = time > 0f && time < this._playable.duration;
		}

		public void FadeToBlack() {
			CFader.get.FadeToBlack(1f);
		}
		
		public void FadeToTransparent() {
			CFader.get.FadeToTransparent(1f);
		}
	}
}

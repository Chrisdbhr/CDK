using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	[RequireComponent(typeof(PlayableDirector))]
	public class CPlayableSetPlayingCutscene : MonoBehaviour {
		private PlayableDirector _playable;

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
			p.gameObject.AddComponent<CPlayableSetPlayingCutscene>();
		}
		#endif


		private void Reset() {
			this.gameObject.AddComponent<SignalReceiver>();
		}

		private void Awake() {
			this._playable = this.GetComponent<PlayableDirector>();
			this._blockingEventsManager = CBlockingEventsManager.get;
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

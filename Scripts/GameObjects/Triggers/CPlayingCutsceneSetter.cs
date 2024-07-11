using System;
using Reflex.Attributes;
using UnityEngine;

namespace CDK {
    [Obsolete("Use PlayableHelper instead")]
	public class CPlayingCutsceneSetter : MonoBehaviour {

		[SerializeField] private bool _setOnEnableDisable = true;
		[Inject] readonly CBlockingEventsManager _blockingEventsManager;
		
		
		


		private void OnEnable() {
			if (!_setOnEnableDisable) return;
            this._blockingEventsManager.PlayingCutsceneRetainable.Retain(this);
		}

		private void OnDisable() {
			if (!_setOnEnableDisable) return;
            this._blockingEventsManager.PlayingCutsceneRetainable.Release(this);
		}

		public void SetPlayingState(bool isPlaying) {
			if(isPlaying) this._blockingEventsManager.PlayingCutsceneRetainable.Retain(this);
            else this._blockingEventsManager.PlayingCutsceneRetainable.Release(this);
		}
	}
}

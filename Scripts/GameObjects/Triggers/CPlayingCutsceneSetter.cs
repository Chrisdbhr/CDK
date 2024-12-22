using System;
using Reflex.Attributes;
using UnityEngine;

namespace CDK {
    [Obsolete("Use PlayableHelper instead")]
	public class CPlayingCutsceneSetter : MonoBehaviour {

		[SerializeField] bool _setOnEnableDisable = true;
		[Inject] readonly CBlockingEventsManager _blockingEventsManager;


		void OnEnable() {
			if (!_setOnEnableDisable) return;
            _blockingEventsManager.PlayingCutsceneRetainable.Retain(this);
		}

		void OnDisable() {
			if (!_setOnEnableDisable) return;
            _blockingEventsManager.PlayingCutsceneRetainable.Release(this);
		}

		public void SetPlayingState(bool isPlaying) {
			if(isPlaying) _blockingEventsManager.PlayingCutsceneRetainable.Retain(this);
            else _blockingEventsManager.PlayingCutsceneRetainable.Release(this);
		}
	}
}

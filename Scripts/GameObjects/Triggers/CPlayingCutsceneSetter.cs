using System;
using UnityEngine;

namespace CDK {
	public class CPlayingCutsceneSetter : MonoBehaviour {

		[SerializeField] private bool _setOnEnableDisable = true;
		private CBlockingEventsManager _blockingEventsManager;
		
		
		
        
		private void Awake() {
			_blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
		}

		private void OnEnable() {
			if (!_setOnEnableDisable) return;
			_blockingEventsManager.IsPlayingCutscene = true;
		}

		private void OnDisable() {
			if (!_setOnEnableDisable) return;
			_blockingEventsManager.IsPlayingCutscene = false;
		}

		public void SetPlayingState(bool isPlaying) {
			this._blockingEventsManager.IsPlayingCutscene = isPlaying;
		}
	}
}

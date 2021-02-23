using System;
using UnityEngine;

namespace CDK {
	public class CBlockingEventsTrigger : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private CUnityEventBool _isBlockingEvent;
		[SerializeField] private CUnityEventBool _onCutsceneEvent;
		[SerializeField] private CUnityEventBool _onMenuEvent;

		#endregion <<---------- Properties and Fields ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>

		private void OnEnable() {
			CBlockingEventsManager.BlockingEventHappeningEvent += this.OnBlockingEvent;
			CBlockingEventsManager.PlayingCutsceneEvent += this.OnCutsceneEvent;
			CBlockingEventsManager.OnMenuEvent += this.OnMenuEvent;
		}

		private void OnDisable() {
			CBlockingEventsManager.BlockingEventHappeningEvent -= this.OnBlockingEvent;
			CBlockingEventsManager.PlayingCutsceneEvent -= this.OnCutsceneEvent;
			CBlockingEventsManager.OnMenuEvent -= this.OnMenuEvent;
		}
		
		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		#region <<---------- Events ---------->>

		private void OnBlockingEvent(bool isBlocking) {
			this._isBlockingEvent?.Invoke(isBlocking);
		}
		
		private void OnCutsceneEvent(bool onCutscene) {
			this._onCutsceneEvent?.Invoke(onCutscene);
		}
		
		private void OnMenuEvent(bool onMenu) {
			this._onMenuEvent?.Invoke(onMenu);
		}
		
		#endregion <<---------- Events ---------->>




		#region <<---------- Triggers ---------->>

		public void TriggerIsOnMenuBlockingEvent(bool block = true) {
			CBlockingEventsManager.IsOnMenu = block;
		}

		public void TriggerIsPlayingCutsceneBlockingEvent(bool block = true) {
			CBlockingEventsManager.IsPlayingCutscene = block;
		}
		
		#endregion <<---------- Triggers ---------->>
	}
}

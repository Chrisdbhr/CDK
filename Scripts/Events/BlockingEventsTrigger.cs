using UnityEngine;

namespace CDK {
	public class BlockingEventsTrigger : MonoBehaviour {

		public void TriggerIsOnMenuBlockingEvent(bool block = true) {
			CBlockingEventsManager.IsOnMenu = block;
		}

		public void TriggerIsPlayingCutsceneBlockingEvent(bool block = true) {
			CBlockingEventsManager.IsPlayingCutscene = block;
		}
		
	}
}

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CDK {
	public class CBlockingEventsManager {
                
		public bool InMenuOrPlayingCutscene => IsInMenu || IsPlayingCutscene;

        public bool IsPlayingCutscene => PlayingCutsceneRetainable.IsRetained;
        public readonly CRetainable PlayingCutsceneRetainable = new ();

        public bool IsInMenu => MenuRetainable.IsRetained;
        public readonly CRetainable MenuRetainable = new ();

        public event Action<bool> InMenuOrPlayingCutsceneEvent = delegate { };


        public CBlockingEventsManager() {
	        // on menu
	        MenuRetainable.StateEvent += onMenu => {
		        Debug.Log($"<color=#4fafb6>{nameof(onMenu)}: <b>{onMenu}</b></color>");
		        InMenuOrPlayingCutsceneEvent.Invoke(InMenuOrPlayingCutscene);
	        };

	        // playing cutscene
	        PlayingCutsceneRetainable.StateEvent += isPlayingCutscene => {
		        Debug.Log($"<color=#cc5636>{nameof(isPlayingCutscene)}: <b>{isPlayingCutscene}</b></color>");
		        InMenuOrPlayingCutsceneEvent.Invoke(InMenuOrPlayingCutscene);
	        };
        }

	}
}
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CDK {
	public class CBlockingEventsManager {
                
		public bool IsOnMenuOrPlayingCutscene => IsPlayingCutscene || IsOnMenu;

        public bool IsPlayingCutscene => PlayingCutsceneRetainable.IsRetained;
        public CRetainable PlayingCutsceneRetainable { get; private set; }

        public bool IsOnMenu => OnMenuRetainable.IsRetained;
        public CRetainable OnMenuRetainable { get; private set; }

        public bool IsAnyHappening {get; private set; }

        public event EventHandler<bool> OnAnyEventHappeningChanged = delegate { };


        public CBlockingEventsManager() {
	        // on menu
	        OnMenuRetainable = new CRetainable();
	        OnMenuRetainable.OnRetainedStateChanged += ((sender,onMenu) => {
		        Debug.Log($"<color={"#4fafb6"}>{nameof(onMenu)}: <b>{onMenu}</b></color>", sender as Object);
		        OnAnyEventHappeningChanged.Invoke(this, IsAnyHappening);
	        });

	        // playing cutscene
	        PlayingCutsceneRetainable = new CRetainable();
	        PlayingCutsceneRetainable.OnRetainedStateChanged += ((sender,isPlayingCutscene) => {
		        Debug.Log($"<color={"#cc5636"}>{nameof(isPlayingCutscene)}: <b>{isPlayingCutscene}</b></color>");
		        OnAnyEventHappeningChanged.Invoke(this, IsAnyHappening);
	        });
        }

	}
}
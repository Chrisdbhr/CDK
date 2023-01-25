using System;
using UniRx;
using UnityEngine;

namespace CDK {
	public class CBlockingEventsManager {
		
		#region <<---------- Properties ---------->>

        // --- start Blocking Events

        public bool IsAnyExceptMenu => IsPlayingCutscene || IsLimitingPlayerActions;
        
        public bool IsPlayingCutscene => this.PlayingCutsceneRetainable.IsRetained;
        public CRetainable PlayingCutsceneRetainable { get; private set; }

        public bool IsOnMenu => this.OnMenuRetainable.IsRetained;
        public CRetainable OnMenuRetainable { get; private set; }

        public bool IsLimitingPlayerActions => this.LimitPlayerActionsRetainable.IsRetained;
        public CRetainable LimitPlayerActionsRetainable { get; private set; }
        
        // --- end Blocking Events


        public bool IsAnyHappening => this._isAnyHappeningRx.Value;
        private BoolReactiveProperty _isAnyHappeningRx;
        
        private CompositeDisposable _disposables;
        
		#endregion <<---------- Properties ---------->>

		
		

		#region <<---------- Initializers ---------->>
		
		public CBlockingEventsManager() {

            if (!Application.isPlaying) return;
            
            this._disposables?.Dispose();
            this._disposables = new CompositeDisposable();

			// on menu
			this.OnMenuRetainable = new CRetainable();
			this.OnMenuRetainable.IsRetainedRx.Subscribe(onMenu => {
				Debug.Log($"<color={"#4fafb6"}>{nameof(onMenu)}: {onMenu}</color>");
			})
            .AddTo(this._disposables);
			
			// playing cutscene
			this.PlayingCutsceneRetainable = new CRetainable();
			this.PlayingCutsceneRetainable.IsRetainedRx.Subscribe(isPlayingCutscene => {
				Debug.Log($"<color={"#cc5636"}>{nameof(isPlayingCutscene)}: {isPlayingCutscene}</color>");
			})
            .AddTo(this._disposables);

            // limit player actions
            this.LimitPlayerActionsRetainable = new CRetainable();
            this.LimitPlayerActionsRetainable.IsRetainedRx.Subscribe(isLimitingPlayerActions => {
                //Debug.Log($"<color={"#bb91ff"}>{nameof(isLimitingPlayerActions)}: {isLimitingPlayerActions}</color>");
            })
            .AddTo(this._disposables);
            
			// blocking Event Happening
            this._isAnyHappeningRx = new BoolReactiveProperty();
			Observable.CombineLatest(
			this.OnMenuRetainable.IsRetainedRx, 
			this.PlayingCutsceneRetainable.IsRetainedRx,
            this.LimitPlayerActionsRetainable.IsRetainedRx,
			(isOnMenu, isPlayingCutscene, limitPlayerActions) => isOnMenu || isPlayingCutscene || limitPlayerActions)
			.Subscribe(blockingEventHappening => {
				this._isAnyHappeningRx.Value = blockingEventHappening;
			})
            .AddTo(this._disposables);
		}

		#endregion <<---------- Initializers ---------->>




        #region <<---------- Observables ---------->>

        public IObservable<bool> IsAnyHappeningAsObservable() {
            return this._isAnyHappeningRx.DistinctUntilChanged().AsObservable();
        }

        #endregion <<---------- Observables ---------->>
		
	}
}
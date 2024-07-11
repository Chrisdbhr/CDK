using System;
using System.Linq;
using R3;
using UnityEngine;

namespace CDK {
	public class CBlockingEventsManager : IDisposable {

		#region <<---------- Initializers ---------->>
		
		public CBlockingEventsManager() {
            this._disposables?.Dispose();
            this._disposables = new CompositeDisposable();

			// on menu
			this.OnMenuRetainable = new CRetainable();
			this.OnMenuRetainable.IsRetainedAsObservable().Subscribe(onMenu => {
				Debug.Log($"<color={"#4fafb6"}>{nameof(onMenu)}: <b>{onMenu}</b></color>");
			})
            .AddTo(this._disposables);
			
			// playing cutscene
			this.PlayingCutsceneRetainable = new CRetainable();
			this.PlayingCutsceneRetainable.IsRetainedAsObservable().Subscribe(isPlayingCutscene => {
				Debug.Log($"<color={"#cc5636"}>{nameof(isPlayingCutscene)}: <b>{isPlayingCutscene}</b></color>");
			})
            .AddTo(this._disposables);

			// blocking Event Happening
            this._isAnyHappeningRx = new ReactiveProperty<bool>();
			Observable.CombineLatest(
			    this.OnMenuRetainable.IsRetainedAsObservable(),
			    this.PlayingCutsceneRetainable.IsRetainedAsObservable())
			.Subscribe(blockingEventHappening => {
				this._isAnyHappeningRx.Value = blockingEventHappening.Any(b=>b);
			})
            .AddTo(this._disposables);
		}

		~CBlockingEventsManager() {
			Dispose();
		}

		#endregion <<---------- Initializers ---------->>
        
        
        
                
        #region <<---------- Properties ---------->>

        // --- start Blocking Events

		public bool IsOnMenuOrPlayingCutscene => this.IsPlayingCutscene || this.IsOnMenu;

        public bool IsPlayingCutscene => this.PlayingCutsceneRetainable.IsRetained;
        public CRetainable PlayingCutsceneRetainable { get; private set; }

        public bool IsOnMenu => this.OnMenuRetainable.IsRetained;
        public CRetainable OnMenuRetainable { get; private set; }

        // --- end Blocking Events


        public bool IsAnyHappening => this._isAnyHappeningRx.Value;

        private ReactiveProperty<bool> _isAnyHappeningRx;
        
        private CompositeDisposable _disposables;
        
        #endregion <<---------- Properties ---------->>

        


        #region <<---------- Observables ---------->>

        public Observable<bool> IsAnyHappeningAsObservable() {
            return this._isAnyHappeningRx.DistinctUntilChanged().AsObservable();
        }

        #endregion <<---------- Observables ---------->>


        public void Dispose() {
	        _isAnyHappeningRx?.Dispose();
	        _disposables?.Dispose();
        }

	}
}
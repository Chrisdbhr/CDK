using System;
using UniRx;
using UnityEngine;

namespace CDK {
	public class CBlockingEventsManager {
		
		#region <<---------- Properties ---------->>
		
		private static CBlockingEventsManager Instance {
			get { return _instance ?? new CBlockingEventsManager(); }
		}
		private static CBlockingEventsManager _instance;
		
		
		public static bool IsBlockingEventHappening {
			get { return Instance._isBlockingEventHappening; }
		}
		private bool _isBlockingEventHappening;

		
		public static bool IsPlayingCutscene {
			get { return Instance._isPlayingCutsceneRx.Value; }
			set { Instance._isPlayingCutsceneRx.Value = value; }
		}
		private BoolReactiveProperty _isPlayingCutsceneRx = new BoolReactiveProperty();


		public static bool IsOnMenu {
			get { return Instance._isOnMenuRx.Value; }
			set { Instance._isOnMenuRx.Value = value; }
		}
		private BoolReactiveProperty _isOnMenuRx = new BoolReactiveProperty();

		
		private bool _applicationIsQuitting;

		
		#endregion <<---------- Properties ---------->>


		#region <<---------- Events ---------->>
		
		public static event Action<bool> PlayingCutsceneEvent {
			add {
				Instance._playingCutsceneEvent -= value;
				Instance._playingCutsceneEvent += value;
			}
			remove {
				Instance._playingCutsceneEvent -= value;
			}
		}
		private event Action<bool> _playingCutsceneEvent;

		
		public static event Action<bool> OnMenuEvent {
			add {
				Instance._isOnMenuEvent -= value;
				Instance._isOnMenuEvent += value;
			}
			remove {
				Instance._isOnMenuEvent -= value;
			}
		}
		private event Action<bool> _isOnMenuEvent;
		
		
		public static event Action<bool> BlockingEventHappeningEvent {
			add {
				Instance._isBlockingEventHappeningEvent -= value;
				Instance._isBlockingEventHappeningEvent += value;
			}
			remove {
				Instance._isBlockingEventHappeningEvent -= value;
			}
		}
		private event Action<bool> _isBlockingEventHappeningEvent;
		
		#endregion <<---------- Events ---------->>

		
		

		#region <<---------- Initializers ---------->>
		
		private CBlockingEventsManager() {
			Debug.Log("Creating BlockingEventsManager instance.");
			_instance = this;

			// app is quitting
			this._applicationIsQuitting = false;
			CApplication.IsQuitting += () => {
				this._applicationIsQuitting = true;
				_instance = null;
			};
			
			// on menu
			this._isOnMenuRx?.Dispose();
			this._isOnMenuRx = new BoolReactiveProperty();
			this._isOnMenuRx.Subscribe(onMenu => {
				Debug.Log($"IsOnMenuEvent: {onMenu}");
				this._isOnMenuEvent?.Invoke(onMenu);
			});
			
			// playing cutscene
			this._isPlayingCutsceneRx?.Dispose();
			this._isPlayingCutsceneRx = new BoolReactiveProperty();
			this._isPlayingCutsceneRx.Subscribe(isPlayingCutscene => {
				Debug.Log($"IsPlayingCutscene: {isPlayingCutscene}");
				this._playingCutsceneEvent?.Invoke(isPlayingCutscene);
			});

			// blocking Event Happening
			Observable.CombineLatest(
						  this._isOnMenuRx, this._isPlayingCutsceneRx,
						  (isOnMenu, isPlayingCutscene) => isOnMenu || isPlayingCutscene)
					  .Subscribe(blockingEventHappening => {
						  Debug.Log($"IsBlockingEventHappening: {blockingEventHappening}");
						  this._isBlockingEventHappening = blockingEventHappening;
						  this._isBlockingEventHappeningEvent?.Invoke(blockingEventHappening);
					  });
		}

		#endregion <<---------- Initializers ---------->>

	}
}
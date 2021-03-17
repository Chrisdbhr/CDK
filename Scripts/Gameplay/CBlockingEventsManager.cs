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
		
		
		public static bool IsAnyBlockingEventHappening {
			get { return Instance._isAnyBlockingEventHappening; }
		}
		private bool _isAnyBlockingEventHappening;

		
		public static bool IsPlayingCutscene {
			get { return Instance._isPlayingCutsceneRx.Value; }
			set { Instance._isPlayingCutsceneRx.Value = value; }
		}
		private BoolReactiveProperty _isPlayingCutsceneRx;


		public static bool IsOnMenu {
			get { return Instance._isOnMenuRx.Value; }
			set { Instance._isOnMenuRx.Value = value; }
		}
		private BoolReactiveProperty _isOnMenuRx;

		public static CRetainable IsDoingBlockingAction {
			get { return Instance._isDoingBlockingAction; }
			set { Instance._isDoingBlockingAction = value; }
		}
		private CRetainable _isDoingBlockingAction;

		
		private bool _applicationIsQuitting;

		
		#endregion <<---------- Properties ---------->>


		#region <<---------- Events ---------->>
		
		public static event Action<bool> OnPlayCutscene {
			add {
				Instance._onPlayCutscene -= value;
				Instance._onPlayCutscene += value;
			}
			remove {
				Instance._onPlayCutscene -= value;
			}
		}
		private event Action<bool> _onPlayCutscene;
		
		public static event Action<bool> OnMenu {
			add {
				Instance._onMenu -= value;
				Instance._onMenu += value;
			}
			remove {
				Instance._onMenu -= value;
			}
		}
		private event Action<bool> _onMenu;
		
		public static event Action<bool> OnDoingBlockingAction {
			add {
				Instance._onDoingBlockingAction -= value;
				Instance._onDoingBlockingAction += value;
			}
			remove {
				Instance._onDoingBlockingAction -= value;
			}
		}
		private event Action<bool> _onDoingBlockingAction;

		
		
		public static event Action<bool> OnAnyBlockingEventHappening {
			add {
				Instance._onAnyBlockingEventHappening -= value;
				Instance._onAnyBlockingEventHappening += value;
			}
			remove {
				Instance._onAnyBlockingEventHappening -= value;
			}
		}
		private event Action<bool> _onAnyBlockingEventHappening;
		
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
				CTime.SetTimeScale(onMenu ? 0f : 1f);
				Debug.Log($"IsOnMenuEvent: {onMenu}");
				this._onMenu?.Invoke(onMenu);
			});
			
			// playing cutscene
			this._isPlayingCutsceneRx?.Dispose();
			this._isPlayingCutsceneRx = new BoolReactiveProperty();
			this._isPlayingCutsceneRx.Subscribe(isPlayingCutscene => {
				Debug.Log($"IsPlayingCutscene: {isPlayingCutscene}");
				this._onPlayCutscene?.Invoke(isPlayingCutscene);
			});

			// is doing blocking action
			this._isDoingBlockingAction = new CRetainable();
			
			
			
			// blocking Event Happening
			Observable.CombineLatest(
						  this._isOnMenuRx, this._isPlayingCutsceneRx, this._isDoingBlockingAction.IsRetainedRx,
						  (isOnMenu, isPlayingCutscene, isDoingBlockingAction)
								  => isOnMenu || isPlayingCutscene || isDoingBlockingAction)
					  .Subscribe(blockingEventHappening => {
						  Debug.Log($"IsBlockingEventHappening changed to: {blockingEventHappening}");
						  this._isAnyBlockingEventHappening = blockingEventHappening;
						  this._onAnyBlockingEventHappening?.Invoke(blockingEventHappening);
					  });
		}

		#endregion <<---------- Initializers ---------->>

	}
}
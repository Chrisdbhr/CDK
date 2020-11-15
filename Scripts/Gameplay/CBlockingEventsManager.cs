using UniRx;
using UnityEngine;

namespace CDK {
	public class CBlockingEventsManager {
		
		#region <<---------- Singleton ---------->>

		private static CBlockingEventsManager _instance;
		
		#endregion <<---------- Singleton ---------->>

		
		

		#region <<---------- Properties ---------->>

		public static bool IsBlockingEventHappening { get { return (_instance ?? new CBlockingEventsManager())._isBlockingEventHappening; } }
		private bool _isBlockingEventHappening;
		
		public static bool IsPlayingCutscene {
			get { return (_instance ?? new CBlockingEventsManager()).IsPlayingCutsceneRx.Value; }
			set {
				if (_applicationIsQuitting) return;
				(_instance ?? new CBlockingEventsManager()).IsPlayingCutsceneRx.Value = value;
			}
		}

		public static bool IsOnMenu {
			get { return (_instance ?? new CBlockingEventsManager()).IsOnMenuRx.Value; }
			set {
				if (_applicationIsQuitting) return;
				(_instance ?? new CBlockingEventsManager()).IsOnMenuRx.Value = value;
			}
		}
		
		private ReactiveProperty<bool> IsOnMenuRx;
		private ReactiveProperty<bool> IsPlayingCutsceneRx;

		private static bool _applicationIsQuitting;
		
		#endregion <<---------- Properties ---------->>


		

		#region <<---------- Initializers ---------->>
		
		private CBlockingEventsManager() {
			Debug.Log("Creating BlockingEventsManager instance.");
			_instance = this;

			// app is quitting
			_applicationIsQuitting = false;
			Application.quitting += () => {
				_applicationIsQuitting = true;
				Debug.Log("Application is quitting...");
			};
			
			// rx
			this.IsOnMenuRx?.Dispose();
			this.IsOnMenuRx = new ReactiveProperty<bool>();
			this.IsPlayingCutsceneRx?.Dispose();
			this.IsPlayingCutsceneRx = new ReactiveProperty<bool>();
			
			// cursor menu rx visibility
			this.IsOnMenuRx.Subscribe(onMenu => {
				Cursor.visible = onMenu;
				Cursor.lockState = onMenu ? Cursor.lockState = CursorLockMode.None : CursorLockMode.Locked;
			});

			// playing cutscene
			this.IsPlayingCutsceneRx.Subscribe(isPlayingCutscene => {
				Debug.Log($"[BlockingEventsManager] IsPlayingCutscene: {isPlayingCutscene}");
			});

			// blocking Event Happening
			Observable.CombineLatest(
				this.IsOnMenuRx, this.IsPlayingCutsceneRx, 
				(isOnMenu, isPlayingCutscene) => isOnMenu || isPlayingCutscene).Subscribe(blockingEventHappening => {
				this._isBlockingEventHappening = blockingEventHappening;
			});
		}

		#endregion <<---------- Initializers ---------->>

	}
}
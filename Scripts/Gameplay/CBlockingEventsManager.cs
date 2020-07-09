using System;
using UniRx;
using UnityEngine;

namespace CDK {
	public class CBlockingEventsManager {
		
		#region <<---------- Singleton ---------->>
		public static CBlockingEventsManager get {
			get {
				if (_instance == null) {
					_instance = new CBlockingEventsManager();
				}
				return _instance;
			}
		}
		private static CBlockingEventsManager _instance;
		#endregion <<---------- Singleton ---------->>
		
		
		
		
		public CBlockingEventsManager() {
			// blocking events happening
			Observable.CombineLatest(
						  this.IsOnMenu,
						  this.IsPlayingCutscene,
						  (onMenu, isPlayingCutscene)
								  =>
								  (onMenu || isPlayingCutscene))
					  .Subscribe(blockingEventHappening => {
						  this._isBlockingEventHappening = blockingEventHappening;
						  Time.timeScale = blockingEventHappening ? 0f : 1f;
					  });
			
			// on menu
			this.IsOnMenu.Subscribe(onMenu => {
				if (onMenu) {
					Cursor.visible = true;
					Cursor.lockState = CursorLockMode.None;
				}
				else if (CGameSettings.get.HideCursorOnGame) {
					Cursor.visible = false;
					Cursor.lockState = CursorLockMode.Locked;
				}
				
			});
		}
		
		public ReactiveProperty<bool> IsOnMenu = new ReactiveProperty<bool>();
		public ReactiveProperty<bool> IsPlayingCutscene = new ReactiveProperty<bool>();


		public bool IsBlockingEventHappening {
			get { return this._isBlockingEventHappening; }
		}
		[NonSerialized] private bool _isBlockingEventHappening;
		
	}
}

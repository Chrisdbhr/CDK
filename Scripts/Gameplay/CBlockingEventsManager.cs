using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CDK {
	public class CBlockingEventsManager {
		
		#region <<---------- Properties ---------->>

		public bool IsAnyBlockingEventHappening {
			get { return this._isAnyBlockingEventHappening; }
		}
		private bool _isAnyBlockingEventHappening;

		
		public bool IsPlayingCutscene {
			get { return this._isPlayingCutsceneRx.Value; }
			set { this._isPlayingCutsceneRx.Value = value; }
		}
		private BoolReactiveProperty _isPlayingCutsceneRx;


		public bool IsOnMenu {
			get { return this._isOnMenuRx.Value; }
			set { this._isOnMenuRx.Value = value; }
		}
		private BoolReactiveProperty _isOnMenuRx;

		public CRetainable IsDoingBlockingAction {
			get { return this._isDoingBlockingAction; }
			set { this._isDoingBlockingAction = value; }
		}
		private CRetainable _isDoingBlockingAction;

		private HashSet<UnityEngine.Object> RetainedObjects;
		
		#endregion <<---------- Properties ---------->>

		
		

		#region <<---------- Events ---------->>
		
		public event Action<bool> OnPlayCutscene {
			add {
				this._onPlayCutscene -= value;
				this._onPlayCutscene += value;
			}
			remove {
				this._onPlayCutscene -= value;
			}
		}
		private event Action<bool> _onPlayCutscene;
		
		public event Action<bool> OnMenu {
			add {
				this._onMenu -= value;
				this._onMenu += value;
			}
			remove {
				this._onMenu -= value;
			}
		}
		private event Action<bool> _onMenu;
		
		public event Action<bool> OnDoingBlockingAction {
			add {
				this._onDoingBlockingAction -= value;
				this._onDoingBlockingAction += value;
			}
			remove {
				this._onDoingBlockingAction -= value;
			}
		}
		private event Action<bool> _onDoingBlockingAction;
		
		public event Action<bool> OnAnyBlockingEventHappening {
			add {
				this._onAnyBlockingEventHappening -= value;
				this._onAnyBlockingEventHappening += value;
			}
			remove {
				this._onAnyBlockingEventHappening -= value;
			}
		}
		private event Action<bool> _onAnyBlockingEventHappening;
		
		#endregion <<---------- Events ---------->>

		
		

		#region <<---------- Initializers ---------->>
		
		public CBlockingEventsManager() {

			this.RetainedObjects = new HashSet<Object>();
			
			// on menu
			this._isOnMenuRx?.Dispose();
			this._isOnMenuRx = new BoolReactiveProperty();
			this._isOnMenuRx.Subscribe(onMenu => {
				CTime.TimeScale = (onMenu ? 0f : 1f);
				Debug.Log($"<color={"#4fafb6"}>IsOnMenuEvent: {onMenu}</color>");
				this._onMenu?.Invoke(onMenu);
			});
			
			// playing cutscene
			this._isPlayingCutsceneRx?.Dispose();
			this._isPlayingCutsceneRx = new BoolReactiveProperty();
			this._isPlayingCutsceneRx.Subscribe(isPlayingCutscene => {
				Debug.Log($"<color={"#cc5636"}>IsPlayingCutscene: {isPlayingCutscene}</color>");
				this._onPlayCutscene?.Invoke(isPlayingCutscene);
			});

			// is doing blocking action
			this._isDoingBlockingAction = new CRetainable();
			
			// blocking Event Happening
			Observable.CombineLatest(
			this._isOnMenuRx, 
			this._isPlayingCutsceneRx,
			this._isDoingBlockingAction.IsRetainedRx,
			(isOnMenu, isPlayingCutscene, isDoingBlockingAction) => isOnMenu || isPlayingCutscene || isDoingBlockingAction)
			.Subscribe(blockingEventHappening => {
				Debug.Log($"<color={"#b62a24"}>IsBlockingEventHappening changed to: {blockingEventHappening}</color>");
				this._isAnyBlockingEventHappening = blockingEventHappening;
				this._onAnyBlockingEventHappening?.Invoke(blockingEventHappening);
			});
		}

		#endregion <<---------- Initializers ---------->>
		
		
		
		
		#region <<---------- Game Object Retain ---------->>
		
		public void RetainFromUnityObject(UnityEngine.Object unityObject) {
			if (unityObject == null) {
				Debug.LogError($"Will not retain a null object.");
				return;
			}
			if (this.RetainedObjects.Add(unityObject)) {
				Debug.Log($"Retaining BlockingEvents from '{unityObject.name}'");
				this.IsDoingBlockingAction.Retain();
				return;
			}
			Debug.LogError($"GameObject '{unityObject.name}' tried to Retain BlockingEvents when already retaining!", unityObject);
		}

		public void ReleaseFromUnityObject(UnityEngine.Object unityObject) {
			if (unityObject == null) {
				Debug.LogError($"Tried to release a null object.");
				return;
			}

			if (this.RetainedObjects.Remove(unityObject)) {
				Debug.Log($"Releasing BlockingEvents from '{unityObject.name}'");
				this.IsDoingBlockingAction.Release();
				return;
			}
			Debug.LogWarning($"GameObject '{unityObject.name}' tried to Release BlockingEvents when not on list of retained objects. This can lead to unpredicable behaviour.", unityObject);
		}
		
		#endregion <<---------- Game Object Retain ---------->>

	}
}
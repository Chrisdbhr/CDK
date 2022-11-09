using System;
using System.Collections;
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

		public bool IsDoingBlockingAction => this._isDoingBlockingAction.IsRetained();
		private CRetainable _isDoingBlockingAction;

        public IEnumerable RetainedObjects => this._retainedObjects;
		private HashSet<UnityEngine.Object> _retainedObjects;

        private CompositeDisposable _disposables;
        
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

            if (!Application.isPlaying) return;
            
            this._disposables?.Dispose();
            this._disposables = new CompositeDisposable();

			this._retainedObjects = new HashSet<Object>();
			
			// on menu
			this._isOnMenuRx?.Dispose();
			this._isOnMenuRx = new BoolReactiveProperty();
			this._isOnMenuRx.Subscribe(onMenu => {
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
            this._isDoingBlockingAction.IsRetainedRx.Subscribe(retained => {
                this._onDoingBlockingAction?.Invoke(retained);
            });
            
			// blocking Event Happening
			Observable.CombineLatest(
			this._isOnMenuRx.AsObservable(), 
			this._isPlayingCutsceneRx.AsObservable(),
			this._isDoingBlockingAction.IsRetainedRx.AsObservable(),
			(isOnMenu, isPlayingCutscene, isDoingBlockingAction) => isOnMenu || isPlayingCutscene || isDoingBlockingAction)
			.Subscribe(blockingEventHappening => {
				Debug.Log($"<color={"#b62a24"}>IsBlockingEventHappening changed to: {blockingEventHappening}</color>");
				this._isAnyBlockingEventHappening = blockingEventHappening;
				this._onAnyBlockingEventHappening?.Invoke(blockingEventHappening);
			})
            .AddTo(this._disposables);
            
            // check for null retainers
            Observable.EveryUpdate().Subscribe(_ => {
                bool anyWasNull = false;
                foreach (var o in this._retainedObjects) {
                    if (o != null) continue;
                    anyWasNull = true;
                    Debug.LogWarning("Releasing one Retainable because one object inside list was null!");
                    this._isDoingBlockingAction.Release();
                }

                if (!anyWasNull) return;

                int nullObjs = this._retainedObjects.RemoveWhere(i => i == null);
                if (nullObjs > 0) {
                    Debug.LogWarning($"Removed {nullObjs} null objects from Retainable List.");
                }
            })
            .AddTo(this._disposables);
		}

		#endregion <<---------- Initializers ---------->>
		
		
		
		
		#region <<---------- Game Object Retain ---------->>
		
		public void RetainFromUnityObject(UnityEngine.Object unityObject) {
			if (unityObject == null) {
				Debug.LogError($"Will not retain a null object.");
				return;
			}
			if (this._retainedObjects.Add(unityObject)) {
				Debug.Log($"Retaining BlockingEvents from '{unityObject.name}'");
				this._isDoingBlockingAction.Retain();
				return;
			}
			Debug.LogError($"GameObject '{unityObject.name}' tried to Retain BlockingEvents when already retaining!", unityObject);
		}

		public void ReleaseFromUnityObject(UnityEngine.Object unityObject) {
			if (unityObject == null) {
				Debug.LogError($"Tried to release a null object.");
				return;
			}

			if (this._retainedObjects.Remove(unityObject)) {
				Debug.Log($"Releasing BlockingEvents from '{unityObject.name}'");
                this._isDoingBlockingAction.Release();
				return;
			}
			Debug.LogWarning($"GameObject '{unityObject.name}' tried to Release BlockingEvents when not on list of retained objects. This can lead to unpredicable behaviour.", unityObject);
		}
		
		#endregion <<---------- Game Object Retain ---------->>

	}
}
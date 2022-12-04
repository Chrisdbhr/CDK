using System;
using UnityEngine;

namespace CDK.Interaction {
	[DisallowMultipleComponent]
	public abstract class CPlayerInteractionBase : MonoBehaviour {
        [SerializeField] protected bool _debug;

		[SerializeField] protected LayerMask _interactionLayerMask = 1;
		protected CBlockingEventsManager _blockingEventsManager;

        protected ICInteractable TargetInteractable {
            get { return this._targetInteractable; }
            set {
                var targetChanged = (value != this._targetInteractable); 
                if (targetChanged) this._targetInteractable?.OnStoppedBeingInteractionTarget(this.transform);
                this._targetInteractable = value;
                this._targetInteractable?.OnBecameInteractionTarget(this.transform);
                if (targetChanged) this._interactTargetChanged?.Invoke(value);
            }
        }
        private ICInteractable _targetInteractable;

        public event Action<ICInteractable> InteractTargetChanged {
            add {
                this._interactTargetChanged -= value;
                this._interactTargetChanged += value;
            }
            remove {
                this._interactTargetChanged -= value;
            }
        }
        private Action<ICInteractable> _interactTargetChanged;
        
		protected virtual void Awake() {
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
		}

		public abstract void TryToInteract();
		
	}
}

using System;
using UnityEngine;

namespace CDK {
	[DisallowMultipleComponent]
	public abstract class CPlayerInteractionBase : MonoBehaviour {
		
		[NonSerialized] protected LayerMask _interactionLayerMask = 1;
		[NonSerialized] protected Transform _myTransform;
		[NonSerialized] protected CBlockingEventsManager _blockingEventsManager;

		protected virtual void Awake() {
			this._myTransform = this.transform;
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
		}

		public abstract void TryToInteract();
		
		
	}
}

using System;
using UnityEngine;

namespace CDK {
	[DisallowMultipleComponent]
	public abstract class CPlayerInteractionBase : MonoBehaviour {
		
		[NonSerialized] protected LayerMask _interactionLayerMask = 1;
		[NonSerialized] protected CBlockingEventsManager _blockingEventsManager;

		protected virtual void Awake() {
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
		}

		public abstract void TryToInteract();
		
		
	}
}

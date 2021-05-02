using System;
using UnityEngine;

namespace CDK {
	public abstract class CPlayerInteractionBase : MonoBehaviour {
		
		[NonSerialized] protected LayerMask _interactionLayerMask = 1;
		[NonSerialized] protected Transform _myTransform;
		
		protected virtual void Awake() {
			this._myTransform = this.transform;
		}

		public abstract void TryToInteract();
		
		
	}
}

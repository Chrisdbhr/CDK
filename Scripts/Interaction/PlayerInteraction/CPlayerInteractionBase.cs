using System;
using UnityEngine;

namespace CDK {
	public abstract class CPlayerInteractionBase : MonoBehaviour {
		
		[SerializeField] protected LayerMask _interactionLayerMask = 1;
		[NonSerialized] protected Transform _myTransform;
		
		protected virtual void Awake() {
			this._myTransform = this.transform;
			
		}

		protected void Update() {
			// input interaction
			bool inputDownInteract = Input.GetButtonDown(CInputKeys.INTERACT);
			if (inputDownInteract) {
				this.TryToInteract();
			}
		}

		protected abstract void TryToInteract();
		
		
	}
}

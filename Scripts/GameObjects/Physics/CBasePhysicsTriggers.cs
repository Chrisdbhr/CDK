using System;
using UnityEngine;

namespace CDK {
	public abstract class CBasePhysicsTriggers : MonoBehaviour {
	
		[SerializeField] protected bool _isTrigger = true;
		[SerializeField] [TagSelector] private string _tag = "Player";
		
		[SerializeField] protected CUnityEventTransform Enter;
		[SerializeField] protected CUnityEventTransform Exit;
		[Space]
		[SerializeField] protected CUnityEventBool Entered;
		[SerializeField] protected CUnityEventBool Exited;

		
		
		
		protected bool WillIgnoreTrigger(Component col) {
			return !this._tag.CIsNullOrEmpty() && !col.CompareTag(this._tag);
		}

        protected void Reset() {
            var col = this.GetComponent<Collider>();
            if (col) col.isTrigger = this._isTrigger;
        }
    }
}
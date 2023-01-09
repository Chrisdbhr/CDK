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



        protected virtual void Awake() { }

        protected virtual void Reset() {
            if (this.TryGetComponent<Collider>(out var c)) {
                this._isTrigger = c.isTrigger;
            }
        }

        protected virtual bool WillIgnoreTrigger(Component col) {
			return !this._tag.CIsNullOrEmpty() && !col.CompareTag(this._tag);
		}

        private void OnValidate() {
            if (this.TryGetComponent<Collider>(out var c)) {
                c.isTrigger = this._isTrigger;
            }
        }
    }
}
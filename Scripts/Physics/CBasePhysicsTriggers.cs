using System;
using UnityEngine;

namespace CDK {
	public abstract class CBasePhysicsTriggers : MonoBehaviour {
	
		[SerializeField] [CTagSelector] protected string _tag = "Player";

        protected bool TriggerOnce => this._triggerOnce;
        [SerializeField] private bool _triggerOnce;
        protected bool _triggered;
        protected bool CannotTrigger => this.TriggerOnce && this._triggered;
        
		[SerializeField] protected CUnityEventTransform Enter;
		[SerializeField] protected CUnityEventTransform Exit;
		[Space]
		[SerializeField] protected CUnityEventBool Entered;
		[SerializeField] protected CUnityEventBool Exited;
        protected Collider _collider;



        protected virtual void Awake() {
            this._collider = this.GetComponent<Collider>();
        }

        protected virtual void Reset() { }

        protected virtual bool WillIgnoreTrigger(Component col) {
			return !this._tag.CIsNullOrEmpty() && !col.CompareTag(this._tag);
		}
        
    }
}
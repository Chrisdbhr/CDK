using System;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace CDK {
	public abstract class CBasePhysicsTriggers : MonoBehaviour {
	
		[SerializeField] [CTagSelector] protected string _tag = "Player";

        protected bool TriggerOnce => this._triggerOnce;
        [SerializeField] private bool _triggerOnce;
        protected bool _triggered;
        protected bool CannotTrigger => this.TriggerOnce && this._triggered;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Transform Events")]
        #endif
		[SerializeField] protected CUnityEventTransform Enter;
        #if ODIN_INSPECTOR
        [FoldoutGroup("Transform Events")]
        #endif
        [SerializeField] protected CUnityEventTransform Exit;
		[Space]
        #if ODIN_INSPECTOR
        [FoldoutGroup("Transform Booleans")]
        #endif
		[SerializeField] protected CUnityEventBool Entered;
        #if ODIN_INSPECTOR
        [FoldoutGroup("Transform Booleans")]
        #endif
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
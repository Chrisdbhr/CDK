using System;
using UnityEngine;

namespace CDK {
    [RequireComponent(typeof(Collider))]
    public class CSmashableObject : MonoBehaviour, CICanBeSmashedWhenStepping {

        #region <<---------- Properties and Fields ---------->>

        [SerializeField] private CUnityEvent _getSmashedEvent;
        
        public event Action<Transform, CFootstepsSource.FootstepFeet> OnGetSmashed {
            add {
                this._onGetSmashed -= value;
                this._onGetSmashed += value;
            }
            remove {
                this._onGetSmashed -= value;
            }
        }
        private Action<Transform, CFootstepsSource.FootstepFeet> _onGetSmashed;
        
        #endregion <<---------- Properties and Fields ---------->>
        
        
        
        
        #region <<---------- CICanBeSmashedWhenStepping ---------->>

        public virtual void Smash(Transform smashingTransform, CFootstepsSource.FootstepFeet feet) {
            this._getSmashedEvent?.Invoke();
            this._onGetSmashed?.Invoke(smashingTransform, feet);
        }
        
        #endregion <<---------- CICanBeSmashedWhenStepping ---------->>


    }
}
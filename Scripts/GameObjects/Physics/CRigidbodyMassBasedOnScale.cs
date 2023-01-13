using System;
using UnityEngine;

namespace CDK {
    [RequireComponent(typeof(Rigidbody))]
    public class CRigidbodyMassBasedOnScale : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>

        [SerializeField, Range(0.0001f, 1000000f)] private float _baseMass = 10f;
        [SerializeField] private bool _useWorldScale = true;

        #endregion <<---------- Properties and Fields ---------->>



        
        #region <<---------- Mono Behaviour ---------->>

        private void Awake() {
            this.AdjustMass();
        }

        private void OnValidate() {
            this.AdjustMass();
        }

        #endregion <<---------- Mono Behaviour ---------->>


        
        
        #region <<---------- General ---------->>

        void AdjustMass() {
            if(this.TryGetComponent<Rigidbody>(out var rb)) {
                rb.mass = GetMass();
            }
        }

        float GetMass() {
            return this._baseMass * (this._useWorldScale ? this.transform.lossyScale : this.transform.localScale).magnitude;
        }

        #endregion <<---------- General ---------->>
        
    }

}
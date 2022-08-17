using UnityEngine;

namespace CDK {
    public class CRandomRotation : CAutoTriggerCompBase {

        [SerializeField] private Vector3 _rotationMin;
        [SerializeField] private Vector3 _rotationMax = Vector3.one * 360f;
        [SerializeField] private Space _relativeTo;
    
        
        
        protected override void TriggerEvent() {
            this.transform.Rotate(
                Random.Range(this._rotationMin.x, this._rotationMax.x),
                Random.Range(this._rotationMin.y, this._rotationMax.y),
                Random.Range(this._rotationMin.z, this._rotationMax.z),
                this._relativeTo
            );
        }
    }
}
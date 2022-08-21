using UnityEngine;

namespace CDK {
    public class CSetLayerOnAwake : MonoBehaviour {

        [SerializeField] private LayerMask _layer = -1;
        
        
        public void Awake() {
            this.gameObject.CSetLayerFromLayerMask(this._layer);
            Debug.Log($"Setting '{this.name}' to '{this.gameObject.layer.ToString()}'");
        }
    }
}
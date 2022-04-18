using UnityEngine;
using UnityEngine.Events;

namespace CDK {
    public class CSystemNotSupportedTriggers : MonoBehaviour {
        
        [Header("Event will be invoked if feature is not supported")]
        [Space]
        [SerializeField] private UnityEvent _tessellationShaders;
        [SerializeField] private UnityEvent _computeShaders;
        [SerializeField] private UnityEvent _geometryShaders;
        
        
        
        
        private void Awake() {
            if(!SystemInfo.supportsTessellationShaders) this._tessellationShaders?.Invoke();
            if(!SystemInfo.supportsGeometryShaders) this._geometryShaders?.Invoke();
            if(!SystemInfo.supportsComputeShaders) this._computeShaders?.Invoke();
        }
    }
}
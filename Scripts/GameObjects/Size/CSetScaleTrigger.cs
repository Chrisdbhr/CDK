using UnityEngine;

namespace CDK.GameObjects {
    public class CSetScaleTrigger : MonoBehaviour {

        [SerializeField] private Vector3 _targetLocalScale = Vector3.one;


        
        
        public void ResetScale() {
            transform.localScale = _targetLocalScale;
        }
        
        public void SetScale(Vector3 newScale) {
            transform.localScale = newScale;
        }
        
    }
}
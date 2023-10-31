using UnityEngine;

namespace CDK {
    public class CDisableOnEditor : MonoBehaviour {
        #if UNITY_EDITOR
        private void Awake() {
            Debug.Log($"Disabling {this.name} on Editor only.", this);
            this.gameObject.SetActive(false);
        }
        #endif
    }
}
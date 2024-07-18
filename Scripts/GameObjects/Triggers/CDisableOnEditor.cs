using UnityEngine;

namespace CDK {
    public class CDisableOnEditor : MonoBehaviour {
        #if UNITY_EDITOR
        private void Awake() {
            Debug.Log($"<b>Editor only</b>: Disabling {this.name}", this);
            this.gameObject.SetActive(false);
        }
        #endif
    }
}
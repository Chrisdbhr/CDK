using UnityEngine;

namespace CDK {
    public class CUnparentWhenNotInEditor : MonoBehaviour {
        private void Awake() {
            if (Application.isEditor) return;
            this.transform.DetachChildren();
            Debug.Log($"Detachin Children from {this.name} ");
        }
    }
}
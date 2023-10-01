using UnityEngine;

namespace CDK {
    public class CUnparentOnAwake : MonoBehaviour {
        protected virtual void Awake() {
            Unparent();
        }

        void Unparent() {
            transform.SetAsLastSibling();
            transform.CUnparentAllChildren();
        }
    }
}
using UnityEngine;

namespace CDK {
    public class CUnparentChildrenOnAwake : MonoBehaviour {
        protected virtual void Awake() {
            Unparent();
        }

        void Unparent() {
            transform.SetAsLastSibling();
            transform.CUnparentAllChildren();
        }
    }
}
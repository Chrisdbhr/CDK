using UnityEngine;

namespace CDK {
    public class CUnparentChildrenWhenNotInEditor : CUnparentChildrenOnAwake {
        protected override void Awake() {
            if (Application.isEditor) return;
            base.Awake();
        }
    }
}
using UnityEngine;

namespace CDK {
    public class CUnparentChildrenWhenNotInEditor : CUnparentOnAwake {
        protected override void Awake() {
            if (Application.isEditor) return;
            base.Awake();
        }
    }
}
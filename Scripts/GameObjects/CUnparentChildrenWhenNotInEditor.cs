using UnityEngine;

namespace CDK {
    public class CUnparentChildrenWhenNotInEditor : CAutoTriggerCompBase {

        //[SerializeField] private bool _runOnEditor;
        
        protected override void TriggerEvent() {
            if (Application.isEditor) return;
            new GameObject($"----- {this.name}").transform.SetAsLastSibling();
            this.transform.CUnparentAllChildren();
        }
        
    }
}
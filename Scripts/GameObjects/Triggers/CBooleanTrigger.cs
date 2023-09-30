using UnityEngine;
using UnityEngine.Events;

namespace CDK {
    public class CBooleanTrigger : MonoBehaviour {

        public bool Value;
        [SerializeField] private UnityEvent _onValueTrue;
        [SerializeField] private UnityEvent _onValueFalse;
        [SerializeField] private CUnityEventBool _onAnyValue;




        public void TriggerValue(bool value) {
            this.Value = value;
            if(value) this._onValueTrue?.Invoke();
            else this._onValueFalse?.Invoke();
            this._onAnyValue?.Invoke(value);
        }

    }
}
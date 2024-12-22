using UnityEngine;
using UnityEngine.Events;

namespace CDK {
    public class CBooleanTrigger : MonoBehaviour {

        public bool Value;
        [SerializeField] UnityEvent _onValueTrue;
        [SerializeField] UnityEvent _onValueFalse;
        [SerializeField] CUnityEventBool _onAnyValue;




        public void TriggerValue(bool value) {
            Value = value;
            if(value) _onValueTrue?.Invoke();
            else _onValueFalse?.Invoke();
            _onAnyValue?.Invoke(value);
        }

    }
}
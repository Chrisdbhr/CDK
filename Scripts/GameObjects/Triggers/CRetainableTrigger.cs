using UnityEngine;
using UnityEngine.Events;

namespace CDK
{
    public class CRetainableTrigger : MonoBehaviour
    {
        public CRetainable Retainable { get; } = new ();

        [SerializeField] CUnityEventBool _retainedStateEvent;
        [SerializeField] CUnityEventBool _retainedStateInverseEvent;
        [SerializeField] UnityEvent _isRetainedEvent;
        [SerializeField] UnityEvent _isNotRetainedEvent;


        void OnEnable()
        {
            Retainable.StateEvent += RetainableOnStateEvent;
        }

        void OnDisable()
        {
            Retainable.StateEvent -= RetainableOnStateEvent;
        }

        void RetainableOnStateEvent(bool isRetained)
        {
            _retainedStateEvent?.Invoke(isRetained);
            _retainedStateInverseEvent?.Invoke(!isRetained);
            if (isRetained)
            {
                _isRetainedEvent?.Invoke();
            }
            else
            {
                _isNotRetainedEvent?.Invoke();
            }
        }
    }
}
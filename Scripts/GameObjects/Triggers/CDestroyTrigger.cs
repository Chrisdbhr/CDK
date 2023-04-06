using UnityEngine;
using UnityEngine.Events;

namespace CDK {
    public class CDestroyTrigger : MonoBehaviour {

        [SerializeField] private UnityEvent DestroyEvent;
        
        
        public void DestroyGameObject(GameObject go) {
            go.CDestroy();
            DestroyEvent?.Invoke();
        }
        
    }
}
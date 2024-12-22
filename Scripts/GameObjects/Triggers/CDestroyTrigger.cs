using UnityEngine;
using UnityEngine.Events;

namespace CDK {
    public class CDestroyTrigger : MonoBehaviour {

        [SerializeField] UnityEvent DestroyEvent;
        
        
        public void DestroyGameObject(GameObject go) {
            go.CDestroy();
            DestroyEvent?.Invoke();
        }
        
    }
}
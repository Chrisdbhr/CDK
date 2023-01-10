using UnityEngine;

namespace CDK {
    public class CDestroyOnContact : CPhysicsTrigger {

        [SerializeField] private GameObject[] _otherGameObjectsToDestroy;
        
        
        protected override void StartedCollisionOrTrigger(Transform other) {
            base.StartedCollisionOrTrigger(other);
            foreach (var o in _otherGameObjectsToDestroy) {
                o.CDestroy();
            }
            this.gameObject.CDestroy();
        }
    }
}
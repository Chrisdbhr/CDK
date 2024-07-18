using System;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
    public class CSpawnChildrenOnAwake : MonoBehaviour {
        
        [SerializeField] private int _amount = 10;
        [SerializeField] private GameObject _templateGameObject;
        [SerializeField] private UnityEvent _finishedSpawning;
        
        
        

        private void Awake() {
            if (!this._templateGameObject) return;
            for (int i = 0; i < _amount; i++) {
                this.CInstantiate(this._templateGameObject, this.transform);
            }
            this._finishedSpawning?.Invoke();
        }
        
    }
}
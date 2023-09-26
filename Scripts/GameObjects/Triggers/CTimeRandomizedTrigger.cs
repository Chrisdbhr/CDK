using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace CDK {
    public class CTimeRandomizedTrigger : MonoBehaviour {

        [SerializeField, Min(0f)] private float _firstMinTime = 5f;
        [SerializeField, Min(0f)] private float _firstMaxTime = 15f;
        [SerializeField, Min(0f)] private float _minTime = 1f;
        [SerializeField, Min(0f)] private float _maxTime = 5f;
        [SerializeField] private UnityEvent _triggerEvent;
        private bool _randomizing;




        private void OnEnable() {
            this._randomizing = true;
        }

        IEnumerator Start() {
            yield return new WaitForSeconds(Random.Range(this._firstMinTime, this._firstMaxTime));
            while (this._randomizing) {
                yield return new WaitForSeconds(Random.Range(this._minTime, this._maxTime));
                this._triggerEvent?.Invoke();
            }
        }

        private void OnDisable() {
            this._randomizing = false;
        }

    }
}
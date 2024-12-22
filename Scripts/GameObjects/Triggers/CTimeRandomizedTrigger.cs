using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace CDK {
    public class CTimeRandomizedTrigger : MonoBehaviour {

        [SerializeField, Min(0f)] float _firstMinTime = 5f;
        [SerializeField, Min(0f)] float _firstMaxTime = 15f;
        [SerializeField, Min(0f)] float _minTime = 1f;
        [SerializeField, Min(0f)] float _maxTime = 5f;
        [SerializeField] UnityEvent _triggerEvent;
        bool _randomizing;


        void OnEnable() {
            _randomizing = true;
        }

        IEnumerator Start() {
            yield return new WaitForSeconds(Random.Range(_firstMinTime, _firstMaxTime));
            while (_randomizing) {
                yield return new WaitForSeconds(Random.Range(_minTime, _maxTime));
                _triggerEvent?.Invoke();
            }
        }

        void OnDisable() {
            _randomizing = false;
        }

    }
}
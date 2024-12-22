using System;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
    public class CCriticalHitFeedbackTrigger : MonoBehaviour {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void ResetListeners() {
            OnCriticalHit = null;
        }

        [SerializeField] UnityEvent _onCriticalHit;
        public static event EventHandler<CHealthComponent> OnCriticalHit = delegate { };

        void OnEnable() {
            OnCriticalHit += CriticalHitCallback;
        }

        void CriticalHitCallback(object sender, CHealthComponent healthHit) {
            if (healthHit.CompareTag("Player")) return;
            _onCriticalHit?.Invoke();
        }

        void OnDisable() {
            OnCriticalHit -= CriticalHitCallback;
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
    public class CTimedAutoTrigger : CAutoTriggerCompBase {

        [SerializeField, Min(0f)] private float _secondsToTrigger = 5f;
        [SerializeField] private UnityEvent _onTriggerEvent;




        protected override void TriggerEvent() {
            this.CStartCoroutine(this.TriggerRoutine());
        }

        private IEnumerator TriggerRoutine() {
            yield return new WaitForSeconds(this._secondsToTrigger);
            this._onTriggerEvent?.Invoke();
        }

    }
}
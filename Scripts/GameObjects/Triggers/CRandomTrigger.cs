using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CRandomTrigger : CAutoTriggerCompBase {

        #region <<---------- Properties and Fields ---------->>

        public float ChanceToTrigger => _chanceToTrigger;
        [SerializeField, Tooltip("Chance in % to trigger event"), Range(1f, 99f)]
        float _chanceToTrigger = 50f;
        [SerializeField] protected bool _triggerValue;
        [SerializeField] bool _triggerAffectsSelfActiveState = true;
        [SerializeField] CUnityEventBool _randomResultEvent;
        [SerializeField] CUnityEventBool _randomResultInvertedEvent;
        [SerializeField] UnityEvent _randomPositiveEvent;
        [SerializeField] UnityEvent _randomNegativeEvent;

        #endregion <<---------- Properties and Fields ---------->>


        

        #region <<---------- CAutoTriggerCompBase ---------->>

        protected override void TriggerEvent() {
            _triggerValue = GetRandomResult(GetChanceToTrigger());

            if (_triggerAffectsSelfActiveState) {
                gameObject.SetActive(_triggerValue);
            }
			
            _randomResultEvent?.Invoke(_triggerValue);
            _randomResultInvertedEvent?.Invoke(!_triggerValue);
            if (_triggerValue) {
                _randomPositiveEvent?.Invoke();
            }
            else {
                _randomNegativeEvent?.Invoke();
            }
        }

        #endregion <<---------- CAutoTriggerCompBase ---------->>

        


        #region <<---------- Random ---------->>

        protected virtual bool GetRandomResult(float chance) {
            return ((100f - chance) <= 100f * Random.value);
        }

        protected virtual float GetChanceToTrigger() {
            return _chanceToTrigger;
        }
        
        #endregion <<---------- Random ---------->>
        
    }
}

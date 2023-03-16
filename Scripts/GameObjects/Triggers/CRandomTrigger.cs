using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CRandomTrigger : CAutoTriggerCompBase {

        #region <<---------- Properties and Fields ---------->>

        public float ChanceToTrigger => this._chanceToTrigger;
        [SerializeField, Tooltip("Chance in % to trigger event"), Range(1f, 99f)] private float _chanceToTrigger = 50f;
        [SerializeField] protected bool _triggerValue;
        [SerializeField] private bool _triggerAffectsSelfActiveState = true;
        [SerializeField] private CUnityEventBool _randomResultEvent;
        [SerializeField] private CUnityEventBool _randomResultInvertedEvent;
        [SerializeField] private UnityEvent _randomPositiveEvent;
        [SerializeField] private UnityEvent _randomNegativeEvent;

        #endregion <<---------- Properties and Fields ---------->>


        

        #region <<---------- CAutoTriggerCompBase ---------->>

        protected override void TriggerEvent() {
            this._triggerValue = GetRandomResult(GetChance());

            if (this._triggerAffectsSelfActiveState) {
                this.gameObject.SetActive(this._triggerValue);
            }
			
            this._randomResultEvent?.Invoke(this._triggerValue);
            this._randomResultInvertedEvent?.Invoke(!this._triggerValue);
            if (this._triggerValue) {
                this._randomPositiveEvent?.Invoke();
            }
            else {
                this._randomNegativeEvent?.Invoke();
            }
        }

        #endregion <<---------- CAutoTriggerCompBase ---------->>

        


        #region <<---------- Random ---------->>

        protected virtual bool GetRandomResult(float chance) {
            return (chance <= this._chanceToTrigger);
        }

        protected virtual float GetChance() {
            return 100f * Random.value;
        }
        
        #endregion <<---------- Random ---------->>
        
    }
}

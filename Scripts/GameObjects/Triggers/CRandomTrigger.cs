using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CRandomTrigger : CAutoTriggerCompBase {
		
		[Tooltip("Chance in % to trigger event")]
		[SerializeField] [Range(1f, 99f)]private float _chanceToTrigger = 50f;
		[SerializeField] private bool _triggerAffectsSelfActiveState = true;

        [SerializeField] private CUnityEventBool _randomResultEvent;
        [SerializeField] private CUnityEventBool _randomResultInvertedEvent;
		[SerializeField] private UnityEvent _randomPositiveEvent;
		[SerializeField] private UnityEvent _randomNegativeEvent;
		
		
		protected override void TriggerEvent() {
			var chance = 100f * Random.value;
			bool willTrigger = (chance <= this._chanceToTrigger);

			if (this._triggerAffectsSelfActiveState) {
				this.gameObject.SetActive(willTrigger);
			}
			
            this._randomResultEvent?.Invoke(willTrigger);
            this._randomResultInvertedEvent?.Invoke(!willTrigger);
			if (willTrigger) {
				this._randomPositiveEvent?.Invoke();
			}
			else {
				this._randomNegativeEvent?.Invoke();
			}
		}
	}
}

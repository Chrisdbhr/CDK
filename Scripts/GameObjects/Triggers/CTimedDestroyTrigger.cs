using UnityEngine;

namespace CDK {
	public class CTimedDestroyTrigger : CAutoTriggerCompBase {
		[SerializeField][Range(0f, 600f)] private float _secondsToDestroy = 5f;
        [SerializeField] private bool _shouldLog = true;
		
		protected override void TriggerEvent() {
			this.gameObject.CDestroy(_shouldLog, this._secondsToDestroy);
		}
	}
}
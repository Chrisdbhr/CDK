using UnityEngine;

namespace CDK {
	public class CTimedDestroyTrigger : CAutoTriggerCompBase {
		[SerializeField][Range(0f, 600f)] private float _secondsToDestroy = 5f;
		
		
		protected override void TriggerEvent() {
			this.gameObject.CDestroy(this._secondsToDestroy);
		}
	}
}

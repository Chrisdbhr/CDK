using UnityEngine;

namespace CDK {
	public class CUnparentObjectTrigger : CAutoTriggerCompBase {
		protected override void TriggerEvent() {
			this.transform.SetParent(null, true);
			this.CDestroy();
		}
	}
}
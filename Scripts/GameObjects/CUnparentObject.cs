using UnityEngine;

namespace CDK {
	public class CUnparentObject : CAutoTriggerCompBase {
		protected override void TriggerEvent() {
			this.transform.parent = null;
			this.CDestroy();
		}
	}
}
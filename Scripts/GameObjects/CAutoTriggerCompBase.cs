using UnityEngine;

namespace CDK {
	public abstract class CAutoTriggerCompBase : MonoBehaviour {

		[SerializeField] private CMonobehaviourExecutionTime executionTime;

		private void Awake() {
			if (!this.enabled) return;
			if (this.executionTime != CMonobehaviourExecutionTime.Awake) return;
			this.TriggerEvent();
		}

		private void Start() {
			if (!this.enabled) return;
			if (this.executionTime != CMonobehaviourExecutionTime.Start) return;
			this.TriggerEvent();
		}

		private void OnEnable() {
			if (!this.enabled) return;
			if (this.executionTime != CMonobehaviourExecutionTime.OnEnable) return;
			this.TriggerEvent();
		}

		protected abstract void TriggerEvent();

	}
}
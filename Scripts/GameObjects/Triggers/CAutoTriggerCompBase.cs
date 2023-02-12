using UnityEngine;

namespace CDK {
	public abstract class CAutoTriggerCompBase : MonoBehaviour {

		[SerializeField] private CMonobehaviourExecutionTime executionTime = CMonobehaviourExecutionTime.OnEnable;

		protected virtual void Awake() {
			if (this.executionTime != CMonobehaviourExecutionTime.Awake) return;
			this.TriggerEvent();
		}

		protected virtual  void Start() {
			if (!this.enabled) return;
			if (this.executionTime != CMonobehaviourExecutionTime.Start) return;
			this.TriggerEvent();
		}

		protected virtual  void OnEnable() {
			if (!this.enabled) return;
			if (this.executionTime != CMonobehaviourExecutionTime.OnEnable) return;
			this.TriggerEvent();
		}

		protected abstract void TriggerEvent();

	}
}
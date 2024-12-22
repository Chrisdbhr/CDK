using UnityEngine;

namespace CDK {
	public abstract class CAutoTriggerCompBase : MonoBehaviour {

		[SerializeField] CMonobehaviourExecutionTime executionTime = CMonobehaviourExecutionTime.Awake;

		protected virtual void Awake() {
			if (executionTime != CMonobehaviourExecutionTime.Awake) return;
			TriggerEvent();
		}

		protected virtual  void Start() {
			if (!enabled) return;
			if (executionTime != CMonobehaviourExecutionTime.Start) return;
			TriggerEvent();
		}

		protected virtual  void OnEnable() {
			if (!enabled) return;
			if (executionTime != CMonobehaviourExecutionTime.OnEnable) return;
			TriggerEvent();
		}

		protected abstract void TriggerEvent();

	}
}
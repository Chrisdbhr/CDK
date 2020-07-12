using UnityEngine;

namespace CDK {
	public abstract class CMonoBehaviourUpdateExecutionLoopTime : MonoBehaviour {

		
		[SerializeField] private CMonobehaviourExecutionLoop _executionTime;
		

		#region <<---------- MonoBehaviour ---------->>

		private void Update() {
			if (this._executionTime != CMonobehaviourExecutionLoop.Update) return;
			this.Execute(CTime.DeltaTimeScaled);
		}

		private void FixedUpdate() {
			if (this._executionTime != CMonobehaviourExecutionLoop.FixedUpdate) return;
			this.Execute(CTime.FixedDeltaTimeScaled);
		}

		private void LateUpdate() {
			if (this._executionTime != CMonobehaviourExecutionLoop.LateUpdate) return;
			this.Execute(CTime.DeltaTimeScaled);
		}

		#endregion <<---------- MonoBehaviour ---------->>

		
		protected abstract void Execute(float deltaTime);
		

	}
}

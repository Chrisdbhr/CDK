using UnityEngine;

namespace CDK {
	public class CGameObjectRotator : CMonoBehaviourUpdateExecutionLoopTime {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private Vector3 _rotateDirectionAndSpeed;
		
		#endregion <<---------- Properties and Fields ---------->>
		
		protected override void Execute(float deltaTime) {
			this.transform.Rotate(this._rotateDirectionAndSpeed * (CTime.DeltaTimeScaled));
		}
	}
}
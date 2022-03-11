using System;
using UnityEngine;

namespace CDK {
	public class CGameObjectRotator : CMonoBehaviourUpdateExecutionLoopTime {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private Vector3 _rotateDirectionAndSpeed;
		[SerializeField] private bool _onlyUpdateWhenVisible = true;
		[SerializeField] private bool _ignoreTimescale;
		
		[NonSerialized] private bool _isVisible;
		
		#endregion <<---------- Properties and Fields ---------->>

		private void OnBecameVisible() {
			this._isVisible = true;
		}

		private void OnBecameInvisible() {
			this._isVisible = false;
		}

		protected override void Execute(float deltaTime) {
			if (this._onlyUpdateWhenVisible && !this._isVisible) return;
			this.transform.Rotate(this._rotateDirectionAndSpeed * (_ignoreTimescale ? Time.deltaTime : CTime.DeltaTimeScaled));
		}
	}
}
using System;
using UnityEngine;

namespace CDK {
	public class CGameObjectLinearForwardMove : CMonoBehaviourUpdateExecutionLoopTime {

		[SerializeField] private float _moveSpeed = 8f;
		[SerializeField] private float _timeToAutoDestroy = 10f;
		
		// Cache variables
		[NonSerialized] private Vector3 _pos;
		[NonSerialized] private Transform _transform;

		private void Awake() {
			this._transform = this.transform;
		}

		protected override void Execute(float deltaTime) {
			this._timeToAutoDestroy -= deltaTime;
			if (this._timeToAutoDestroy <= 0f) {
				this.gameObject.CDestroy();
				return;
			}

			this._transform.position += this._transform.forward * (deltaTime * this._moveSpeed);
		}
	}
}

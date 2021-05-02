using System;
using UnityEngine;

namespace CDK {
	public class CGameObjectSineLocalMovement : CMonoBehaviourUpdateExecutionLoopTime {
		
		[SerializeField] private Vector3 _sineDirectionAndMagnitude = Vector3.up * 0.5f;
		[SerializeField] private float _sineSpeed = 4f;
		
		[NonSerialized] private Vector3 _startPosition;
		
		
		private void Start() {
			this._startPosition = this.transform.localPosition;
		}

		protected override void Execute(float deltaTime) {
			this.transform.localPosition = this._startPosition + this._sineDirectionAndMagnitude * (Mathf.Sin(Time.time * this._sineSpeed) * Time.timeScale);
		}
	}
}

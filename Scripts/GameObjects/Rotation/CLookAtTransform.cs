using System;
using UnityEngine;

namespace CDK {
	public class CLookAtTransform : CMonoBehaviourUpdateExecutionLoopTime {

		[SerializeField] private Transform _lookAtTarget;
		[SerializeField] private float _speed = 1f;
		[SerializeField] [TagSelector] private string _targetTag = "Player";
		
		private Transform _transform;
		
		
		private void Awake() {
			_transform = this.transform;
		}

		protected override void Execute(float deltaTime) {
			if (!_lookAtTarget) return;
			var direction = _lookAtTarget.position - _transform.position;
			_transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(direction), this._speed * deltaTime);
		}

		private void OnTriggerEnter(Collider other) {
			if (!other.CompareTag(_targetTag)) return;
			_lookAtTarget = other.transform;
		}

		private void OnTriggerExit(Collider other) {
			if (!other.CompareTag(_targetTag) || other.transform != this._lookAtTarget) return;
			_lookAtTarget = null;
		}
	}
}

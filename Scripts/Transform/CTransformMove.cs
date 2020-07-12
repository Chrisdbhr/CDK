using System;
using UnityEngine;

namespace CDK {
	public class CTransformMove : MonoBehaviour {
		
		[SerializeField] private Vector3 _localDirectionAndSpeed = Vector3.forward;
		[NonSerialized] private Transform _transform;
		[NonSerialized] private Vector3 _newPosition;

		
		
		
		private void Awake() {
			this._transform = this.transform;
		}

		private void Update() {
			this._newPosition = this._transform.position;
			this._newPosition += this._transform.forward * (this._localDirectionAndSpeed.z * CTime.DeltaTimeScaled);
			this._newPosition += this._transform.right * (this._localDirectionAndSpeed.x * CTime.DeltaTimeScaled);
			this._newPosition += this._transform.up * (this._localDirectionAndSpeed.y * CTime.DeltaTimeScaled);
			this._transform.position = this._newPosition;
		}
		
	}
}

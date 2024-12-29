using System;
using UnityEngine;

namespace CDK {
	public class CTransformMove : MonoBehaviour {
		
		[SerializeField] Vector3 _localDirectionAndSpeed = Vector3.forward;
		[NonSerialized] Transform _transform;
		[NonSerialized] Vector3 _newPosition;


		void Awake() {
			_transform = transform;
		}

		void Update() {
			_newPosition = _transform.position;
			_newPosition += _transform.forward * (_localDirectionAndSpeed.z * CTime.DeltaTimeScaled);
			_newPosition += _transform.right * (_localDirectionAndSpeed.x * CTime.DeltaTimeScaled);
			_newPosition += _transform.up * (_localDirectionAndSpeed.y * CTime.DeltaTimeScaled);
			_transform.position = _newPosition;
		}
		
	}
}

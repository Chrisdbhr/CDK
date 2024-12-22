using System;
using Reflex.Attributes;
using UnityEngine;

namespace CDK {
	public class CMouseRotator : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] Vector2 _rotationSpeed = Vector2.one * 0.2f;
		[SerializeField] Vector2 _rotationYRange = new Vector2(60f, 60f);
		[SerializeField] Vector2 _rotationXRange = new Vector2(60f, 30f);
		Vector3 _eulerRotation;
		Vector2 _inputLook;
		Transform _transform;
		Quaternion _initialRotation;
		[Inject] readonly CBlockingEventsManager _blockingEventsManager;

		#endregion <<---------- Properties and Fields ---------->>

		#region <<---------- MonoBehaviour ---------->>
		protected void Awake() {
			_transform = transform;
			_initialRotation = _transform.rotation;
		}

		private void Update() {
			if (_blockingEventsManager.IsAnyHappening) return;
			
			_inputLook = new Vector2(Input.GetAxisRaw(CInputKeys.LOOK_X), Input.GetAxisRaw(CInputKeys.LOOK_Y));

			// rotate camera
			_transform.Rotate(Vector3.up,
				_inputLook.x * _rotationSpeed.x * CTime.DeltaTimeScaled,
				Space.World);
			_transform.Rotate(_transform.right,
				_inputLook.y * -1 * _rotationSpeed.y * CTime.DeltaTimeScaled,
				Space.World);

			// clamp rotation
			_eulerRotation = _transform.eulerAngles;
			_eulerRotation.z = 0f;
			// vertical rotation
			if (_eulerRotation.x.CIsInRange(_rotationXRange.x, 360 - _rotationXRange.y)) {
				_eulerRotation.x = _eulerRotation.x.CGetCloserValue(_rotationXRange.x, 360 - _rotationXRange.y);
			}
			
			// horizontal rotation
			if (_eulerRotation.y.CIsInRange(_rotationYRange.x, 360 - _rotationYRange.y)) {
				_eulerRotation.y = _eulerRotation.y.CGetCloserValue(_rotationYRange.x, 360 - _rotationYRange.y);
			}
			_transform.eulerAngles = _eulerRotation;
		}
		#endregion <<---------- MonoBehaviour ---------->>



		public void ResetRotation() {
			_transform.rotation = _initialRotation;
		}
		
	}
}

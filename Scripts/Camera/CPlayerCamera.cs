using System;
using UnityEngine;

namespace CDK {
	public class CPlayerCamera : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private float _rotationSpeed = 10f;
		[SerializeField] private Camera _camera;
		
		#endregion <<---------- Properties and Fields ---------->>



		#region <<---------- General ---------->>
		
		public void SetCameraEnabled(bool enabled) {
			this._camera.enabled = enabled;
		}

		public void Rotate(Vector2 inputRotation) {
			this.transform.Rotate(inputRotation.y * this._rotationSpeed, inputRotation.x * this._rotationSpeed, 0f);
		}
		
		#endregion <<---------- General ---------->>
		
	}
}

using System;
using UnityEngine;

namespace CDK {
	public class CMouseRotator : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private Vector2 _rotationSpeed = Vector2.one * 0.2f;
		[SerializeField] private Vector2 _rotationXRange = new Vector2(40f, 50f);
		[NonSerialized] private Vector3 _eulerRotation;
		[NonSerialized] private Vector2 _inputLook;
		[NonSerialized] private Transform _transform;

		#endregion <<---------- Properties and Fields ---------->>

	
		
	
		#region <<---------- MonoBehaviour ---------->>
	
		protected void Awake() {
			this._transform = this.transform;
		}

		private void Update() {
			this._inputLook = new Vector2(Input.GetAxisRaw(CInputKeys.LOOK_X), Input.GetAxisRaw(CInputKeys.LOOK_Y));
			
			// rotate camera
			this._transform.Rotate(Vector3.up,
				this._inputLook.x * (Screen.width <= 0 ? 1f : Screen.width) * this._rotationSpeed.x * Time.deltaTime,
				Space.World );
			this._transform.Rotate(this._transform.right,
				this._inputLook.y * (Screen.height <= 0 ? 1f : Screen.height) * -1 * this._rotationSpeed.y * Time.deltaTime, 
				Space.World );

			// clamp rotation
			this._eulerRotation = this._transform.eulerAngles;
			this._eulerRotation.z = 0f;
			if (this._eulerRotation.x.CIsInRange(this._rotationXRange.x, 360 - this._rotationXRange.y)) {
				this._eulerRotation.x = this._eulerRotation.x.CGetCloserValue(this._rotationXRange.x, 360 - this._rotationXRange.y);	
			}
			this._transform.eulerAngles = this._eulerRotation;
		}
		
		#endregion <<---------- MonoBehaviour ---------->>
		
	}
}
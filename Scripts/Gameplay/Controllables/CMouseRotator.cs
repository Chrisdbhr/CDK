using System;
using UnityEngine;

namespace CDK {
	public class CMouseRotator : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private Vector2 _rotationSpeed = Vector2.one * 0.2f;
		[SerializeField] private Vector2 _rotationYRange = new Vector2(60f, 60f);
		[SerializeField] private Vector2 _rotationXRange = new Vector2(60f, 30f);
		[NonSerialized] private Vector3 _eulerRotation;
		[NonSerialized] private Vector2 _inputLook;
		[NonSerialized] private Transform _transform;
		[NonSerialized] private Quaternion _initialRotation;
		[NonSerialized] private CBlockingEventsManager _blockingEventsManager;

		#endregion <<---------- Properties and Fields ---------->>

		#region <<---------- MonoBehaviour ---------->>
		protected void Awake() {
			this._transform = this.transform;
			this._initialRotation = this._transform.rotation;
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
		}

		private void Update() {

			if (this._blockingEventsManager.IsAnyBlockingEventHappening) return;
			
			this._inputLook = new Vector2(Input.GetAxisRaw(CInputKeys.LOOK_X), Input.GetAxisRaw(CInputKeys.LOOK_Y));

			// rotate camera
			this._transform.Rotate(Vector3.up,
				this._inputLook.x * this._rotationSpeed.x * CTime.DeltaTimeScaled,
				Space.World);
			this._transform.Rotate(this._transform.right,
				this._inputLook.y * -1 * this._rotationSpeed.y * CTime.DeltaTimeScaled,
				Space.World);

			// clamp rotation
			this._eulerRotation = this._transform.eulerAngles;
			this._eulerRotation.z = 0f;
			// vertical rotation
			if (this._eulerRotation.x.CIsInRange(this._rotationXRange.x, 360 - this._rotationXRange.y)) {
				this._eulerRotation.x = this._eulerRotation.x.CGetCloserValue(this._rotationXRange.x, 360 - this._rotationXRange.y);
			}
			
			// horizontal rotation
			if (this._eulerRotation.y.CIsInRange(this._rotationYRange.x, 360 - this._rotationYRange.y)) {
				this._eulerRotation.y = this._eulerRotation.y.CGetCloserValue(this._rotationYRange.x, 360 - this._rotationYRange.y);
			}
			this._transform.eulerAngles = this._eulerRotation;
		}
		#endregion <<---------- MonoBehaviour ---------->>



		public void ResetRotation() {
			this._transform.rotation = this._initialRotation;
		}
		
	}
}

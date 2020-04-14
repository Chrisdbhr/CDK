using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CDK {
	public class CPreventCameraWallClip : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>
		
		[FormerlySerializedAs("_targetCamera"),SerializeField] private Camera _targetChildCamera;
		[SerializeField] private LayerMask _collisionLayers;
		[SerializeField] private float _cameraRecoverSpeed = 10f;
		
		[NonSerialized] private float _defaultDistanceX;
		[NonSerialized] private float _defaultDistanceZ;
		[NonSerialized] private Transform _transform;
		[NonSerialized] private Transform _cameraTransform;
		[NonSerialized] private RaycastHit _raycastHit;
		
		#endregion <<---------- Properties and Fields ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>
		
		private void Awake() {
			this._transform = this.transform;
			this._cameraTransform = this._targetChildCamera.transform;
			this._defaultDistanceX = this._cameraTransform.localPosition.x;
			this._defaultDistanceZ = this._cameraTransform.localPosition.z;
		}

		private void LateUpdate() {
			this._cameraTransform.localPosition = new Vector3(this._defaultDistanceX, this._cameraTransform.localPosition.y, this._defaultDistanceZ);
			/*this._cameraTransform.localPosition = Vector3.Lerp(
				this._cameraTransform.localPosition,
				new Vector3(this._defaultDistanceX, this._cameraTransform.localPosition.y, this._defaultDistanceZ),
				this._cameraRecoverSpeed * Time.deltaTime
			);*/
			this.FixPositionWallCollision();
		}
		
		#endregion <<---------- MonoBehaviour ---------->>

		private void FixPositionWallCollision() {
			var camNearPlane = this._targetChildCamera.nearClipPlane;
			
			var camPos = this._cameraTransform.position;

			// check for something in front of the camera
			var camFwd = this._cameraTransform.forward;
			var camFwdInverted = camFwd * -1f;
			var origin = camPos + camFwd * this._cameraTransform.localPosition.z.CAbs();
			bool hitSomethingInFrontOfCamera = Physics.SphereCast(
				origin,
				camNearPlane,
				camFwdInverted,
				out this._raycastHit,
				this._cameraTransform.localPosition.z.CAbs(),
				this._collisionLayers,
				QueryTriggerInteraction.Ignore
			);

			if (hitSomethingInFrontOfCamera) {
				this._cameraTransform.position = origin + (camFwdInverted * (this._raycastHit.distance));
				return;
			}

			// check for something between cam and pivot
			var thisPos = this._transform.position;
			camPos = this._cameraTransform.position;
			var dir = (camPos - thisPos).normalized;
			bool hitSomethingBetweenCamAndThis = Physics.SphereCast(
				thisPos,
				camNearPlane,
				dir,
				out this._raycastHit,
				Vector3.Distance(thisPos, camPos),
				this._collisionLayers,
				QueryTriggerInteraction.Ignore
			);

			
			// fix position
			if (hitSomethingBetweenCamAndThis) {
				this._cameraTransform.position = thisPos + (dir * (this._raycastHit.distance));
			}

		}
		
	}
}

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
			_transform = transform;
			_cameraTransform = _targetChildCamera.transform;
			_defaultDistanceX = _cameraTransform.localPosition.x;
			_defaultDistanceZ = _cameraTransform.localPosition.z;
		}

		private void LateUpdate() {
			_cameraTransform.localPosition = new Vector3(_defaultDistanceX, _cameraTransform.localPosition.y, _defaultDistanceZ);
			/*this._cameraTransform.localPosition = Vector3.Lerp(
				this._cameraTransform.localPosition,
				new Vector3(this._defaultDistanceX, this._cameraTransform.localPosition.y, this._defaultDistanceZ),
				this._cameraRecoverSpeed * Time.deltaTime
			);*/
			FixPositionWallCollision();
		}
		
		#endregion <<---------- MonoBehaviour ---------->>

		private void FixPositionWallCollision() {
			var camNearPlane = _targetChildCamera.nearClipPlane;
			
			var camPos = _cameraTransform.position;

			// check for something in front of the camera
			var camFwd = _cameraTransform.forward;
			var camFwdInverted = camFwd * -1f;
			var origin = camPos + camFwd * _cameraTransform.localPosition.z.CAbs();
			bool hitSomethingInFrontOfCamera = Physics.SphereCast(
				origin,
				camNearPlane,
				camFwdInverted,
				out _raycastHit,
				_cameraTransform.localPosition.z.CAbs(),
				_collisionLayers,
				QueryTriggerInteraction.Ignore
			);

			if (hitSomethingInFrontOfCamera) {
				_cameraTransform.position = origin + (camFwdInverted * (_raycastHit.distance));
				return;
			}

			// check for something between cam and pivot
			var thisPos = _transform.position;
			camPos = _cameraTransform.position;
			var dir = (camPos - thisPos).normalized;
			bool hitSomethingBetweenCamAndThis = Physics.SphereCast(
				thisPos,
				camNearPlane,
				dir,
				out _raycastHit,
				Vector3.Distance(thisPos, camPos),
				_collisionLayers,
				QueryTriggerInteraction.Ignore
			);

			
			// fix position
			if (hitSomethingBetweenCamAndThis) {
				_cameraTransform.position = thisPos + (dir * (_raycastHit.distance));
			}

		}
		
	}
}

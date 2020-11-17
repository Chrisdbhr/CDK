using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CDK {
	[RequireComponent(typeof(SphereCollider))]
	public class CPlayerCamera : MonoBehaviour {

		#region <<---------- Enums ---------->>

		public enum CameraTransitionType {
			fade,
			crossfade
		}

		#endregion <<---------- Enums ---------->>
		
		
		#region <<---------- Properties and Fields ---------->>
		
		// Main refs
		[SerializeField] private CGamePlayer _ownerPlayer;
		[SerializeField] private Camera _unityCamera;
		
		// Position
		[NonSerialized] private Vector3 FreeCamPosition;
		[NonSerialized] private Vector3 newPosition;

		// Rotation
		[NonSerialized] private Quaternion FreeCamRotation;
		[NonSerialized] private Quaternion newRotation;
		[SerializeField] private float _rotationSpeed = 10f;
		public float RotationX { get; private set; }
		public float RotationY { get; private set; }
		[SerializeField] private float xSpeed = 2.4f;
		[SerializeField] private float ySpeed = 2.4f;
		[SerializeField] private float yMinLimit = -20f;
		[SerializeField] private float yMaxLimit = 80f;

		// Wall collision
		[SerializeField] private LayerMask wallCollisionLayers = 1;
		[NonSerialized] private RaycastHit _raycastHit;
		
		// Camera distance from target
		[SerializeField] private Renderer[] _renderToHideWhenCameraIsClose;
		[NonSerialized] private float _currentDistanceFromTarget = 10.0f;
		[NonSerialized] private float _distanceToConsiderCloseForCharacter = 0.5f;
		[NonSerialized] private ReactiveProperty<bool> _isCloseToTheCharacterRx;
		[NonSerialized] private float _clampMaxDistanceSpeed = 3f;

		// Screen print
		[SerializeField] private RawImage _screenPrintFrameRawImage;
		[NonSerialized] private bool _getRenderImgOnNextFrame;
		[NonSerialized] private Texture2D _screenShootTexture2d;
		
		// Camera profiles
		private ReactiveProperty<CCameraAreaProfileData> ActiveProfileRx = new ReactiveProperty<CCameraAreaProfileData>();
		[SerializeField] private CCameraAreaProfileData defaultProfile;
		private HashSet<CCameraAreaProfileData> _activeCameraProfilesList = new HashSet<CCameraAreaProfileData>();

		// Cache
		[NonSerialized] private Transform _transform;
		[NonSerialized] private CompositeDisposable _compositeDisposable;
		[NonSerialized] private Tweener _tween;
		
		#endregion <<---------- Properties and Fields ---------->>

		


		#region <<---------- MonoBehaviour ---------->>
		
		private void Awake() {
			this.ActiveProfileRx.Value = this.defaultProfile;
			
			this._transform = this.transform;

			Vector3 angles = this._transform.eulerAngles;
			this.RotationY = angles.y;
			this.RotationX = angles.x;
		}

		private void OnEnable() {
			this._compositeDisposable?.Dispose();
			this._compositeDisposable = new CompositeDisposable();

			this.SubscribeToEvents();
			
			this._screenShootTexture2d = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false, true);
		}

		private void OnDisable() {
			this._compositeDisposable?.Dispose();
			this._compositeDisposable = null;
			
			this.UnsubscribeToEvents();
		}

		private void OnDestroy() {
			Destroy(this._screenShootTexture2d);
			this._screenShootTexture2d = null;
		}
		
		private void LateUpdate() {
			var camTransform = this._unityCamera.transform;
			this.FreeCamPosition = camTransform.position;
			this.FreeCamRotation = camTransform.rotation;

			// camera distance
			bool camIsCloseToTheCharacter = this._currentDistanceFromTarget <= this._distanceToConsiderCloseForCharacter;
			this._isCloseToTheCharacterRx.Value = camIsCloseToTheCharacter;
			
			if (this._ownerPlayer == null) return;
			
			this.ProcessRotation();
			this.ProcessPosition();
		}

		private void OnTriggerEnter(Collider other) {
			var a = other.GetComponent<CCameraAreaVolume>();
			if (a == null) return;
			if (a.CameraAreaProfileData == null) {
				Debug.LogWarning($"Ignoring null new area profile data OnTriggerEnter from {other.name}!");
				return;
			}
			this.EnteredCameraVolume(a.CameraAreaProfileData);
		}

		private void OnTriggerExit(Collider other) {
			var a = other.GetComponent<CCameraAreaVolume>();
			if (a == null) return;
			if (a.CameraAreaProfileData == null) {
				Debug.LogWarning($"Ignoring null new area profile data OnTriggerExit from {other.name}!");
				return;
			}
			this.ExitedCameraVolume(a.CameraAreaProfileData);
		}
		
		private void OnPostRender() {
			if (this._getRenderImgOnNextFrame) {
				this._screenShootTexture2d.Resize(Screen.width, Screen.height);
				this._screenShootTexture2d.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
				this._screenShootTexture2d.Apply();
				this._screenPrintFrameRawImage.texture = this._screenShootTexture2d;
				this._screenPrintFrameRawImage.color = Color.white;
				this._getRenderImgOnNextFrame = false;
			}
		}
		
		
		#if UNITY_EDITOR
		private void OnDrawGizmos() {
			if (this._unityCamera) {
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere(this.transform.position, this._unityCamera.nearClipPlane);
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(this._raycastHit.point, this._unityCamera.nearClipPlane);
			}
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		#region <<---------- Events ---------->>

		private void SubscribeToEvents() {
			this._isCloseToTheCharacterRx = new ReactiveProperty<bool>(false);

			// subscribe to CAMERA PROFILE changed
			this.ActiveProfileRx.Subscribe(newProfile => {
				if(this._unityCamera == null) return;
				DOVirtual.Float(this._unityCamera.fieldOfView, newProfile.fov, 1f, value => { this._unityCamera.fieldOfView = value; });
				DOVirtual.Float(this._unityCamera.farClipPlane, newProfile.FarClippingPlane, 1f, value => { this._unityCamera.farClipPlane = value; });
			}).AddTo(this._compositeDisposable);

			// player warped callback 
			Debug.LogWarning("TODO implement PlayerWarped callback");
			// this._ownerPlayer.OnPlayerWarped += (oldPos, newPos) => {
			// 	this.ResetPositionAndRotationToPlayerRotation(oldPos, newPos);
			//
			// 	//this._freeLookCamera.OnTargetObjectWarped(this.FollowAndLookTarget, newPos - oldPos);
			// };
			
			// is close to the character?
			this._isCloseToTheCharacterRx.Subscribe(isClose => {
				// disable renderers
				foreach (var objToDisable in this._renderToHideWhenCameraIsClose) {
					objToDisable.enabled = !isClose;
				}
			}).AddTo(this._compositeDisposable);
		}

		private void UnsubscribeToEvents() {
		}
		
		#endregion <<---------- Events ---------->>

		
		

		#region <<---------- General ---------->>
		
		public void SetCameraEnabled(bool enabledState) {
			this._unityCamera.enabled = enabledState;
		}

		public void Rotate(Vector2 inputRotation) {
			#if !UNITY_ANDROID
			this.RotationY += inputRotation.x * this.xSpeed * Time.deltaTime; // * this.currentDistanceFromTarget; // make it rotate faster when very far from target.
			this.RotationX -= inputRotation.y * this.ySpeed * Time.deltaTime;
			#endif
			this.transform.Rotate(inputRotation.y * this._rotationSpeed, inputRotation.x * this._rotationSpeed, 0f);
		}
		
		#endregion <<---------- General ---------->>

		
		
		
		#region <<---------- Camera Transition ---------->>

		public void StartTransition(float duration, CameraTransitionType transitionType) {
			this._tween?.Complete();
			this._tween?.Kill();

			switch (transitionType) {
				case CameraTransitionType.fade:
					print($"[CPlayerCamera] Start fade transition with duration {duration}.");
					CFadeCanvas.FadeToBlack(duration).CAwait();
					break;
			case CameraTransitionType.crossfade:
					print($"[CPlayerCamera] Start {CameraTransitionType.crossfade.ToString()} transition with duration {duration}.");
					this._getRenderImgOnNextFrame = true;
					break;
			}

			CBlockingEventsManager.IsPlayingCutscene = true;
			this._tween.Play();
		}

		public void ReverseTransition(float duration, CameraTransitionType transitionType) {
			this._tween?.Complete();
			this._tween?.Kill();

			switch (transitionType) {
				case CameraTransitionType.fade:
					print($"[CPlayerCamera] Reverse fade transition with duration {duration}.");
					CFadeCanvas.FadeToTransparent(duration).CAwait();
					break;
				case CameraTransitionType.crossfade:
					print($"[CPlayerCamera] Reverse {CameraTransitionType.crossfade.ToString()} transition with duration {duration}.");
					this._tween = DOVirtual.Float(this._screenPrintFrameRawImage.color.a, 0f, duration, value => {
						var color = this._screenPrintFrameRawImage.color;
						color = new Color(color.r, color.g, color.b, value);
						this._screenPrintFrameRawImage.color = color;
					});
					break;
			}

			CBlockingEventsManager.IsPlayingCutscene = false;
			this._tween.Play();
		}

		#endregion <<---------- Camera Transition ---------->>

		
		

		#region <<---------- Camera Volume ---------->>
		
		private void EnteredCameraVolume(CCameraAreaProfileData newCameraAreaProfileData) {
			if (!this._activeCameraProfilesList.Add(newCameraAreaProfileData)) return;
			Debug.Log($"Entered camera volume {newCameraAreaProfileData.name}.");
			this.ActiveProfileRx.Value = newCameraAreaProfileData;
		}

		private void ExitedCameraVolume(CCameraAreaProfileData newCameraAreaProfileData) {
			if (!this._activeCameraProfilesList.Remove(newCameraAreaProfileData)) return;
			Debug.Log($"Exiting camera volume {newCameraAreaProfileData.name}.");

			if (this._activeCameraProfilesList.Count <= 0) {
				this.ActiveProfileRx.Value = this.defaultProfile;
				return;
			}

			this.ActiveProfileRx.Value = this._activeCameraProfilesList.LastOrDefault();
		}
		
		#endregion <<---------- Camera Volume ---------->>
		
		
		
		
		#region <<---------- Position and Rotation ---------->>

		private void ProcessRotation() {
			this.RotationX = this.RotationX.CClampAngle(this.yMinLimit, this.yMaxLimit);
			this.newRotation = Quaternion.Euler(this.RotationX, this.RotationY, 0);
			this._transform.rotation = this.newRotation;
		}

		private void ProcessPosition() {
			var character = this._ownerPlayer.GetMainCharacter();
			if (character == null) return;
			
			// clamp max distance
			var clampedDistance = Mathf.Clamp(this._currentDistanceFromTarget, 0f, this.ActiveProfileRx.Value.maxDistanceFromPlayer);
			this._currentDistanceFromTarget = Mathf.Lerp(this._currentDistanceFromTarget, clampedDistance, this._clampMaxDistanceSpeed * Time.deltaTime);
			if (this._currentDistanceFromTarget < 0f) this._currentDistanceFromTarget = 0f;
			
			// set position
			var finalPosition = character.transform.position;
			this.newPosition = this.newRotation
					* new Vector3(0f, 0f, -(this._currentDistanceFromTarget))
					+ finalPosition;

			this.FixPositionWallCollision(finalPosition);

			this._transform.position = this.newPosition;
		}

		private void FixPositionWallCollision(Vector3 followAndLookTargetPos) {
			
			float nearClipPlane = this._unityCamera.nearClipPlane;
			float posOffset = this.ActiveProfileRx.Value.WallCheckOffset + nearClipPlane;
			bool hitSomething = Physics.SphereCast(
				followAndLookTargetPos,
				nearClipPlane,
				(this.newPosition - followAndLookTargetPos).normalized,
				out this._raycastHit,
				Vector3.Distance(this.newPosition, followAndLookTargetPos) + posOffset,
				this.wallCollisionLayers,
				QueryTriggerInteraction.Ignore
			);

			// wall collision
			if (hitSomething) {
				this._currentDistanceFromTarget = this._raycastHit.distance - posOffset;
			}

			this.newPosition = this.newRotation * new Vector3(0.0f, 0.0f, -this._currentDistanceFromTarget) + followAndLookTargetPos;
		}

		#endregion <<---------- Position and Rotation ---------->>
	
	}
}

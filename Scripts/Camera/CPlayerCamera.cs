using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using FMODUnity;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CDK {
	public class CPlayerCamera : MonoBehaviour {

		#region <<---------- Initializers ---------->>

		public void Initialze(CGamePlayer ownerPlayer) {
			
			// owner player
			if (ownerPlayer == null) {
				Debug.LogError($"Could not create player camera of a null player.");
				return;
			}
			this._ownerPlayer = ownerPlayer;

			// character object
			var ownerCharacter = this._ownerPlayer.GetControllingCharacter();
			
			// cinemachine

			var lookTarget = ownerCharacter.GetComponentInChildren<CCameraLookAndFollowTarget>();
			
			this._cinemachineCamera.Follow = lookTarget != null ? lookTarget.transform : ownerCharacter.transform;
			this._cinemachineCamera.LookAt = lookTarget != null ? lookTarget.transform : ownerCharacter.transform;
			
			// fmod listener
			this._studioListener.ListenerNumber = this._ownerPlayer.PlayerNumber;
			this._studioListener.attenuationObject = ownerCharacter.gameObject;
		}
		
		#endregion <<---------- Initializers ---------->>
		

		#region <<---------- Enums ---------->>

		public enum CameraTransitionType {
			fade,
			crossfade
		}

		#endregion <<---------- Enums ---------->>
		
		
		#region <<---------- Properties and Fields ---------->>
		
		// Position
		[SerializeField] private Vector3 _offset = Vector3.up * 1.6f;
		[NonSerialized] private Vector3 newPosition;

		// Rotation
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
		
		// Camera 
		[SerializeField] private UnityEngine.Camera _unityCamera;
		[SerializeField] private CinemachineVirtualCamera _cinemachineCamera;
		

		[SerializeField] private Renderer[] _renderToHideWhenCameraIsClose;
		[NonSerialized] private float _currentDistanceFromTarget = 10.0f;
		[NonSerialized] private float _distanceToConsiderCloseForCharacter = 0.5f;
		[NonSerialized] private ReactiveProperty<bool> _isCloseToTheCharacterRx;
		[NonSerialized] private float _clampMaxDistanceSpeed = 3f;

		// Screen print
		[SerializeField] private RawImage _screenCrossfadeRawImage;
		[NonSerialized] private bool _getRenderImgOnNextFrame;
		[NonSerialized] private Texture2D _screenShootTexture2d;
		
		// Camera profiles
		private ReactiveProperty<CCameraAreaProfileData> ActiveProfileRx;
		[SerializeField] private CCameraAreaProfileData defaultProfile;
		private readonly HashSet<CCameraAreaProfileData> _activeCameraProfilesList = new HashSet<CCameraAreaProfileData>();

		// Audio
		[SerializeField] private StudioListener _studioListener;
		
		// Cache
		[NonSerialized] private CGamePlayer _ownerPlayer;
		[NonSerialized] private Transform _transform;
		[NonSerialized] private CompositeDisposable _compositeDisposable;
		[NonSerialized] private Tweener _tween;
		
		#endregion <<---------- Properties and Fields ---------->>

		


		#region <<---------- MonoBehaviour ---------->>

		private void Awake() {
			this._transform = this.transform;

			if(this.defaultProfile == null) this.defaultProfile = ScriptableObject.CreateInstance<CCameraAreaProfileData>();
			
			// active camera profile
			this.ActiveProfileRx = new ReactiveProperty<CCameraAreaProfileData>();
			this.ActiveProfileRx.Value = this.defaultProfile;

		}
		
		private void OnEnable() {
			this._compositeDisposable?.Dispose();
			this._compositeDisposable = new CompositeDisposable();
			
			var angles = this._transform.eulerAngles;
			this.RotationY = angles.y;
			this.RotationX = angles.x;

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
			
			if(this.defaultProfile == default) this.defaultProfile.CDestroy();
		}
		
		private void LateUpdate() {
			// var camTransform = this._unityCamera.transform;
			// this.FreeCamPosition = camTransform.position;
			// this.FreeCamRotation = camTransform.rotation;

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
				this._screenCrossfadeRawImage.texture = this._screenShootTexture2d;
				this._screenCrossfadeRawImage.color = Color.white;
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


		

		#region <<---------- General ---------->>

		public Transform GetCameraTransform() {
			return this._unityCamera.transform;
		}
		
		#endregion <<---------- General ---------->>
		

		

		#region <<---------- Events ---------->>

		private void SubscribeToEvents() {
			this._isCloseToTheCharacterRx = new ReactiveProperty<bool>(false);

			// subscribe to CAMERA PROFILE changed
			this.ActiveProfileRx.Subscribe(newProfile => {
				if(this._unityCamera == null) return;
				DOVirtual.Float(this._unityCamera.fieldOfView, newProfile.fieldOfView, 1f, value => { this._unityCamera.fieldOfView = value; });
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
				if (this._renderToHideWhenCameraIsClose == null) return;
				// disable renderers
				foreach (var objToDisable in this._renderToHideWhenCameraIsClose) {
					if (objToDisable == null) continue;
					objToDisable.enabled = !isClose;
				}
			}).AddTo(this._compositeDisposable);
		}

		private void UnsubscribeToEvents() {
		}
		
		#endregion <<---------- Events ---------->>
		
		
		
		
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
					this._tween = DOVirtual.Float(this._screenCrossfadeRawImage.color.a, 0f, duration, value => {
						var color = this._screenCrossfadeRawImage.color;
						color = new Color(color.r, color.g, color.b, value);
						this._screenCrossfadeRawImage.color = color;
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
			var character = this._ownerPlayer.GetControllingCharacter();
			if (character == null) return;
			
			// clamp max distance
			this._currentDistanceFromTarget += this.ActiveProfileRx.Value.RecoverFromWallSpeed * CTime.DeltaTimeScaled;
			var clampedDistance = this._currentDistanceFromTarget.CClamp(0f, this.ActiveProfileRx.Value.maxDistanceFromPlayer);
			this._currentDistanceFromTarget = Mathf.Lerp(this._currentDistanceFromTarget, clampedDistance, this._clampMaxDistanceSpeed * CTime.DeltaTimeScaled);
			if (this._currentDistanceFromTarget < 0f) this._currentDistanceFromTarget = 0f;
			
			// set position
			var finalPosition = character.transform.position + this._offset;
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

using System;
using Cinemachine;
using DG.Tweening;
using FMODUnity;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CDK {
	public class CPlayerCamera : MonoBehaviour {

		#region <<---------- Initializers ---------->>

		public void Initialize(CGamePlayer ownerPlayer) {
			
			// owner player
			if (ownerPlayer == null) {
				Debug.LogError($"Could not create player camera of a null player.");
				return;
			}
			this._ownerPlayer = ownerPlayer;

			// character object
			var ownerCharacter = this._ownerPlayer.GetControllingCharacter();
			
			// renderers to hide
			this._renderToHideWhenCameraIsClose = ownerCharacter.GetComponentsInChildren<Renderer>(); 
			
			// cinemachine

			var lookTarget = ownerCharacter.GetComponentInChildren<CCameraLookAndFollowTarget>();
			
			this._cinemachineCamera.Follow = ownerCharacter.transform;
			this._cinemachineCamera.LookAt = lookTarget != null ? lookTarget.transform : ownerCharacter.transform;
			
			// fmod listener
			this._studioListener.ListenerNumber = this._ownerPlayer.PlayerNumber;
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		

		#region <<---------- Enums ---------->>

		public enum CameraTransitionType {
			fade,
			crossfade
		}

		#endregion <<---------- Enums ---------->>
		
		
		
		
		#region <<---------- Properties and Fields ---------->>
		
		// Rotation
		public float RotationX { get; private set; }
		public float RotationY { get; private set; }
		[NonSerialized] private Tween _recenterRotationTween;
		
		// Camera 
		[SerializeField] private UnityEngine.Camera _unityCamera;
		[SerializeField] private CinemachineBrain _cinemachineBrain;
		[SerializeField] private CinemachineVirtualCameraBase _cinemachineCamera;
		[NonSerialized] private int _currentGameFrame;

		[SerializeField] private Renderer[] _renderToHideWhenCameraIsClose;
		[NonSerialized] private float _currentDistanceFromTarget = 10.0f;
		[NonSerialized] private float _distanceToConsiderCloseForCharacter = 0.5f;
		[NonSerialized] private ReactiveProperty<bool> _isCloseToTheCharacterRx;
		[NonSerialized] private float _clampMaxDistanceSpeed = 3f;

		// Screen print
		[SerializeField] private RawImage _screenCrossfadeRawImage;
		[NonSerialized] private bool _getRenderImgOnNextFrame;
		[NonSerialized] private Texture2D _screenShootTexture2d;
		
		// Audio
		[SerializeField] private StudioListener _studioListener;
		
		// Cache
		[NonSerialized] private CGamePlayer _ownerPlayer;
		[NonSerialized] private Transform _transform;
		[NonSerialized] private Tweener _tween;
		[NonSerialized] private CFader _fader;
		[NonSerialized] private CBlockingEventsManager _blockingEventsManager;

		#endregion <<---------- Properties and Fields ---------->>

		


		#region <<---------- MonoBehaviour ---------->>

		private void Awake() {
			this._transform = this.transform;
			this._fader = CDependencyResolver.Get<CFader>();
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
		}

		private void OnEnable() {
			var angles = this._transform.eulerAngles;
			this.RotationY = angles.y;
			this.RotationX = angles.x;

			this.SubscribeToEvents();
			
			this._screenShootTexture2d = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false, true);
		}

		private void OnDisable() {
			this.UnsubscribeToEvents();
		}

		private void OnDestroy() {
			Destroy(this._screenShootTexture2d);
			this._screenShootTexture2d = null;
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

			// camera sensitivity settings
			if (this._cinemachineCamera is CinemachineFreeLook freeLookCamera) {
				CSave.getRx.ObserveEveryValueChanged(p=>p.Value.CameraSensitivity.x).TakeUntilDisable(this).Subscribe(value => {
					freeLookCamera.m_XAxis.m_MaxSpeed = value;
				});
				CSave.getRx.ObserveEveryValueChanged(p=>p.Value.CameraSensitivity.y).TakeUntilDisable(this).Subscribe(value => {
					freeLookCamera.m_YAxis.m_MaxSpeed = value;
				});
			}else if (this._cinemachineCamera is CinemachineVirtualCamera virtualCamera) {
				var pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
				if (pov == null) return;
				CSave.getRx.ObserveEveryValueChanged(p=>p.Value.CameraSensitivity.x).TakeUntilDisable(this).Subscribe(value => {
					pov.m_HorizontalAxis.m_MaxSpeed = value;
				});
				CSave.getRx.ObserveEveryValueChanged(p=>p.Value.CameraSensitivity.y).TakeUntilDisable(this).Subscribe(value => {
					pov.m_VerticalAxis.m_MaxSpeed = value;
				});
			}
			
			// active camera changed
			this._cinemachineBrain.m_CameraActivatedEvent.AddListener(this.ActiveCameraChanged);

			// is close to the character?
			Observable.EveryUpdate().TakeUntilDisable(this).Subscribe(_ => {
				this._currentDistanceFromTarget = Vector3.Distance(this._cinemachineCamera.LookAt.position, this._unityCamera.transform.position); 
				this._isCloseToTheCharacterRx.Value = this._currentDistanceFromTarget <= this._distanceToConsiderCloseForCharacter;
			});
			this._isCloseToTheCharacterRx.TakeUntilDisable(this).Subscribe(isClose => {
				if (this._renderToHideWhenCameraIsClose.Length <= 0) return;
				// disable renderers
				foreach (var objToDisable in this._renderToHideWhenCameraIsClose) {
					if (objToDisable == null) continue;
					objToDisable.enabled = !isClose;
				}
			});
			
		}
		
		private void UnsubscribeToEvents() {
			this._cinemachineBrain.m_CameraActivatedEvent.RemoveListener(this.ActiveCameraChanged);
		}
		
		private void ActiveCameraChanged(ICinemachineCamera newCamera, ICinemachineCamera oldCamera) {
			if (ReferenceEquals(this._cinemachineCamera, newCamera)) {
				if (this._cinemachineCamera is CinemachineFreeLook freeLookCamera) {
					freeLookCamera.m_YAxis.Value = 0.5f;
				}
			}
		}
		
		#endregion <<---------- Events ---------->>

		
		

		#region <<---------- Input ---------->>

		public void ResetRotation(float duration = 0.3f) {
			if (this._recenterRotationTween != null ) return;
			this._recenterRotationTween?.Kill();

			if (this._cinemachineCamera is CinemachineFreeLook freeLookCamera) {
				var finalTimes = new Vector2(freeLookCamera.m_RecenterToTargetHeading.m_RecenteringTime, freeLookCamera.m_YAxisRecentering.m_RecenteringTime);
				
				this._recenterRotationTween = DOTween.To(
					()=>new Vector2(freeLookCamera.m_RecenterToTargetHeading.m_RecenteringTime * 0.01f, freeLookCamera.m_YAxisRecentering.m_RecenteringTime * 0.01f),
					x=> {
						freeLookCamera.m_RecenterToTargetHeading.m_RecenteringTime = x.x;
						freeLookCamera.m_YAxisRecentering.m_RecenteringTime = x.y;
						
						freeLookCamera.m_RecenterToTargetHeading.RecenterNow();
						freeLookCamera.m_YAxisRecentering.RecenterNow();
					},
					finalTimes,
					duration
				);

				this._recenterRotationTween.onComplete += () => {
					this._recenterRotationTween?.Kill();
					this._recenterRotationTween = null;
				};
				this._recenterRotationTween.Play();
			}
		}
		
		#endregion <<---------- Input ---------->>



		
		#region <<---------- Camera Transition ---------->>

		public void StartTransition(float duration, CameraTransitionType transitionType) {
			this._tween?.Complete();
			this._tween?.Kill();

			switch (transitionType) {
				case CameraTransitionType.fade:
					print($"[CPlayerCamera] Start fade transition with duration {duration}.");
					this._fader.FadeToBlack(duration, false);
					break;
			case CameraTransitionType.crossfade:
					print($"[CPlayerCamera] Start {CameraTransitionType.crossfade.ToString()} transition with duration {duration}.");
					this._getRenderImgOnNextFrame = true;
					break;
			}

			this._blockingEventsManager.IsPlayingCutscene = true;
			this._tween.Play();
		}

		public void ReverseTransition(float duration, CameraTransitionType transitionType) {
			this._tween?.Complete();
			this._tween?.Kill();

			switch (transitionType) {
				case CameraTransitionType.fade:
					print($"[CPlayerCamera] Reverse fade transition with duration {duration}.");
					this._fader.FadeToTransparent(duration, false);
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

			this._blockingEventsManager.IsPlayingCutscene = false;
			this._tween.Play();
		}

		#endregion <<---------- Camera Transition ---------->>
		
	}
}

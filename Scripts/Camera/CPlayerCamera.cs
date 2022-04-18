using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if Cinemachine
using Cinemachine;
#endif

#if DOTween
using DG.Tweening;
#endif

#if FMOD 
using FMODUnity;
#endif

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
			this._ownerCharacter = this._ownerPlayer.GetControllingCharacter();
			
			// renderers to hide
			this._renderToHideWhenCameraIsClose = this._ownerCharacter.GetComponentsInChildren<SkinnedMeshRenderer>()
            .Where(s=> s.name == "Face" || s.name == "Body" || s.name ==  "Hair").ToArray(); 
			
			// cinemachine
			this.UpdateCameraTargets();

			#if FMOD
			// fmod listener
			this._studioListener.ListenerNumber = this._ownerPlayer.PlayerNumber;
			#endif
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		

		#region <<---------- Enums ---------->>

		public enum CameraTransitionType {
			fade,
		}
		
		public enum CameraType {
			@default,
			smallAreaCloseLimitedVision,
			bigOpenArea,
            twoDCamera
        }

		#endregion <<---------- Enums ---------->>
		
		
		
		
		#region <<---------- Properties and Fields ---------->>
		
		// Rotation
		public float RotationX { get; private set; }
		public float RotationY { get; private set; }
		#if DOTween
		private Tween _recenterRotationTween;
		#endif
		
		// Camera 
		[SerializeField] private UnityEngine.Camera _unityCamera;
		#if Cinemachine
		[SerializeField] private CinemachineBrain _cinemachineBrain;
		[SerializeField] private CinemachineVirtualCameraBase[] _cinemachineCameras;
		#endif
		private int _currentGameFrame;

		[Header("Shake")]
		[SerializeField] private float _shakeLerpAmount = 0.7f;
		[SerializeField] private float _fallShakeMultiplier = 3f;
		[SerializeField] [Range(0f, 20f)] private float _cameraShakeMinimumSpeedToApply = 6f;
		[SerializeField] [Range(0f, 1f)] private float _cameraShakeAmplitude;
		[SerializeField] [Range(0f, 1f)] private float _cameraShakeFrequency;
		[SerializeField] private SkinnedMeshRenderer[] _renderToHideWhenCameraIsClose;
		private float _currentDistanceFromTarget = 10.0f;
		private float _distanceToConsiderCloseForCharacter = 0.5f;
		private ReactiveProperty<bool> _isCloseToTheCharacterRx;
		private float _clampMaxDistanceSpeed = 3f;

		// Audio
		#if FMOD
		[SerializeField] private StudioListener _studioListener;
		#endif
		
		// Cache
		private CGamePlayer _ownerPlayer;
		private Transform _transform;
		private CCharacterBase _ownerCharacter;
		
		#if DOTween
		private Tweener _tween;
		#endif
		
		private CFader _fader;
		private CBlockingEventsManager _blockingEventsManager;
		
		// Camera Profiles
		private List<CCameraProfileVolume> ActiveCameraProfiles;
		
		#endregion <<---------- Properties and Fields ---------->>

		


		#region <<---------- MonoBehaviour ---------->>

		private void Awake() {
			this._transform = this.transform;
			this._fader = CDependencyResolver.Get<CFader>();
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
			this.ActiveCameraProfiles = new List<CCameraProfileVolume>();
			this._isCloseToTheCharacterRx = new ReactiveProperty<bool>(false);

			this.SearchForGlobalVolume();
			this.ApplyLastOrDefaultCameraProfile();
		}

		private void OnEnable() {
			var angles = this._transform.eulerAngles;
			this.RotationY = angles.y;
			this.RotationX = angles.x;
			
			// is close to the character?
			Observable.EveryUpdate().TakeUntilDisable(this).Subscribe(_ => {
				if (CApplication.IsQuitting) return;
				if (this._cinemachineBrain == null || this._cinemachineBrain.ActiveVirtualCamera == null || this._cinemachineBrain.ActiveVirtualCamera.Follow == null) return;
				
				// camera shake intensity
				if (this._ownerCharacter && this._cinemachineBrain.ActiveVirtualCamera is CinemachineFreeLook cam) {
					var velocity = this._ownerCharacter.GetMyVelocity();
					if(velocity.y < 0) velocity.y *= _fallShakeMultiplier;
					var magnitude = velocity.magnitude;
					float amplitude = 0f;
					float frequency = 0f;
					if (magnitude <= _cameraShakeMinimumSpeedToApply) {
						amplitude = 0f;
						frequency = 0f;
					}
					else {
						amplitude = magnitude * this._cameraShakeAmplitude;
						frequency = magnitude * this._cameraShakeFrequency;
					}
					for (int i = 0; i < 3; i++) {
						var rig = cam.GetRig(i);
						if (rig) {
							var noise = rig.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
							if (noise) {
								noise.m_AmplitudeGain = noise.m_AmplitudeGain.CLerp(amplitude, this._shakeLerpAmount * CTime.DeltaTimeScaled);
								noise.m_FrequencyGain = noise.m_FrequencyGain.CLerp(frequency, this._shakeLerpAmount * CTime.DeltaTimeScaled);
							}
						}
					}
				}
				
				this._currentDistanceFromTarget = Vector3.Distance(this._cinemachineBrain.ActiveVirtualCamera.Follow.position, this._unityCamera.transform.position); 
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
			
			this.SubscribeToEvents();
        }

		private void OnDisable() {
			this.UnsubscribeToEvents();
		}

		#if UNITY_EDITOR
		private void OnDrawGizmos() {
			if (this._unityCamera) {
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere(this.transform.position, this._unityCamera.nearClipPlane);
			}
		}
		#endif
		
		private void OnTriggerEnter(Collider other) {
			var cameraArea = other.GetComponent<CCameraProfileVolume>();
			if (cameraArea == null) return;
			this.EnteredCameraArea(cameraArea);
		}

		private void OnTriggerExit(Collider other) {
			var cameraArea = other.GetComponent<CCameraProfileVolume>();
			if (cameraArea == null) return;
			this.ExitedCameraArea(cameraArea);
		}
		
		#endregion <<---------- MonoBehaviour ---------->>
		
		
		

		#region <<---------- General ---------->>

		public Transform GetCameraTransform() {
			return this._unityCamera.transform;
		}

		public void UpdateCameraTargets() {
			#if Cinemachine
			var lookTarget = this._ownerCharacter.GetComponentInChildren<CCameraLookAndFollowTarget>();
			var target = lookTarget != null ? lookTarget.transform : this._ownerCharacter.transform;
			foreach (var cam in this._cinemachineCameras) {
				cam.Follow = target;
				cam.LookAt = target;
			}
			#else
			Debug.LogError("'PlayerCamera' will not work without Cinemachine");
			#endif
		}

		#endregion <<---------- General ---------->>
		

		

		#region <<---------- Events ---------->>

		private void SubscribeToEvents() {
			#if Cinemachine

			// camera sensitivity changed
			CSave.getRx.ObserveEveryValueChanged(p=>p.Value.CameraSensitivity.x).TakeUntilDisable(this).Subscribe(value => {
				// freelook camera
				if (this._cinemachineBrain.ActiveVirtualCamera is CinemachineFreeLook freeLookCamera) freeLookCamera.m_XAxis.m_MaxSpeed = value;
				
				// virtual camera
				if (this._cinemachineBrain.ActiveVirtualCamera is CinemachineVirtualCamera virtualCamera) {
					var pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
					if (pov == null) return;
					pov.m_HorizontalAxis.m_MaxSpeed = value;
				}
			});
			CSave.getRx.ObserveEveryValueChanged(p=>p.Value.CameraSensitivity.y).TakeUntilDisable(this).Subscribe(value => {
				// freelook camera
				if (this._cinemachineBrain.ActiveVirtualCamera is CinemachineFreeLook freeLookCamera) freeLookCamera.m_YAxis.m_MaxSpeed = value;
				
				// virtual camera
				if (this._cinemachineBrain.ActiveVirtualCamera is CinemachineVirtualCamera virtualCamera) {
					var pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
					if (pov == null) return;
					pov.m_VerticalAxis.m_MaxSpeed = value;
				}
			});
			
			// active camera changed
			this._cinemachineBrain.m_CameraActivatedEvent.AddListener(this.ActiveCameraChanged);
			#endif

            CTime.OnTimeScaleChanged += this.TimeScaleChanged;
            
			SceneManager.activeSceneChanged += ActiveSceneChanged;
		}
		
		private void UnsubscribeToEvents() {
			#if Cinemachine
			this._cinemachineBrain.m_CameraActivatedEvent.RemoveListener(this.ActiveCameraChanged);
			#endif
			SceneManager.activeSceneChanged -= ActiveSceneChanged;
		}
		
		#if Cinemachine
		private void ActiveCameraChanged(ICinemachineCamera newCamera, ICinemachineCamera oldCamera) {
			if (newCamera == null) return;
			this.UpdateCameraTargets();
			this.ApplyLastOrDefaultCameraProfile();
			if (this._cinemachineBrain.ActiveVirtualCamera is CinemachineFreeLook freeLookCamera) {
				freeLookCamera.m_YAxis.Value = 0.5f;
			}
			Debug.Log($"Player Cinemachine Active Camera Changed to '{newCamera.Name}'", newCamera.VirtualCameraGameObject);
		}
		#endif

		#endregion <<---------- Events ---------->>
		
		
		
		
		#region <<---------- Callbacks ---------->>

		private void ActiveSceneChanged(Scene oldScene, Scene newScene) {
			this.SearchForGlobalVolume();
			this.ApplyLastOrDefaultCameraProfile();
			this.UpdateCameraTargets();
		}

        private void TimeScaleChanged(float oldTime, float newTime) {
            if (this._cinemachineBrain != null) this._cinemachineBrain.enabled = newTime != 0f;
        }
		
		#endregion <<---------- Callbacks ---------->>
		
		
		

		#region <<---------- Input ---------->>

		public void ResetRotation(float duration = 0.3f) {
			#if Cinemachine && DOTween
			if (this._recenterRotationTween != null ) return;
			this._recenterRotationTween?.Kill();

			if (this._cinemachineBrain.ActiveVirtualCamera is CinemachineFreeLook freeLookCamera) {
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
			#else
			Debug.LogError("'Camera ResetRotation' Not implemented without Cinemachine and Dotween");
			#endif
		}
		
		#endregion <<---------- Input ---------->>



		
		#region <<---------- Camera Transition ---------->>

		#if DOTween
		
		public void StartTransition(float duration, CameraTransitionType transitionType) {
			this._tween?.Complete();
			this._tween?.Kill();

			switch (transitionType) {
				case CameraTransitionType.fade:
					print($"[CPlayerCamera] Start fade transition with duration {duration}.");
					this._fader.FadeToBlack(duration, false);
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
				
			}

			this._blockingEventsManager.IsPlayingCutscene = false;
			this._tween.Play();
		}

		#endif

		#endregion <<---------- Camera Transition ---------->>




		#region <<---------- Camera Area and Profiles ---------->>

		private void SearchForGlobalVolume() {
			var globalVolume = FindObjectsOfType<CCameraProfileVolume>().FirstOrDefault(s => s.IsGlobal);
			if (globalVolume != null) {
				this.ActiveCameraProfiles.Insert(0, globalVolume);
			}
		}
		
		public void EnteredCameraArea(CCameraProfileVolume cameraProfile) {
			ActiveCameraProfiles.Add(cameraProfile);
			this.ApplyLastOrDefaultCameraProfile();
		}

		public void ExitedCameraArea(CCameraProfileVolume cameraProfile) {
			ActiveCameraProfiles.Remove(cameraProfile);
			this.ApplyLastOrDefaultCameraProfile();
		}

		private void ApplyLastOrDefaultCameraProfile() {
			this.ActiveCameraProfiles.RemoveAll(i => i == null);
			var lastOrDefaultProfile = this.ActiveCameraProfiles.LastOrDefault();
			EnableCameraFromType(lastOrDefaultProfile != null ? lastOrDefaultProfile.CameraType : CameraType.@default);
		}

		private void EnableCameraFromType(CameraType cameraType) {
			int typeIndex = cameraType.CToInt();
			for (int i = 0; i < this._cinemachineCameras.Length; i++) {
				this._cinemachineCameras[i].gameObject.SetActive(i == typeIndex);
			}
		}
		
		#endregion <<---------- Camera Area and Profiles ---------->>
		
	}
}

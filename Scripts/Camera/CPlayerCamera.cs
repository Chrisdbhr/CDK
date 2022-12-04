using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

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
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		

		#region <<---------- Enums ---------->>

		public enum CameraTransitionType {
			fade,
		}
		
		public enum CameraType {
			@default,
			smallAreaCloseLimitedVision,
            twoDCamera
        }

		#endregion <<---------- Enums ---------->>
		
		
		
		
		#region <<---------- Properties and Fields ---------->>
		
        private readonly Vector2 DefaultMaxCameraSensitivity = new Vector2(64f * 2f, 0.56f * 2f);
        
		// Rotation
		public float RotationX { get; private set; }
		public float RotationY { get; private set; }
        private Coroutine _recenterRotationRoutine;
        private float _recenterRotationSpeed = 0.15f;
		
		// Camera 
		[SerializeField] private UnityEngine.Camera _unityCamera;
		#if Cinemachine
		[SerializeField] private CinemachineBrain _cinemachineBrain;
		[SerializeField] private CinemachineVirtualCameraBase[] _cinemachineCameras;
		#endif
		private int _currentGameFrame;

		[SerializeField] private SkinnedMeshRenderer[] _renderToHideWhenCameraIsClose;
		private float _currentDistanceFromTarget = 10.0f;
		private float _distanceToConsiderCloseForCharacter = 0.5f;
		private ReactiveProperty<bool> _isCloseToTheCharacterRx;
		private float _clampMaxDistanceSpeed = 3f;

		// Cache
		private CGamePlayer _ownerPlayer;
		private Transform _transform;
		private CCharacter_Base _ownerCharacter;
		
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

            this._cinemachineBrain.m_IgnoreTimeScale = false;
            
			this.SearchForGlobalVolume();
			this.ApplyLastOrDefaultCameraProfile();
		}

		private void OnEnable() {
			var angles = this._transform.eulerAngles;
			this.RotationY = angles.y;
			this.RotationX = angles.x;
			
			// is close to the character?
            this._isCloseToTheCharacterRx.TakeUntilDisable(this).Subscribe(isClose => {
				if (this._renderToHideWhenCameraIsClose.Length <= 0) return;
				// disable renderers
				foreach (var objToDisable in this._renderToHideWhenCameraIsClose) {
					if (objToDisable == null) continue;
					objToDisable.enabled = !isClose;
				}
			});
			
			#if Cinemachine

            // camera sensitivity changed
            Observable.EveryGameObjectUpdate().TakeUntilDisable(this).Subscribe(_ => {
                SetCameraSensitivity(CPlayerPrefs.Current.CameraSensitivityMultiplier);
            });
			
            // active camera changed
            this._cinemachineBrain.m_CameraActivatedEvent.AddListener(this.ActiveCameraChanged);
			#endif

            SceneManager.activeSceneChanged += ActiveSceneChanged;
        }

        private void LateUpdate() {
            if (CApplication.IsQuitting) return;
            if (this._cinemachineBrain == null || this._cinemachineBrain.ActiveVirtualCamera == null || this._cinemachineBrain.ActiveVirtualCamera.Follow == null) return;
            this.UpdateDistanceFromTarget();
        }

        private void OnDisable() {
			#if Cinemachine
            this._cinemachineBrain.m_CameraActivatedEvent.RemoveListener(this.ActiveCameraChanged);
			#endif
            SceneManager.activeSceneChanged -= ActiveSceneChanged;
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

        private void UpdateDistanceFromTarget() {
            this._currentDistanceFromTarget = Vector3.Distance(this._cinemachineBrain.ActiveVirtualCamera.Follow.position, this._unityCamera.transform.position); 
            this._isCloseToTheCharacterRx.Value = this._currentDistanceFromTarget <= this._distanceToConsiderCloseForCharacter;
        }
        
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
		

		

        #region <<---------- Callbacks ---------->>

		#if Cinemachine
		private void ActiveCameraChanged(ICinemachineCamera newCamera, ICinemachineCamera oldCamera) {
			if (newCamera == null) return;
			this.UpdateCameraTargets();
			this.ApplyLastOrDefaultCameraProfile();
			if (this._cinemachineBrain.ActiveVirtualCamera is CinemachineFreeLook freeLookCamera) {
				freeLookCamera.m_YAxis.Value = 0.5f;
                freeLookCamera.m_RecenterToTargetHeading.m_enabled = false;
                freeLookCamera.m_YAxisRecentering.m_enabled = false;
                
                float recenterRotationDuration = this._recenterRotationSpeed * 0.5f;
                freeLookCamera.m_RecenterToTargetHeading.m_RecenteringTime = freeLookCamera.m_YAxisRecentering.m_RecenteringTime = recenterRotationDuration;
                freeLookCamera.m_RecenterToTargetHeading.m_WaitTime = freeLookCamera.m_YAxisRecentering.m_WaitTime = 0f;
            }
			Debug.Log($"Player Cinemachine Active Camera Changed to '{newCamera.Name}'", newCamera.VirtualCameraGameObject);
		}
		#endif

        private void SetCameraSensitivity(Vector2 multiplier) {
            var activeCamera = this._cinemachineBrain.ActiveVirtualCamera;
            if (activeCamera == null) return;
            
            if (!this._cinemachineCameras.Contains(activeCamera)) return;

            float x = multiplier.x * this.DefaultMaxCameraSensitivity.x;
            float y = multiplier.y * this.DefaultMaxCameraSensitivity.y;
            
            // freelook camera
            if (this._cinemachineBrain.ActiveVirtualCamera is CinemachineFreeLook freeLookCamera) {
                freeLookCamera.m_XAxis.m_MaxSpeed = x;
                freeLookCamera.m_YAxis.m_MaxSpeed = y;
                return;
            }

            // virtual camera
            if (this._cinemachineBrain.ActiveVirtualCamera is CinemachineVirtualCamera virtualCamera) {
                var pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
                if (pov == null) return;
                pov.m_HorizontalAxis.m_MaxSpeed = x;
                pov.m_VerticalAxis.m_MaxSpeed = y;
                return;
            }
        }
        
		private void ActiveSceneChanged(Scene oldScene, Scene newScene) {
			this.SearchForGlobalVolume();
			this.ApplyLastOrDefaultCameraProfile();
			this.UpdateCameraTargets();
		}
		
		#endregion <<---------- Callbacks ---------->>
		
		
		

		#region <<---------- Input ---------->>

		public void ResetRotation() {
            this.CStopCoroutine(this._recenterRotationRoutine);
            this._recenterRotationRoutine = this.CStartCoroutine(this.ResetRotationRoutine());
        }

        private IEnumerator ResetRotationRoutine() {
            #if Cinemachine
            if (this._cinemachineBrain.ActiveVirtualCamera is CinemachineFreeLook freeLookCamera) {
                freeLookCamera.m_RecenterToTargetHeading.m_enabled = true;
                freeLookCamera.m_YAxisRecentering.m_enabled = true;

                yield return new WaitForSeconds(this._recenterRotationSpeed);
                
                freeLookCamera.m_RecenterToTargetHeading.m_enabled = false;
                freeLookCamera.m_YAxisRecentering.m_enabled = false;
            }
			#else
			Debug.LogError("'Camera ResetRotation' Not implemented without Cinemachine");
			#endif
            
            yield break;
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

			this._tween.Play();
		}

		#endif

		#endregion <<---------- Camera Transition ---------->>




		#region <<---------- Camera Area and Profiles ---------->>

		public void EnableCameraFromType(CameraType cameraType) {
			#if Cinemachine
			int typeIndex = cameraType.CToInt();
			for (int i = 0; i < this._cinemachineCameras.Length; i++) {
				this._cinemachineCameras[i].gameObject.SetActive(i == typeIndex);
			}
			#endif
		}
        
        public void ApplyLastOrDefaultCameraProfile() {
            this.ActiveCameraProfiles.RemoveAll(i => i == null);
            var lastOrDefaultProfile = this.ActiveCameraProfiles.LastOrDefault();
            this.EnableCameraFromType(lastOrDefaultProfile != null ? lastOrDefaultProfile.CameraType : CPlayerCamera.CameraType.@default);
        }
        
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

		#endregion <<---------- Camera Area and Profiles ---------->>
		
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UnityEngine.Rendering;

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

		public virtual void Initialize(CGamePlayer ownerPlayer) {
			
			// owner player
			if (ownerPlayer == null) {
				Debug.LogError($"Could not create player camera of a null player.");
				return;
			}
			this._ownerPlayer = ownerPlayer;

			// character object
			this._ownerCharacter = this._ownerPlayer.GetControllingCharacter();
			
			// cinemachine
            var lookTarget = this._ownerCharacter.GetComponentInChildren<CCameraLookAndFollowTarget>();
			this.SetCameraTargets(lookTarget != null ? lookTarget.transform : null);
        }
		
		#endregion <<---------- Initializers ---------->>
		
		
		

		#region <<---------- Enums ---------->>

		public enum CameraTransitionType {
			fade,
		}
		
		public enum CameraType {
			@default,
			smallAreaCloseLimitedVision,
            twoDCamera,
            FPS
        }

		#endregion <<---------- Enums ---------->>
		
		
		
		
		#region <<---------- Properties and Fields ---------->>
		
        private readonly Vector2 DefaultMaxCameraSensitivity = new Vector2(64f * 2f, 0.56f * 2f);
        
		// Rotation
		public float RotationX { get; private set; }
		public float RotationY { get; private set; }
        private Coroutine _recenterRotationRoutine;
        private float _recenterRotationSpeed = 0.075f;
		
		// Camera 
        [SerializeField] private UnityEngine.Camera _unityCamera;
		#if Cinemachine
		[SerializeField] private CinemachineBrain _cinemachineBrain;
		[SerializeField] private CinemachineVirtualCameraBase[] _cinemachineCameras;
		#endif
		private int _currentGameFrame;

		private float _currentDistanceFromTarget = 10.0f;
		private float _distanceToConsiderCloseForCharacter = 0.75f;
        private float _clampMaxDistanceSpeed = 3f;
		
        private ReactiveProperty<bool> _isCloseToPlayerRx = new ReactiveProperty<bool>(false);
        private ReactiveProperty<CameraType> _cameraTypeRx = new ReactiveProperty<CameraType>();
        private ReactiveCollection<Renderer> _playerRenderersRx = new ReactiveCollection<Renderer>();

		// Cache
        protected CGamePlayer _ownerPlayer;
        protected Transform _transform;
		protected CCharacter_Base _ownerCharacter;
		
		#if DOTween
		private Tweener _tween;
		#endif
		
		private CFader _fader;
        private CBlockingEventsManager _blockingEventsManager;
		
		// Camera Profiles
		private List<CCameraProfileVolume> ActiveCameraProfiles;
        [SerializeField] private float _parametersLerp = 10f;

        [SerializeField] private bool _resetBrainOnEnable;
        [SerializeField] private bool _recenterEnable;
        
		#endregion <<---------- Properties and Fields ---------->>

		


		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			this._transform = this.transform;
			this._fader = CDependencyResolver.Get<CFader>();
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
			this.ActiveCameraProfiles = new List<CCameraProfileVolume>();

            this._cinemachineBrain.m_IgnoreTimeScale = false;

            this.SetCamerasRecenterDefaultValues();
            
			this.ApplyLastOrDefaultCameraProfile();
        }

        protected virtual void OnEnable() {
			var angles = this._transform.eulerAngles;
			this.RotationY = angles.y;
			this.RotationX = angles.x;

            if(_resetBrainOnEnable) this._cinemachineBrain.enabled = false;

            // Set Renderers Visibility 
            Observable.CombineLatest(
                this._isCloseToPlayerRx.AsObservable(),
                this._cameraTypeRx.AsObservable(),
                this._playerRenderersRx.ObserveCountChanged(),
                (isCloseToPlayer, cameraType, _) => (!isCloseToPlayer && cameraType != CameraType.FPS)
            )
            .TakeUntilDisable(this)
            .Subscribe(SetRenderersVisibility);

			#if Cinemachine

            // camera sensitivity changed
            Observable.EveryGameObjectUpdate().TakeUntilDisable(this).Subscribe(_ => {
                SetCameraSensitivity(CPlayerPrefs.Current.CameraSensitivityMultiplier);
            });
			
            // active camera changed
            this._cinemachineBrain.m_CameraActivatedEvent.AddListener(this.ActiveCameraChanged);
			#endif

            SceneManager.activeSceneChanged += ActiveSceneChanged;
            
            // Next Frame
            Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(_ => {
                if (this == null) return;
                if(this._recenterEnable) this.RecenterCameraFast();
                if (this._resetBrainOnEnable && this._cinemachineBrain != null) {
                    this._cinemachineBrain.enabled = true;
                }
            });
        }

        protected virtual void LateUpdate() {
            if (CApplication.IsQuitting) return;
            if (this._cinemachineBrain == null || this._cinemachineBrain.ActiveVirtualCamera == null) return;
            this.UpdateDistanceFromTarget();
            this.UpdateUnityCameraParameters();
        }

        protected virtual void OnDisable() {
			#if Cinemachine
            this._cinemachineBrain.m_CameraActivatedEvent.RemoveListener(this.ActiveCameraChanged);
			#endif
            SceneManager.activeSceneChanged -= ActiveSceneChanged;
		}

		#if UNITY_EDITOR
        protected virtual void OnDrawGizmos() {
			if (this._unityCamera) {
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere(this.transform.position, this._unityCamera.nearClipPlane);
			}
		}
		#endif

        #endregion <<---------- MonoBehaviour ---------->>
		
		
		

		#region <<---------- General ---------->>

        private void UpdateUnityCameraParameters() {
            if (this._unityCamera == null) return;
            switch (this._cinemachineBrain.ActiveVirtualCamera) {
                case CinemachineFreeLook cFreelook:
                    this._unityCamera.nearClipPlane = this._unityCamera.nearClipPlane.CLerp(cFreelook.m_Lens.NearClipPlane, CTime.DeltaTimeScaled * this._parametersLerp);
                    this._unityCamera.farClipPlane = this._unityCamera.farClipPlane.CLerp(cFreelook.m_Lens.FarClipPlane, CTime.DeltaTimeScaled * this._parametersLerp);
                    this._unityCamera.fieldOfView = this._unityCamera.fieldOfView.CLerp(cFreelook.m_Lens.FieldOfView, CTime.DeltaTimeScaled * this._parametersLerp);
                    break;
                case CinemachineVirtualCamera cVirtual:
                    this._unityCamera.nearClipPlane = this._unityCamera.nearClipPlane.CLerp(cVirtual.m_Lens.NearClipPlane, CTime.DeltaTimeScaled * this._parametersLerp);
                    this._unityCamera.farClipPlane = this._unityCamera.farClipPlane.CLerp(cVirtual.m_Lens.FarClipPlane, CTime.DeltaTimeScaled * this._parametersLerp);
                    this._unityCamera.fieldOfView = this._unityCamera.fieldOfView.CLerp(cVirtual.m_Lens.FieldOfView, CTime.DeltaTimeScaled * this._parametersLerp);
                    break;
            }
        }

        public void FindAndSetDefaultCameraTargets() {
            this.SetCameraTargets(this.FindTarget());
        }

        public void SetCameraTargets(Transform lookAndFollowTarget) {
			#if Cinemachine
            var target = lookAndFollowTarget != null ? lookAndFollowTarget : (this._ownerCharacter != null ? this._ownerCharacter.transform : null);
			foreach (var cam in this._cinemachineCameras) {
				cam.Follow = target;
				cam.LookAt = target;
			}
			#else
			Debug.LogError("'PlayerCamera' will not work without Cinemachine");
			#endif
		}
        
        private Transform FindTarget() {
            if (this._ownerCharacter == null) return null;
            var target = this._ownerCharacter.GetComponentInChildren<CCameraLookAndFollowTarget>();
            return (target != null ? target.transform : null);
        }

        [EasyButtons.Button]
        private void RecenterCameraFast() {
            StartResetingRotation(true);
        }

        #endregion <<---------- General ---------->>




        #region <<---------- Camera Distance ---------->>

        private void UpdateDistanceFromTarget() {
            if (this._cinemachineBrain.ActiveVirtualCamera.Follow == null) return;
            this._currentDistanceFromTarget = Vector3.Distance(this._cinemachineBrain.ActiveVirtualCamera.Follow.position, this._unityCamera.transform.position); 
            this._isCloseToPlayerRx.Value = this._currentDistanceFromTarget <= this._distanceToConsiderCloseForCharacter;
        }
        
        public void UpdateRenderers(Renderer[] renderers) {
            this._playerRenderersRx.Clear();
            foreach (var render in renderers) {
                this._playerRenderersRx.Add(render);
            }
        }
        
        void SetRenderersVisibility(bool render) {
            foreach (var objToDisable in this._playerRenderersRx) {
                if (objToDisable == null) continue;
                objToDisable.shadowCastingMode = render ? ShadowCastingMode.On : ShadowCastingMode.ShadowsOnly;
            }
        }

        #endregion <<---------- Camera Distance ---------->>


        

        #region <<---------- Camera Getters ---------->>

        public Transform GetCameraTransform() {
            return this._unityCamera.transform;
        }

        public float GetCameraFieldOfView() {
            return this._unityCamera.fieldOfView;
        }

        #endregion <<---------- Camera Getters ---------->>


		

        #region <<---------- Callbacks ---------->>

		#if Cinemachine
		private void ActiveCameraChanged(ICinemachineCamera newCamera, ICinemachineCamera oldCamera) {
			if (newCamera == null) return;
            this.FindAndSetDefaultCameraTargets();
			this.ApplyLastOrDefaultCameraProfile();
            
            if (this._cinemachineBrain.ActiveVirtualCamera == null) return;
            
            Debug.Log($"Player Cinemachine Active Camera Changed to '{newCamera.Name}'", newCamera.VirtualCameraGameObject);
		}
		#endif

        private void SetCamerasRecenterDefaultValues() {
            foreach (var cam in this._cinemachineCameras) {
                switch (cam) {
                    case CinemachineFreeLook cFreelook:
                        cFreelook.m_RecenterToTargetHeading.m_enabled = cFreelook.m_YAxisRecentering.m_enabled = false;
                
                        cFreelook.m_RecenterToTargetHeading.m_RecenteringTime = cFreelook.m_YAxisRecentering.m_RecenteringTime = this._recenterRotationSpeed;
                        cFreelook.m_RecenterToTargetHeading.m_WaitTime = cFreelook.m_YAxisRecentering.m_WaitTime = 0f;
                        break;
                    case CinemachineVirtualCamera cVirtual:
                        var pov = cVirtual.GetCinemachineComponent<CinemachinePOV>();
                        if (pov) {
                            pov.m_VerticalRecentering.m_enabled = pov.m_HorizontalRecentering.m_enabled = false;
                            pov.m_VerticalRecentering.m_RecenteringTime = pov.m_HorizontalRecentering.m_RecenteringTime = this._recenterRotationSpeed;
                            pov.m_VerticalRecentering.m_WaitTime = pov.m_HorizontalRecentering.m_WaitTime = 0f;
                        }
                        break;
                }
            }
        }

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
                pov.m_VerticalAxis.m_MaxSpeed = x; // uses X because its better with virtual Cameras
                return;
            }
        }
        
		private void ActiveSceneChanged(Scene oldScene, Scene newScene) {
			this.ApplyLastOrDefaultCameraProfile();
            this.FindAndSetDefaultCameraTargets();
            this.RecenterCameraFast();
        }
		
		#endregion <<---------- Callbacks ---------->>
		
		
		

		#region <<---------- Input ---------->>

		public void StartResetingRotation(bool resetInOneFrame) {
            this.CStopCoroutine(this._recenterRotationRoutine);
            this._recenterRotationRoutine = this.CStartCoroutine(this.ResetRotationRoutine(resetInOneFrame));
        }

        private IEnumerator ResetRotationRoutine(bool resetInOneFrame) {
            #if Cinemachine

            if (!resetInOneFrame) {
                SetCamerasRecenterDefaultValues();
            }
            var resetWaitTime = (resetInOneFrame ? null : new WaitForSeconds(this._recenterRotationSpeed * 2f));
            
            switch (this._cinemachineBrain.ActiveVirtualCamera) {
                case CinemachineFreeLook freeLookCamera:
                    if (resetInOneFrame) {
                        freeLookCamera.m_RecenterToTargetHeading.m_RecenteringTime = 0f;
                    }
                    freeLookCamera.m_RecenterToTargetHeading.m_enabled = true;
                    freeLookCamera.m_YAxisRecentering.m_enabled = true;

                    yield return resetWaitTime;
                
                    freeLookCamera.m_RecenterToTargetHeading.m_enabled = false;
                    freeLookCamera.m_YAxisRecentering.m_enabled = false;
                    break;
                case CinemachineVirtualCamera cVirtual:
                    var pov = cVirtual.GetCinemachineComponent<CinemachinePOV>();
                    if (pov) {
                        if (resetInOneFrame) {
                            pov.m_HorizontalRecentering.m_RecenteringTime = 0f;
                            pov.m_VerticalRecentering.m_RecenteringTime = 0f;
                        }
                        pov.m_VerticalRecentering.m_enabled = pov.m_HorizontalRecentering.m_enabled = true;
                        yield return resetWaitTime;
                        pov.m_VerticalRecentering.m_enabled = pov.m_HorizontalRecentering.m_enabled = false;
                    }
                    break;
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
            this._cameraTypeRx.Value = cameraType;
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

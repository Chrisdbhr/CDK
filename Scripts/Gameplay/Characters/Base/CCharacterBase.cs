using System;
using System.Collections.Generic;
using System.Linq;
using CDK.Characters.Enums;
using CDK.Characters.Interfaces;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if FMOD
using FMODUnity;
#endif

namespace CDK {
	[SelectionBase] 
	public abstract class CCharacterBase : MonoBehaviour, ICStunnable {

		#region <<---------- Properties ---------->>

        [SerializeField] private CMonobehaviourExecutionLoop _updateTime = CMonobehaviourExecutionLoop.Update;
		
        #region <<---------- Debug ---------->>

		[SerializeField] protected bool _debug;

		#endregion <<---------- Debug ---------->>

		#region <<---------- Cache and References ---------->>

		[Header("Cache and References")]
		[SerializeField] protected Animator _animator;

		protected CBlockingEventsManager _blockingEventsManager;
		protected Transform transform;

		public Vector3 Position {
			get {
				return transform.position;
			}
			protected set {
				transform.position = value;
			}
		}

		#endregion <<---------- References ---------->>

		#region <<---------- Input ---------->>
		public Vector2 InputMovementRaw { get; set; }
		public Vector3 InputMovementDirRelativeToCam { get; set; }
        public bool InputWalk { get; set; }
        public bool InputRun { get; set; }
		public bool InputAim { get; set; }
		#endregion <<---------- Input ---------->>

		#region <<---------- Time ---------->>

		protected float TimelineTimescale {
			get {
#if LUDIQ_CHRONOS
				return (this._timeline != null ? this._timeline.timeScale : 1f);
#else
				return 1f;
#endif
			}
		}

		#if LUDIQ_CHRONOS
		[SerializeField] private Chronos.Timeline _timeline;
		#endif
		
		#endregion <<---------- Time ---------->>

		#region <<---------- Movement Properties ---------->>
		protected Vector3 _previousPosition { get; private set; }
        
		public Vector3 MyVelocityXZ {
			get {
				var velocity = this.GetMyVelocity();
				velocity.y = 0f;
				return velocity;
			}
		}

		public Vector3 MovementMomentumXZ = Vector3.zero;
        public Vector3 RootMotionDeltaPosition = Vector3.zero;
        public Vector3 AdditionalMovementFromAnimator = Vector3.zero;
		
        private readonly List<CSceneArea> _activeSceneAreas = new List<CSceneArea>();

		#region <<---------- Run and Walk ---------->>
        [Header("Movement")]
        [SerializeField] protected CMovState _currentMovState;
		public CMovState CurrentMovState {
			get { return this._currentMovState; }
		}

        protected abstract void SetMovementState(CMovState value);

		public float WalkSpeed {
			get { return this._walkSpeed; }
		}

        [SerializeField] [Min(0f)] private float _walkSpeed = 0.6f;
		public float RunSpeed {
			get { return this._runSpeed; }
		}
        [SerializeField] [Min(0f)] private float _runSpeed = 1.5f;

        public float SprintSpeed {
            get { return this._sprintSpeed; }
        }
        [SerializeField] [Min(0f)] private float _sprintSpeed = 3f;

		public ReactiveProperty<bool> CanMoveRx { get; protected set; }

        #endregion <<---------- Run and Walk ---------->>
        
		#region <<---------- Rotation ---------->>
        [Header("Rotation")]
        [SerializeField] private AnimationCurve _curveRotationRateOverSpeed = AnimationCurve.Linear(0f,666f,10f,1f);

		protected Quaternion _targetLookRotation;
		#endregion <<---------- Rotation ---------->>
		#endregion <<---------- Movement Properties ---------->>

		#region <<---------- Strafe ---------->>
		protected ReactiveProperty<bool> IsStrafingRx;
		#endregion <<---------- Strafe ---------->>

		#region <<---------- Aim ---------->>
		public bool IsAiming {
			get { return this._isAimingRx.Value; }
		}

		protected ReactiveProperty<bool> _isAimingRx;
		protected Vector3 _aimTargetPos;
		protected Vector3 _aimTargetDirection;
		#endregion <<---------- Aim ---------->>

		#region <<---------- Animation ---------->>
		public const float ANIMATION_BLENDTREE_LERP = 15f;

		#region <<---------- Animation Parameters ---------->>
		protected readonly int ANIM_CHAR_MOV_SPEED_XZ = Animator.StringToHash("speedXZ");
        protected readonly int ANIM_CHAR_MOV_SPEED_X = Animator.StringToHash("speedX");
        protected readonly int ANIM_CHAR_MOV_SPEED_Y = Animator.StringToHash("speedY");
		protected readonly int ANIM_DISTANCE_ON_FREE_FALL = Animator.StringToHash("distanceOnFreeFall");
		protected readonly int ANIM_FALL_LANDING_ANIM_INDEX = Animator.StringToHash("fallLandingAnim");

		// actions
        protected readonly int ANIM_CHAR_STUMBLE = Animator.StringToHash("stumble");

		// condition states
		protected readonly int ANIM_CHAR_IS_STRAFING = Animator.StringToHash("isStrafing");
		protected readonly int ANIM_CHAR_IS_SLIDING = Animator.StringToHash("isSliding");
		protected readonly int ANIM_CHAR_IS_WALKING = Animator.StringToHash("isWalking");
        protected readonly int ANIM_CHAR_IS_RUNNING = Animator.StringToHash("isRunning");
        protected readonly int ANIM_CHAR_IS_SPRINTING = Animator.StringToHash("isSprinting");
		protected readonly int ANIM_CHAR_IS_FALLING = Animator.StringToHash("isFalling");

		// Stun
		protected readonly int ANIM_CHAR_IS_STUNNED_LIGHT = Animator.StringToHash("stunL");
		protected readonly int ANIM_CHAR_IS_STUNNED_MEDIUM = Animator.StringToHash("stunM");
		protected readonly int ANIM_CHAR_IS_STUNNED_HEAVY = Animator.StringToHash("stunH");
		#endregion <<---------- Animation Parameters ---------->>

		#endregion <<---------- Animation ---------->>

		#region <<---------- Sound ---------->>

		#if FMOD
		[Header("Sound")]
		[SerializeField] protected StudioEventEmitter _mounthVoiceEmitter;
		#endif

		#endregion <<---------- Sound ---------->>

		#region <<---------- Observables ---------->>

		private CompositeDisposable _disposables;

		#endregion <<---------- Observables ---------->>

		#endregion <<---------- Properties ---------->>




		#region <<---------- MonoBehaviour ---------->>
		protected virtual void Awake() {
			this.transform = base.transform;
            this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
        }

		protected virtual void OnEnable() {
			this.SubscribeToEvents();
		}

		protected virtual void Start() { }

		protected virtual void Update() {
            if (this._updateTime != CMonobehaviourExecutionLoop.Update) return;
            UpdateCharacter();
        }

		protected virtual void LateUpdate() { }

        protected virtual void FixedUpdate() {
            if (this._updateTime != CMonobehaviourExecutionLoop.FixedUpdate) return;
            UpdateCharacter();
        }
        
		protected virtual void OnDisable() {
			this.UnsubscribeToEvents();
		}

		protected virtual void OnDestroy() {
			this.StopTalking();
		}

        #if UNITY_EDITOR
        protected virtual void Reset() {
            if (this._animator == null) this.GetComponentInChildren<Animator>();
        }
        #endif
        
		#endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- Update ---------->>

        private void UpdateCharacter() {
            this.UpdateIfIsGrounded();

            //this.ProcessSlide(); // disabled until bugs are fixed.
            this.ProcessMovement();
            this.ProcessRotation();
            this.ProcessAim();

            this._previousPosition = this.Position;
        }
        
        #endregion <<---------- Update ---------->>
        


		#region <<---------- Events ---------->>
		protected virtual void SubscribeToEvents() {
			this._disposables?.Dispose();
			this._disposables = new CompositeDisposable();

			#region <<---------- RX Creations ---------->>

			this.CanMoveRx = new ReactiveProperty<bool>(true);
			
			this._isAimingRx = new ReactiveProperty<bool>();
			this.IsStrafingRx = new ReactiveProperty<bool>();

			#endregion <<---------- RX Creations ---------->>

			#region <<---------- Movement ---------->>

			// strafe
			this.IsStrafingRx.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(isStrafing => {
				if (this._animator) this._animator.SetBool(this.ANIM_CHAR_IS_STRAFING, isStrafing);
			});

			// can mov
			this.CanMoveRx.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(canMove => {
				if (!canMove && this.CurrentMovState > CMovState.Idle) {
					this.SetMovementState(CMovState.Idle);
				}
			});

			#endregion <<---------- Movement ---------->>

			// events
			this._blockingEventsManager.OnDoingBlockingAction += this.DoingBlockingAction;
			SceneManager.activeSceneChanged += this.OnActiveSceneChanged;

		}

		protected virtual void UnsubscribeToEvents() {
			this._disposables?.Dispose();
			this._disposables = null;

			this._blockingEventsManager.OnDoingBlockingAction -= this.DoingBlockingAction;
			SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
		}

		protected void DoingBlockingAction(bool isDoing) {
			this.CanMoveRx.Value = !isDoing;
		}

        protected virtual void OnActiveSceneChanged(Scene oldScene, Scene newScene) {
            if (this == null) return;
            this.StopTalking();
            this.ResetFallCalculation();

            this.RootMotionDeltaPosition = Vector3.zero;
            this.AdditionalMovementFromAnimator = Vector3.zero;

            this._animator.CDoIfNotNull(a => {
                a.Rebind();
                a.Update(0f);
            });
            this._blockingEventsManager.ReleaseFromUnityObject(this);

            if (this._activeSceneAreas.RemoveAll(a => a == null) > 0) {
                Debug.Log($"Removed null scene areas when scene changed for character '{this.name}'");
            }
        }

		#endregion <<---------- Events ---------->>




		#region <<---------- Movement ---------->>

        protected abstract void ProcessMovement();

        protected abstract void UpdateIfIsGrounded();

        public abstract Vector3 GetMyVelocity();

        public float GetMyVelocityMagnitude() {
            return this.GetMyVelocity().magnitude;
        }

		#endregion <<---------- Movement ---------->>


        

        #region <<---------- Scene Areas ---------->>
        
        public void AddSceneArea(CSceneArea area) {
            this._activeSceneAreas.Add(area);
        }
		
        public void RemoveSceneArea(CSceneArea area) {
            this._activeSceneAreas.Remove(area);
        }

        protected CMovState GetMaxMovementSpeed() {
            return this._activeSceneAreas.Count > 0 
                ? this._activeSceneAreas.Min(a => a.MaximumMovementState) 
                : CMovState.Sprint;
        }

        protected float GetSpeedForCurrentMovementState() {
            switch (this.CurrentMovState) {
                case CMovState.Sprint:
                    return this.SprintSpeed;
                case CMovState.Running:
                    return this.RunSpeed;
            }
            return this.WalkSpeed;
        }
        
        #endregion <<---------- Scene Areas ---------->>

        


        #region <<---------- Fail ---------->>

        protected abstract void ResetFallCalculation();
        
        #endregion <<---------- Fail ---------->>

		
		
		
		#region <<---------- Rotation ---------->>

        protected abstract void ProcessRotation();
		
		protected void RotateTowardsDirection(Vector3 dir) {
			dir.y = 0f;
			if (dir == Vector3.zero) return;
			this._targetLookRotation = Quaternion.LookRotation(dir);

			var rotateSpeed = this._curveRotationRateOverSpeed.Evaluate(this.GetMyVelocityMagnitude());

            // lerp rotation
			this.transform.rotation = Quaternion.RotateTowards(
													this.transform.rotation,
													this._targetLookRotation,
													rotateSpeed * CTime.DeltaTimeScaled * this.TimelineTimescale);
		}
        
        #endregion <<---------- Rotation ---------->>

		
		

		#region <<---------- Aim ---------->>

		protected void ProcessAim() {
			this._isAimingRx.Value = this.InputAim;
		}
		
		public void SetAimTargetPosition(Vector3 targetPos) {
			this._aimTargetPos = targetPos;
		}

		public void SetAimDirection(Vector3 dir) {
			this._aimTargetDirection = dir;
		}

		#endregion <<---------- Aim ---------->>




		#region <<---------- Crouch ---------->>

		public void ToggleCrouch() {
//			var oldCrouchState = this.IsCrouched;
			
			// if (this.IsCrouched) {
			// 	// get up
			// 	var transformUp = this._myTransform.up;
			// 	var origin = this._myTransform.position + (transformUp * this.charController.height);
			// 	var collidersWithSelf = Physics.SphereCastAll(
			// 		origin,
			// 		this.charController.radius,
			// 		transformUp.normalized,
			// 		this.charController.height,
			// 		1
			// 	).Select(x => x.collider);
			//
			// 	var allSelfColliders = this.transform.root.GetComponentsInChildren<Collider>();
			//
			// 	var colliders = collidersWithSelf.Except(allSelfColliders).ToArray();
			//
			// 	if (colliders.Length > 0) {
			// 		try {
			// 			var sb = new StringBuilder();
			// 			sb.AppendLine($"{this.name} cant get up, head collided with {colliders.Length} objects:");
			// 			foreach (var col in colliders) {
			// 				sb.AppendLine($"{col.name}");
			// 			}
			// 			Debug.Log(sb.ToString());
			// 		} catch (Exception e) {
			// 			Console.WriteLine(e);
			// 		}
			// 		return;
			// 	}
			// }
			
			// // get down
			// this.charController.height = this._charInitialHeight * (oldCrouchState ? 1f : 0.5f);  
			//
			// var center = this.charController.center;
			// center.y = this.charController.height * 0.5f;
			// this.charController.center = center;
			// 	
			// this._isCrouched.Value = !oldCrouchState;
		}
		
		#endregion <<---------- Crouch ---------->>

		
		

		#region <<---------- Stumble ---------->>

		public void Stumble() {
			if(this._animator) this._animator.SetTrigger(this.ANIM_CHAR_STUMBLE);
		}
		
		#endregion <<---------- Stumble ---------->>
		
		


		#region <<---------- ICStunnable ---------->>
		
		public void Stun(CEnumStunType stunType) {
			switch (stunType) {
				case CEnumStunType.medium:
					if(this._animator) this._animator.SetTrigger(this.ANIM_CHAR_IS_STUNNED_MEDIUM);
					break;
				case CEnumStunType.heavy:
					if(this._animator) this._animator.SetTrigger(this.ANIM_CHAR_IS_STUNNED_HEAVY);
					break;
				default:
					if(this._animator) this._animator.SetTrigger(this.ANIM_CHAR_IS_STUNNED_LIGHT);
					break;
			}
		}
		
		#endregion <<---------- ICStunnable ---------->>


		
		
		#region <<---------- Voice ---------->>

		protected virtual void StopTalking() {
			
			#if FMOD
			if (this._mounthVoiceEmitter == null) return;
			if (this._mounthVoiceEmitter.IsPlaying()) {
				this._mounthVoiceEmitter.AllowFadeout = false;
				this._mounthVoiceEmitter.Stop();
			}
			#else
			Debug.LogError("Character StopTalking() not implemented without FMOD");
			#endif
		}

		#endregion <<---------- Voice ---------->>




		#region <<---------- Transform ---------->>

		public void TeleportToLocation(Vector3 targetPos, Quaternion targetRotation = default) {
			if (targetRotation != default) {
                transform.rotation = targetRotation;
			}
			this._previousPosition = targetPos;
			this.Position = targetPos;
			this.ResetFallCalculation();
		}
		
		#endregion <<---------- Transform ---------->>
		
		

		#region <<---------- Animations State Machine Behaviours ---------->>
		
		public void SetAnimationRootMotion(bool state) {
			if(this._animator) this._animator.applyRootMotion = state;
		}
		
		#endregion <<---------- Animations State Machine Behaviours ---------->>
		
	}
}

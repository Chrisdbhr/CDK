using System;
using CDK.Characters.Enums;
using CDK.Characters.Interfaces;
using FMODUnity;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	[SelectionBase] [RequireComponent(typeof(CharacterController))]
	public abstract class CCharacterBase : MonoBehaviour, ICStunnable {

		#region <<---------- Properties ---------->>

		#region <<---------- Debug ---------->>

		[SerializeField] protected bool _debug;

		#endregion <<---------- Debug ---------->>

		#region <<---------- Cache and References ---------->>

		[Header("Cache and References")]
		[SerializeField] protected Animator Anim;

		[NonSerialized] protected CharacterController _charController;
		[NonSerialized] private float _charInitialHeight;

		#endregion <<---------- References ---------->>

		#region <<---------- Input ---------->>
		public Vector2 InputMovementRaw { get; set; }
		public Vector3 InputMovementDirRelativeToCam { get; set; }
		public bool InputRun { get; set; }
		public bool InputAim { get; set; }
		#endregion <<---------- Input ---------->>

		#region <<---------- Time ---------->>

		protected float TimelineTimescale => (this._timeline ? this._timeline.timeScale : 1f);
		[SerializeField] private Chronos.Timeline _timeline;

		#endregion <<---------- Time ---------->>

		#region <<---------- Movement Properties ---------->>
		public Vector3 Position { get; protected set; }
		protected Vector3 _previousPosition { get; private set; }

		[NonSerialized] protected Vector3 _myVelocity;

		protected Vector3 myVelocityXZ {
			get { return this._myVelocityXZ; }
			set {
				if (this._myVelocityXZ == value) return;
				this._myVelocityXZ = value;
				this.Anim.CSetFloatWithLerp(this.ANIM_CHAR_MOV_SPEED_XZ, this._myVelocityXZ.magnitude, ANIMATION_BLENDTREE_LERP * CTime.DeltaTimeScaled * this.TimelineTimescale);
			}
		}

		[NonSerialized] private Vector3 _myVelocityXZ;

		[NonSerialized] public Vector3 AdditiveMovement = Vector3.zero;


		#region <<---------- Run and Walk ---------->>
		public CMovState CurrentMovState {
			get { return this._currentMovState; }
			protected set {
				if (this._currentMovState == value) return;
				var oldValue = this._currentMovState;
				this._currentMovState = value;

				// sliding
				if (value == CMovState.Sliding) {
					// is sliding now
					this._slideBeginTime = Time.time;
				}
				else if (oldValue == CMovState.Sliding) {
					// was sliding
					if (Time.time >= this._slideBeginTime + this._slideTimeToStumble) {
						if (this.Anim) this.Anim.SetTrigger(this.ANIM_CHAR_STUMBLE);
					}
				}

				// set animators
				if (this.Anim) {
					this.Anim.SetBool(this.ANIM_CHAR_IS_SLIDING, value == CMovState.Sliding);
					this.Anim.SetBool(this.ANIM_CHAR_IS_WALKING, value == CMovState.Walking);
					this.Anim.SetBool(this.ANIM_CHAR_IS_RUNNING, value == CMovState.Running);

					//this.Anim.SetBool(this.ANIM_CHAR_IS_SPRINTING, value == CMovState.Sprint);
				}
			}
		}

		[SerializeField] private CMovState _currentMovState;

		public float MovementSpeed {
			get { return this._movementSpeed; }
		}

		[SerializeField] [Min(0f)] private float _movementSpeed = 1f;

		public float WalkMultiplier {
			get { return this._walkMultiplier; }
		}

		private float _walkMultiplier = 0.6f;

		public float RunSpeedMultiplier {
			get { return this._runSpeedMultiplier; }
		}

		private float _runSpeedMultiplier = 2f;

		public ReactiveProperty<bool> CanMoveRx { get; protected set; }

		public bool CanRun {
			get { return !this.BlockRunFromEnvironment; }
		}

		[NonSerialized] public bool BlockRunFromEnvironment;
		#endregion <<---------- Run and Walk ---------->>

		#region <<---------- Sliding ---------->>

		protected ReactiveProperty<bool> _enableSlideRx;

		[SerializeField] protected float _slideSpeed = 3f;

		private bool CanSlide {
			get { return this._canSlide; }
			set {
				if (this._canSlide == value) return;
				this._canSlide = value;
				this._timeThatCanToggleSlide = Time.realtimeSinceStartup + DELAY_TO_TOGGLE_SLIDE;
			}
		}

		[SerializeField] private bool _canSlide;
		[SerializeField] private float _timeThatCanToggleSlide;

		public virtual float SlideControlAmmount => 0.5f;

		[NonSerialized] protected float _slideTimeToStumble = 1.0f;
		[NonSerialized] protected float _slideBeginTime;

		private const float DELAY_TO_TOGGLE_SLIDE = 0.4f;
		private const float SLIDE_FROM_CHAR_SLOPE_LIMIT_MULTIPLIER = 0.6f;

		#endregion <<---------- Sliding ---------->>

		#region <<---------- Aerial Movement ---------->>
		[NonSerialized] protected Vector3 _groundNormal;
		[NonSerialized] private float _lastPositionYSpeedWasPositive;
		[NonSerialized] protected BoolReactiveProperty _isTouchingTheGroundRx;
		[NonSerialized] protected BoolReactiveProperty _isOnFreeFall;
		[NonSerialized] protected FloatReactiveProperty _distanceOnFreeFall;
		private const float HEIGHT_PERCENTAGE_TO_CONSIDER_FREE_FALL = 0.25f;
		#endregion <<---------- Aerial Movement ---------->>

		#region <<---------- Rotation ---------->>
		[SerializeField] private float _rotateTowardsSpeed = 10f;

		[NonSerialized] protected Quaternion _targetLookRotation;
		#endregion <<---------- Rotation ---------->>
		#endregion <<---------- Movement Properties ---------->>

		#region <<---------- Strafe ---------->>
		[NonSerialized] protected ReactiveProperty<bool> IsStrafingRx;
		#endregion <<---------- Strafe ---------->>

		#region <<---------- Aim ---------->>
		public bool IsAiming {
			get { return this._isAimingRx.Value; }
		}

		[NonSerialized] protected ReactiveProperty<bool> _isAimingRx;
		[NonSerialized] protected Vector3 _aimTargetPos;
		[NonSerialized] protected Vector3 _aimTargetDirection;
		#endregion <<---------- Aim ---------->>

		#region <<---------- Animation ---------->>
		public const float ANIMATION_BLENDTREE_LERP = 15f;

		#region <<---------- Animation Parameters ---------->>
		protected readonly int ANIM_CHAR_MOV_SPEED_XZ = Animator.StringToHash("speedXZ");
		protected readonly int ANIM_TIME_FALLING = Animator.StringToHash("timeFalling");

		// actions
		private readonly int ANIM_CHAR_STUMBLE = Animator.StringToHash("stumble");

		// condition states
		protected readonly int ANIM_CHAR_IS_STRAFING = Animator.StringToHash("isStrafing");
		protected readonly int ANIM_CHAR_IS_SLIDING = Animator.StringToHash("isSliding");
		protected readonly int ANIM_CHAR_IS_WALKING = Animator.StringToHash("isWalking");
		protected readonly int ANIM_CHAR_IS_RUNNING = Animator.StringToHash("isRunning");
		protected readonly int ANIM_CHAR_IS_FALLING = Animator.StringToHash("isFalling");

		// Stun
		protected readonly int ANIM_CHAR_IS_STUNNED_LIGHT = Animator.StringToHash("stunL");
		protected readonly int ANIM_CHAR_IS_STUNNED_MEDIUM = Animator.StringToHash("stunM");
		protected readonly int ANIM_CHAR_IS_STUNNED_HEAVY = Animator.StringToHash("stunH");
		#endregion <<---------- Animation Parameters ---------->>

		#endregion <<---------- Animation ---------->>

		#region <<---------- Sound ---------->>

		[Header("Sound")]
		[SerializeField] protected StudioEventEmitter _mounthVoiceEmitter;

		#endregion <<---------- Sound ---------->>

		#region <<---------- Observables ---------->>

		[NonSerialized] private CompositeDisposable _disposables;

		#endregion <<---------- Observables ---------->>

		#endregion <<---------- Properties ---------->>




		#region <<---------- MonoBehaviour ---------->>
		protected virtual void Awake() {
			this._charController = this.GetComponent<CharacterController>();
			this._charInitialHeight = this._charController.height;

			if (this.Anim && !this.Anim.applyRootMotion) {
				Debug.LogWarning($"{this.name} had an Animator with Root motion disabled, it will be enable because Characters use root motion.", this);
				this.SetAnimationRootMotion(true);
			}
		}

		protected virtual void OnEnable() {
			this.SubscribeToEvents();
		}

		protected virtual void Start() { }

		protected virtual void Update() {
			this.Position = this.transform.position;

			this.CheckIfIsGrounded();

			this._myVelocity = this.Position - this._previousPosition;
			this._myVelocityXZ.x = this._myVelocity.x;
			this._myVelocityXZ.y = 0f;
			this._myVelocityXZ.z = this._myVelocity.z;

			this.ProcessAerialMovement();
			this.ProcessSlide();
			this.ProcessMovement();
			this.ProcessRotation();
			this.ProcessAim();

			this._previousPosition = this.Position;
		}

		protected virtual void LateUpdate() { }

		protected virtual void OnDisable() {
			this.UnsubscribeToEvents();
		}

		protected virtual void OnDestroy() {
			this.StopTalking();
		}

		#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected() {
			if (!this._debug) return;
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this._aimTargetPos, 0.05f);
			Handles.Label(this._aimTargetPos, $"{this.name} aimTargetPos");

			// draw ground check sphere
			if (this._charController == null) this._charController = this.GetComponent<CharacterController>();
			var ray = this.GetGroundCheckRay(this._charController.height * HEIGHT_PERCENTAGE_TO_CONSIDER_FREE_FALL);

			//DebugExtension.DrawCapsule(ray.origin, ray.origin + ray.direction, Color.grey, this._charController.radius);
			DebugExtension.DebugCapsule(ray.origin, ray.origin + ray.direction, Color.grey, this._charController.radius);
		}
		#endif
		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- Events ---------->>
		protected virtual void SubscribeToEvents() {
			this._disposables?.Dispose();
			this._disposables = new CompositeDisposable();

			#region <<---------- RX Creations ---------->>

			this.CanMoveRx = new ReactiveProperty<bool>(true);
			this._enableSlideRx = new ReactiveProperty<bool>(true);

			this._isTouchingTheGroundRx = new BoolReactiveProperty(true);
			this._isOnFreeFall = new BoolReactiveProperty(false);
			this._distanceOnFreeFall = new FloatReactiveProperty();

			this._isAimingRx = new ReactiveProperty<bool>();
			this.IsStrafingRx = new ReactiveProperty<bool>();

			#endregion <<---------- RX Creations ---------->>

			#region <<---------- Movement ---------->>

			// strafe
			this.IsStrafingRx.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(isStrafing => {
				if (this.Anim) this.Anim.SetBool(this.ANIM_CHAR_IS_STRAFING, isStrafing);
			});

			// can slide
			this._enableSlideRx.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(canSlide => {
				// stopped sliding
				if (!canSlide && this.CurrentMovState == CMovState.Sliding) {
					this.CurrentMovState = CMovState.Walking;
				}
			});

			// can mov
			this.CanMoveRx.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(canMove => {
				if (!canMove && this.CurrentMovState > CMovState.Idle) {
					this.CurrentMovState = CMovState.Idle;
				}
			});

			#endregion <<---------- Movement ---------->>

			#region <<---------- Fall ---------->>

			Observable.EveryUpdate().Subscribe(_ => {
				if (this._isTouchingTheGroundRx.Value || this._myVelocity.y >= 0f) {
					// not falling
					this._lastPositionYSpeedWasPositive = this.Position.y;
					this._isOnFreeFall.Value = false;
					return;
				}
				
				// is falling
				this._isOnFreeFall.Value = true;
				this._distanceOnFreeFall.Value = this._lastPositionYSpeedWasPositive.CAbs() - this.Position.y.CAbs();
			}).AddTo(this._disposables);

			this._isOnFreeFall.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(isFallingNow => {
				this.Anim.CSetBoolSafe(this.ANIM_CHAR_IS_FALLING, isFallingNow);
				this._distanceOnFreeFall.Value = 0f;
			});

			this._distanceOnFreeFall.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(timeFalling => {
				this.Anim.CSetFloatSafe(this.ANIM_TIME_FALLING, timeFalling);
			});

			#endregion <<---------- Fall ---------->>


			// events
			CBlockingEventsManager.OnDoingBlockingAction += this.DoingBlockingAction;
			CTime.OnTimeScaleChanged += this.TimeScaleChangedEvent;
			SceneManager.activeSceneChanged += this.OnActiveSceneChanged;

		}

		protected virtual void UnsubscribeToEvents() {
			this._disposables?.Dispose();
			this._disposables = null;

			CBlockingEventsManager.OnDoingBlockingAction -= this.DoingBlockingAction;
			CTime.OnTimeScaleChanged -= this.TimeScaleChangedEvent;
			SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
		}

		protected void DoingBlockingAction(bool isDoing) {
			this.CanMoveRx.Value = !isDoing;
		}

		protected void TimeScaleChangedEvent(float newTimeScale) {
			if (this.Anim) this.Anim.applyRootMotion = newTimeScale != 0f;
		}

		protected void OnActiveSceneChanged(Scene oldScene, Scene newScene) {
			if (this == null) return;
			this.StopTalking();
			this._distanceOnFreeFall.Value = 0f;
		}

		#endregion <<---------- Events ---------->>




		#region <<---------- Movement ---------->>

		protected void ProcessMovement() {
			if (!this._charController.enabled) return;

			var targetMotion = this.ProcessHorizontalMovement();
			targetMotion.y = this.ProcessVerticalMovement();
			targetMotion *= CTime.DeltaTimeScaled * this.TimelineTimescale;
			if (targetMotion == Vector3.zero) return;
			this._charController.Move(targetMotion);

		}

		private Vector3 ProcessHorizontalMovement() {
			Vector3 targetMotion = this.InputMovementDirRelativeToCam;
			float targetMovSpeed = 0f;

			// manual movement
			if (this.CanMoveRx.Value
				&& this.CurrentMovState != CMovState.Sliding
				&& !CBlockingEventsManager.IsDoingBlockingAction.IsRetained()
			) {

				// input movement
				if (targetMotion != Vector3.zero) {
					if (this.InputRun && this.CanRun && !this._isAimingRx.Value) {
						this.CurrentMovState = CMovState.Running;
					}
					else {
						this.CurrentMovState = CMovState.Walking;
					}
				}
				else {
					this.CurrentMovState = CMovState.Idle;
				}

				// set is strafing
				this.IsStrafingRx.Value = this._isAimingRx.Value && this.CurrentMovState <= CMovState.Walking;

				// target movement speed
				switch (this.CurrentMovState) {
					case CMovState.Walking: {
						targetMovSpeed = this.MovementSpeed > 0f ? this.MovementSpeed * this.WalkMultiplier : this.WalkMultiplier;
						break;
					}
					case CMovState.Running: {
						targetMovSpeed = this.MovementSpeed > 0f ? this.MovementSpeed * this.RunSpeedMultiplier : this.RunSpeedMultiplier;
						break;
					}
					default: {
						targetMovSpeed = this.MovementSpeed;
						break;
					}
				}


				if (this.IsAiming) {
					targetMovSpeed *= 0.5f;
				}

				if (Debug.isDebugBuild && Input.GetKey(KeyCode.RightShift)) {
					targetMovSpeed *= 20f;
				}
			}

			// is sliding
			if (this.CurrentMovState == CMovState.Sliding) {
				targetMotion = this.InputMovementDirRelativeToCam * this.SlideControlAmmount
						+ this.transform.forward + (this._groundNormal * 2f);
				targetMovSpeed = this._slideSpeed;
			}

			if (this.AdditiveMovement != Vector3.zero) {
				targetMotion += this.AdditiveMovement;
				Debug.Log($"Applying additive movement of {this.AdditiveMovement}, targetMotion now is {targetMotion}");
				this.AdditiveMovement = Vector3.zero;
			}

			// move character
			return targetMotion * targetMovSpeed;
		}

		private float ProcessVerticalMovement() {
			return this._myVelocity.y + Physics.gravity.y;
		}

		protected void CheckIfIsGrounded() {
			float heightFraction = this._charController.height * HEIGHT_PERCENTAGE_TO_CONSIDER_FREE_FALL;
			var ray = this.GetGroundCheckRay(heightFraction);
			bool isGrounded = Physics.SphereCast(
				ray.origin,
				this._charController.radius,
				ray.direction,
				out var hitInfo,
				heightFraction,
				1,
				QueryTriggerInteraction.Ignore
			) || this._charController.isGrounded || (this._charController.collisionFlags & CollisionFlags.Below) != 0;
			
			if (isGrounded) {
				this._groundNormal = hitInfo.normal;
			}
			else {
				this._groundNormal = Vector3.up;
			}
			this._isTouchingTheGroundRx.Value = isGrounded;
		}

		protected void ProcessSlide() {
			if (Time.realtimeSinceStartup < this._timeThatCanToggleSlide + DELAY_TO_TOGGLE_SLIDE) return;

			var angleFromGround = Vector3.SignedAngle(Vector3.up, this._groundNormal, this.transform.right);

			this.CanSlide = this._enableSlideRx.Value
					&& this._isTouchingTheGroundRx.Value
					&& this.CurrentMovState >= CMovState.Running
					&& angleFromGround >= this._charController.slopeLimit * SLIDE_FROM_CHAR_SLOPE_LIMIT_MULTIPLIER;

			if (Time.realtimeSinceStartup < this._timeThatCanToggleSlide) return;

			if (this.CanSlide) {
				this.CurrentMovState = CMovState.Sliding;
			}
			else if (this.CurrentMovState == CMovState.Sliding) {
				this.CurrentMovState = CMovState.Walking;
			}
		}

		#endregion <<---------- Movement ---------->>




		#region <<---------- Aerial and Falling ---------->>

		protected virtual void ProcessAerialMovement() { }

		protected (Vector3 origin, Vector3 direction) GetGroundCheckRay(float heightFraction) {
			var radius = this._charController.radius;
			return (this.Position + Vector3.up * (radius + heightFraction * 0.5f),
					Vector3.down * (heightFraction + radius));
		}

		#endregion <<---------- Aerial and Falling ---------->>

		
		
		
		#region <<---------- Rotation ---------->>
		
		protected void ProcessRotation() {
			if (this.IsStrafingRx.Value) {
				this.RotateTowardsDirection(this._aimTargetDirection);
			}
			else {
				if (this.CurrentMovState == CMovState.Sliding) {
					this.RotateTowardsDirection(this._groundNormal + this.myVelocityXZ);
				}
				else {
					if(!CBlockingEventsManager.IsDoingBlockingAction.IsRetained()) this.RotateTowardsDirection(this.InputMovementDirRelativeToCam);
				}
			}
		}
		
		protected void RotateTowardsDirection(Vector3 dir) {
			dir.y = 0f;
			if (dir == Vector3.zero) return;
			this._targetLookRotation = Quaternion.LookRotation(dir);

			var rotateSpeed = this._rotateTowardsSpeed;
			if (this.CurrentMovState >= CMovState.Running) rotateSpeed *= 0.5f;
			
			// lerp rotation
			this.transform.rotation = Quaternion.Lerp(
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
			if(this.Anim) this.Anim.SetTrigger(this.ANIM_CHAR_STUMBLE);
		}
		
		#endregion <<---------- Stumble ---------->>
		
		


		#region <<---------- ICStunnable ---------->>
		
		public void Stun(CEnumStunType stunType) {
			switch (stunType) {
				case CEnumStunType.medium:
					if(this.Anim) this.Anim.SetTrigger(this.ANIM_CHAR_IS_STUNNED_MEDIUM);
					break;
				case CEnumStunType.heavy:
					if(this.Anim) this.Anim.SetTrigger(this.ANIM_CHAR_IS_STUNNED_HEAVY);
					break;
				default:
					if(this.Anim) this.Anim.SetTrigger(this.ANIM_CHAR_IS_STUNNED_LIGHT);
					break;
			}
		}
		
		#endregion <<---------- ICStunnable ---------->>


		
		
		#region <<---------- Voice ---------->>

		protected virtual void StopTalking() {
			if (this._mounthVoiceEmitter == null) return;
			if (this._mounthVoiceEmitter.IsPlaying()) {
				this._mounthVoiceEmitter.AllowFadeout = false;
				this._mounthVoiceEmitter.Stop();
			}
		}

		#endregion <<---------- Voice ---------->>
		
		
		

		#region <<---------- Animations State Machine Behaviours ---------->>
		
		public void SetAnimationRootMotion(bool state) {
			if(this.Anim) this.Anim.applyRootMotion = state;
		}
		
		#endregion <<---------- Animations State Machine Behaviours ---------->>
		
	}
}
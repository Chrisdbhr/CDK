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
		[NonSerialized] protected CBlockingEventsManager _blockingEventsManager;
		[NonSerialized] private float _charInitialHeight;

		#endregion <<---------- References ---------->>

		#region <<---------- Input ---------->>
		public Vector2 InputMovementRaw { get; set; }
		public Vector3 InputMovementDirRelativeToCam { get; set; }
		public bool InputRun { get; set; }
		public bool InputAim { get; set; }
		#endregion <<---------- Input ---------->>

		#region <<---------- Time ---------->>

		protected float TimelineTimescale => (this._timeline != null ? this._timeline.timeScale : 1f);
		
		#if LUDIQ_CHRONOS
		[SerializeField] private Chronos.Timeline _timeline;
		#else
		private object _timeline = null;
		#endif
		
		#endregion <<---------- Time ---------->>

		#region <<---------- Movement Properties ---------->>
		[Obsolete("Its not recomended to access transform directly.")]
		public new Transform transform {
			get {
				return base.transform;
			}
		}
		
		public Vector3 Position { get; protected set; }
		protected Vector3 _previousPosition { get; private set; }

		public Vector3 MyVelocity { get; private set; }
		public float MyVelocityMagnitude { get; private set; }

		public Vector3 MyVelocityXZ {
			get { return this._myVelocityXZ; }
			private set {
				if (this._myVelocityXZ == value) return;
				this._myVelocityXZ = value;
				this.Anim.CSetFloatWithLerp(this.ANIM_CHAR_MOV_SPEED_XZ, this._myVelocityXZ.magnitude, ANIMATION_BLENDTREE_LERP * CTime.DeltaTimeScaled * this.TimelineTimescale);
			}
		}
		[NonSerialized] private Vector3 _myVelocityXZ;

		[NonSerialized] public Vector3 _movementMomentumXZ = Vector3.zero;
		//[NonSerialized] private Vector3 _rootMotionDeltaPosition = Vector3.zero;
		public Vector3 _rootMotionDeltaPosition = Vector3.zero;
		

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
		[SerializeField] private float _gravityMultiplier = 1f;
		[SerializeField] [Range(0f,1f)] private float _aerialMomentumMaintainPercentage = 0.90f;
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
		protected readonly int ANIM_DISTANCE_ON_FREE_FALL = Animator.StringToHash("distanceOnFreeFall");
		protected readonly int ANIM_FALL_LANDING_ANIM_INDEX = Animator.StringToHash("fallLandingAnim");

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
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();

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
			this.Position = base.transform.position;

			this.CheckIfIsGrounded();

			this.MyVelocity = this.Position - this._previousPosition;
			this.MyVelocityMagnitude = this.MyVelocity.magnitude;
			this.MyVelocityXZ = new Vector3(this.MyVelocity.x,0f, this.MyVelocity.z); // new to trigger event

			this.ProcessAerialMovement();
			//this.ProcessSlide(); // disabled until bugs are fixed.
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

		private void OnAnimatorMove() {
			if (CTime.TimeScale == 0f) {
				this._rootMotionDeltaPosition = Vector3.zero;
				return;
			}

			if (this.Anim == null) return;

			this._rootMotionDeltaPosition = this.Anim.deltaPosition;
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
			//DebugExtension.DebugCapsule(ray.origin, ray.origin + ray.direction, Color.grey, this._charController.radius);
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
				if (this._isTouchingTheGroundRx.Value || this.MyVelocity.y >= 0f) {
					// not falling
					this._lastPositionYSpeedWasPositive = this.Position.y;
					this._isOnFreeFall.Value = false;
					return;
				}
				
				// is falling
				this._isOnFreeFall.Value = true;
				this._distanceOnFreeFall.Value = this._lastPositionYSpeedWasPositive.CAbs() - this.Position.y.CAbs();
			}).AddTo(this._disposables);

			this._isTouchingTheGroundRx.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(isTouchingTheGround => {
				if (isTouchingTheGround && this.Anim != null) {
					int fallAnimIndex = 0;
					if (this._distanceOnFreeFall.Value >= 6f) {
						fallAnimIndex = 2;
					}else if (this._distanceOnFreeFall.Value >= 2f) {
						fallAnimIndex = 1;
					}
					this.Anim.SetInteger(ANIM_FALL_LANDING_ANIM_INDEX, fallAnimIndex);
				}
				this._movementMomentumXZ = isTouchingTheGround ? Vector3.zero : this._myVelocityXZ;
			});
			
			this._isOnFreeFall.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(isFallingNow => {
				this.Anim.CSetBoolSafe(this.ANIM_CHAR_IS_FALLING, isFallingNow);
				this._distanceOnFreeFall.Value = 0f;
			});

			this._distanceOnFreeFall.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(timeFalling => {
				this.Anim.CSetFloatSafe(this.ANIM_DISTANCE_ON_FREE_FALL, timeFalling);
			});

			#endregion <<---------- Fall ---------->>


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

		protected void OnActiveSceneChanged(Scene oldScene, Scene newScene) {
			if (this == null) return;
			this.StopTalking();
			this._distanceOnFreeFall.Value = 0f;
		}

		#endregion <<---------- Events ---------->>




		#region <<---------- Movement ---------->>

		protected void ProcessMovement() {
			if (!this._charController.enabled) return;

			if (CTime.TimeScale == 0f) return;
			
			var deltaTime = CTime.DeltaTimeScaled * this.TimelineTimescale;
			
			// horizontal movement
			var targetMotion = this.ProcessHorizontalMovement(deltaTime);
			
			// vertical movement
			targetMotion.y = this.ProcessVerticalMovement(deltaTime);
			
			if (targetMotion == Vector3.zero) return;
			
			this._charController.Move(targetMotion);
		}

		private Vector3 ProcessHorizontalMovement(float deltaTime) {
			Vector3 targetMotion = this.InputMovementDirRelativeToCam;
			float targetMovSpeed = 0f;

			
			// manual movement
			if (this.CanMoveRx.Value && this.CurrentMovState != CMovState.Sliding && !this._blockingEventsManager.IsDoingBlockingAction.IsRetained()) {
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

			// momentum
			if (!this._isTouchingTheGroundRx.Value && (this._movementMomentumXZ.x.CImprecise() != 0.0f || this._movementMomentumXZ.z.CImprecise() != 0.0f)) {
				targetMotion += this._movementMomentumXZ;
				if(this._debug) Debug.Log($"Applying {this._movementMomentumXZ}, targetMotion now is {targetMotion}");
				var momentumLoseValue = this._movementMomentumXZ - (this._movementMomentumXZ * (this._aerialMomentumMaintainPercentage));
				this._movementMomentumXZ -= momentumLoseValue * deltaTime;
			}
			
			// root motion
			var rootMotionDeltaPos = this._rootMotionDeltaPosition;
			rootMotionDeltaPos.y = 0f;

			// move character
			return (targetMotion * (targetMovSpeed * deltaTime)) + rootMotionDeltaPos;
		}


		[Range(-15f,15f)]public float VAI_velocityY;
		[Range(-15f,15f)]public float VAI_rootMotionY;
		private float ProcessVerticalMovement(float deltaTime) {
			VAI_velocityY = this.MyVelocity.y;
			VAI_rootMotionY = this._rootMotionDeltaPosition.y;

			var verticalDelta = this._rootMotionDeltaPosition.y;
			verticalDelta += this.MyVelocity.y > 0f ? 0f : this.MyVelocity.y; // consider only fall velocity
			verticalDelta += (Physics.gravity.y * this._gravityMultiplier) * deltaTime;
			
			return verticalDelta;
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

			var angleFromGround = Vector3.SignedAngle(Vector3.up, this._groundNormal, base.transform.right);

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
					this.RotateTowardsDirection(this._groundNormal + this.MyVelocityXZ);
				}
				else {
					if(!this._blockingEventsManager.IsDoingBlockingAction.IsRetained()) this.RotateTowardsDirection(this.InputMovementDirRelativeToCam);
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




		#region <<---------- Transform ---------->>

		public void TeleportToLocation(Vector3 targetPos, Quaternion targetRotation = default) {
			base.transform.position = targetPos;
			if (targetRotation != default) {
				base.transform.rotation = targetRotation;
			}
			this.MyVelocity = Vector3.zero;
			this.MyVelocityXZ = Vector3.zero;
			this._previousPosition = targetPos;
			this.Position = targetPos;
			Physics.SyncTransforms();
		}
		
		#endregion <<---------- Transform ---------->>
		
		

		#region <<---------- Animations State Machine Behaviours ---------->>
		
		public void SetAnimationRootMotion(bool state) {
			if(this.Anim) this.Anim.applyRootMotion = state;
		}
		
		#endregion <<---------- Animations State Machine Behaviours ---------->>
		
	}
}

using System;
using UniRx;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	[SelectionBase][RequireComponent(typeof(CharacterController))]
	public abstract class CCharacterBase : MonoBehaviour, ICCharacterBase {

		#region <<---------- Properties ---------->>
		#region <<---------- Cache and References ---------->>
		[Header("Cache and References")]
		[SerializeField] public Animator Anim;
		[SerializeField] protected CharacterController charController;
		[NonSerialized] protected Transform _myTransform;
		[NonSerialized] protected CompositeDisposable _compositeDisposable = new CompositeDisposable();
		#endregion <<---------- References ---------->>
		
		#region <<---------- Input ---------->>
		public Vector2 InputDirAbsolute { get; set; }
		public Vector3 InputDirRelativeToCam { get; set; }
		public bool InputSlowWalk { get; set; }
		public bool InputAim { get; set; }
		#endregion <<---------- Input ---------->>

		#region <<---------- Movement Properties ---------->>
		public Vector3 Position { get; protected set; }

		[NonSerialized] protected Vector3 myVelocity;
		[NonSerialized] protected Vector3 myVelocityXZ;

		#region <<---------- Run and Walk ---------->>
		public CMovState CurrentMovState {
			get { return this._currentMovState; }
			protected set {
				if (this._currentMovState == value) return;
				var oldValue = this._currentMovState;
				this._currentMovState = value;

				if (value == CMovState.Sliding) {
					// is sliding now
					this._slideBeginTime = Time.timeSinceLevelLoad;
				}
				else if (oldValue == CMovState.Sliding && value != CMovState.Sliding) {
					// was sliding
					if (Time.timeSinceLevelLoad >= this._slideBeginTime + this._slideTimeToStumble) {
						this.Anim.SetTrigger(this.ANIM_CHAR_STUMBLE);
					}
				}
				
				// set animators
				this.Anim.SetBool(this.ANIM_CHAR_IS_SLIDING, value == CMovState.Sliding);
				this.Anim.SetBool(this.ANIM_CHAR_IS_WALKING, value == CMovState.Walking);
				this.Anim.SetBool(this.ANIM_CHAR_IS_RUNNING, value == CMovState.Running);
			}
		}
		private CMovState _currentMovState;
		public float WalkSpeed {
			get { return this._walkSpeed; }
		}
		[SerializeField] private float _walkSpeed = 2.5f;

		public float RunSpeed {
			get { return this._runSpeed; }
		}
		[SerializeField] private float _runSpeed = 3.5f;
		
		public ReactiveProperty<bool> CanMoveRx { get; protected set; }
		public ReactiveProperty<bool> CanRunRx { get; protected set; }
		public ReactiveProperty<bool> RunningRx { get; protected set; }
		#endregion <<---------- Run and Walk ---------->>
		
		#region <<---------- Sliding ---------->>

		protected ReactiveProperty<bool> _canSlideRx;

		public float SlideSpeed {
			get { return this._slideSpeed; }
		}
		[SerializeField] private float _slideSpeed = 2.5f;
		public virtual float SlideControlAmmount => 0.5f;
		/// <summary>
		/// tempo para tropecar
		/// </summary>
		[NonSerialized] protected float _slideTimeToStumble = 1.5f;
		[NonSerialized] protected float _slideBeginTime;
		
		#endregion <<---------- Sliding ---------->>

		#region <<---------- Aerial Movement ---------->>
		[NonSerialized] protected ReactiveProperty<bool> _isTouchingTheGroundRx;
		[NonSerialized] protected Vector3 _groundNormal;
		#endregion <<---------- Aerial Movement ---------->>
		
		#region <<---------- Rotation ---------->>
		[SerializeField] private float _rotateTowardsSpeed = 5f;
		
		[NonSerialized] protected Quaternion _targetLookRotation;
		[NonSerialized] protected float rotateTowardsLookTargetSpeed = 800f;
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

		// actions
		protected readonly int ANIM_CHAR_STUMBLE = Animator.StringToHash("stumble");
		
		// condition states
		protected readonly int ANIM_CHAR_IS_STRAFING = Animator.StringToHash("isStrafing");
		protected readonly int ANIM_CHAR_IS_SLIDING = Animator.StringToHash("isSliding");
		protected readonly int ANIM_CHAR_IS_WALKING = Animator.StringToHash("isWalking");
		protected readonly int ANIM_CHAR_IS_RUNNING = Animator.StringToHash("isRunning");
		protected readonly int ANIM_CHAR_IS_FALLING = Animator.StringToHash("isFalling");
		protected readonly int ANIM_CHAR_IS_AIMING = Animator.StringToHash("isAiming");
		
		// Stun
		protected readonly int ANIM_CHAR_IS_STUNNED_LIGHT = Animator.StringToHash("stunL");
		protected readonly int ANIM_CHAR_IS_STUNNED_MEDIUM = Animator.StringToHash("stunM");
		protected readonly int ANIM_CHAR_IS_STUNNED_HEAVY = Animator.StringToHash("stunH");
		#endregion <<---------- Animation Parameters ---------->>
		
		/// <summary>
		/// Character cant move during this condition.
		/// </summary>
		[NonSerialized] protected ReactiveProperty<bool> _isPlayingBlockingAnimationRx;
		#endregion <<---------- Animation ---------->>
		#endregion <<---------- Properties ---------->>

		
		

		#region <<---------- MonoBehaviour ---------->>
		protected virtual void Awake() {
			this._myTransform = this.transform;
		}

		protected virtual void OnEnable() {
			this.SubscribeToEvents();
		}

		protected virtual void Update() {
			this.Position = this._myTransform.position;
			this.myVelocity = this.charController.velocity;
			this.myVelocityXZ = this.myVelocity;
			this.myVelocityXZ.y = 0f;
			this.CheckIfIsGrounded();
			this.ProcessSlide();
			this.ProcessMovement();
			this.ProcessRotation();
			this.ProcessAim();
		}

		protected virtual void LateUpdate() {
			
		}

		protected virtual void OnDisable() {
			this.UnsubscribeToEvents();
		}

		protected virtual void OnDestroy() {
			
		}

		#if UNITY_EDITOR
		protected void OnDrawGizmosSelected() {
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this._aimTargetPos, 0.05f);	
			Handles.Label(this._aimTargetPos, $"{this.name} aimTargetPos");
		}
		#endif
		#endregion <<---------- MonoBehaviour ---------->>

		


		#region <<---------- Events ---------->>
		protected virtual void SubscribeToEvents() {
			this._compositeDisposable?.Dispose();
			this._compositeDisposable = new CompositeDisposable();

			// create rx properties
			this.CanMoveRx = new ReactiveProperty<bool>(true);
			this._canSlideRx = new ReactiveProperty<bool>(false);
			this._isTouchingTheGroundRx = new ReactiveProperty<bool>(true);
			this._isPlayingBlockingAnimationRx = new ReactiveProperty<bool>(false);
			this._isAimingRx = new ReactiveProperty<bool>();
			this.IsStrafingRx = new ReactiveProperty<bool>();
			this.RunningRx = new ReactiveProperty<bool>(false);
			this.CanRunRx = new ReactiveProperty<bool>(true);

			// strafe
			this.IsStrafingRx.Subscribe(isStrafing => {
				this.Anim.SetBool(this.ANIM_CHAR_IS_STRAFING, isStrafing);
			}).AddTo(this._compositeDisposable);

			// aiming
			this._isAimingRx.Subscribe(isAiming => { 
				this.Anim.SetBool(this.ANIM_CHAR_IS_AIMING, isAiming);
			}).AddTo(this._compositeDisposable);
			
			// can slide
			this._canSlideRx.Subscribe(canSlide => {
				if (!canSlide && this.CurrentMovState == CMovState.Sliding) {
					this.CurrentMovState = CMovState.Walking;
				}
			}).AddTo(this._compositeDisposable);
			
			// // can mov
			// Observable.CombineLatest(
			// 	this.CanMoveRx, 
			// 	this._isPlayingBlockingAnimation,
			// (canMove, isPlayingBlockingAnimation) 
			// 		=> 
			// 		(canMove && !isPlayingBlockingAnimation))
			// 	.Subscribe(canMove => {
			// 		if (!canMove) {
			// 			this.InputMov = Vector2.zero;
			// 		}
			// 	})
			// .AddTo(this._compositeDisposable);
			
			
			// movement blocking animations playing
			this._isPlayingBlockingAnimationRx.Subscribe(isPlayingBlockingAnimation => {
				this.CanMoveRx.Value = !isPlayingBlockingAnimation;
			}).AddTo(this._compositeDisposable);
			
			// is touching the ground
			this._isTouchingTheGroundRx.DistinctUntilChanged().Subscribe(isTouchingTheGround => {
				this.Anim.SetBool(this.ANIM_CHAR_IS_FALLING, !isTouchingTheGround);
			}).AddTo(this._compositeDisposable);
			
			// can run
			this.CanRunRx.Subscribe(canRun => {
				if (canRun == false && this.CurrentMovState == CMovState.Running) {
					this.CurrentMovState = CMovState.Walking;
				}
			}).AddTo(this._compositeDisposable);
		}

		protected virtual void UnsubscribeToEvents() {
			this._compositeDisposable?.Dispose();
			this._compositeDisposable = null;
		}
		#endregion <<---------- Events ---------->>


		

		#region <<---------- Movement ---------->>
		
		protected void ProcessMovement() {
			if (!this.charController.enabled) return;
			
			// set is strafing
			this.RunningRx.Value = !this.InputSlowWalk && this.CanRunRx.Value && !this._isAimingRx.Value;
			this.IsStrafingRx.Value = this._isAimingRx.Value || !this.RunningRx.Value;
			
			// idle
			if (!this.CanMoveRx.Value
				|| this.InputDirRelativeToCam == Vector3.zero
				&& this.CurrentMovState != CMovState.Sliding
			) {
				this.CurrentMovState = CMovState.Idle;
				this.charController.Move(Physics.gravity * CTime.DeltaTimeScaled);
				return;
			}
			
			// process mov speed
			float targetMovSpeed;
			if (this.CurrentMovState == CMovState.Sliding) {
				targetMovSpeed = this.SlideSpeed;
			}
			else {
				this.CurrentMovState = this.RunningRx.Value ? CMovState.Running : CMovState.Walking;
				if (this._isAimingRx.Value) {
					targetMovSpeed = this.WalkSpeed * 0.5f;
				}
				else {
					targetMovSpeed = this.RunningRx.Value ? this.RunSpeed : this.WalkSpeed;
				}
			}
			
			if (Debug.isDebugBuild && Input.GetKey(KeyCode.RightShift)) {
				targetMovSpeed *= 20f;
			}

			// move the character
			Vector3 targetMotion;
			if (this.CurrentMovState == CMovState.Sliding) {
				targetMotion = this.InputDirRelativeToCam * this.SlideControlAmmount
						+ (new Vector3(this._groundNormal.x, 0f, this._groundNormal.z).normalized);
			}
			else {
				targetMotion = this.InputDirRelativeToCam;
			}
			
			this.charController.Move(
				(targetMotion  * targetMovSpeed + Physics.gravity) 
				* CTime.DeltaTimeScaled );
		}
		
		protected void CheckIfIsGrounded() {
			bool isGrounded = Physics.SphereCast(
				this.Position + new Vector3(0f, 0.5f),
				this.charController.radius,
				Vector3.down,
				out var hitInfo,
				1f,
				CGameSettings.get.WalkableLayers,
				QueryTriggerInteraction.Ignore
			);
			if (isGrounded) {
				this._groundNormal = hitInfo.normal;
			}
			else {
				this._groundNormal = Vector3.up;
			}
			this._isTouchingTheGroundRx.Value = isGrounded;
		}
		
		protected void ProcessSlide() {
			if (
				this._canSlideRx.Value 
				&& this.CurrentMovState != CMovState.Walking
				&& this.CurrentMovState.IsMoving()
				&& this._isTouchingTheGroundRx.Value
				&& this.InputDirRelativeToCam != Vector3.zero 
				&& Vector3.Angle(this.InputDirRelativeToCam, this._groundNormal) <= CGameSettings.ANGLE_TO_BEGIN_SLIDING
			) {
				this.CurrentMovState = CMovState.Sliding;
			}else if (this.CurrentMovState == CMovState.Sliding) {
				this.CurrentMovState = CMovState.Idle;
			}
		}

		#endregion <<---------- Movement ---------->>



		
		#region <<---------- Rotation ---------->>
		
		protected void ProcessRotation() {
			if (this.IsStrafingRx.Value) {
				this._aimTargetDirection.y = 0f;
				this.RotateTowardsDirection(this._aimTargetDirection);
			}
			else {
				this.RotateTowardsDirection(this.InputDirRelativeToCam);
			}
		}
		
		protected void RotateTowardsVelocity() {
			if (!this.CanMoveRx.Value) return;
			var lookRotationTarget = this.charController.velocity;
			lookRotationTarget.y = 0f;
			if (lookRotationTarget == Vector3.zero) return;
			this._myTransform.rotation = Quaternion.RotateTowards(
				this._myTransform.rotation,
				Quaternion.LookRotation(lookRotationTarget),
				this.rotateTowardsLookTargetSpeed * CTime.DeltaTimeScaled);
		}
		
		protected void RotateTowardsDirection(Vector3 dir) {
			if (dir == Vector3.zero) return;
			this._targetLookRotation = Quaternion.LookRotation(dir);
			
			// lerp rotation
			this._myTransform.rotation = Quaternion.Lerp(
				this._myTransform.rotation,
				this._targetLookRotation,
				this._rotateTowardsSpeed * CTime.DeltaTimeScaled);
			
			/*
			 // Rotate towards
			this._myTransform.rotation = Quaternion.RotateTowards(
				this._myTransform.rotation,
				this._targetLookRotation,
				this._rotateTowardsInputSpeed * Time.deltaTime);*/
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
		
		
		

		#region <<---------- Animations State Machine Behaviours ---------->>
		public void StopAndBlockMovement() {
			this.Anim.applyRootMotion = true;
			this._isPlayingBlockingAnimationRx.Value = true;
		}

		public void ReleaseMovement() {
			this.Anim.applyRootMotion = false;
			this._isPlayingBlockingAnimationRx.Value = false;
		}
		#endregion <<---------- Animations State Machine Behaviours ---------->>
		
	}
}
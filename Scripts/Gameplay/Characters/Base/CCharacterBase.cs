using System;
using CDK.Characters.Enums;
using CDK.Characters.Interfaces;
using UniRx;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	[SelectionBase][RequireComponent(typeof(CharacterController))]
	public abstract class CCharacterBase : MonoBehaviour, ICStunnable {

		#region <<---------- Properties ---------->>
		#region <<---------- Cache and References ---------->>
		
		[Header("Cache and References")]
		[SerializeField] protected Animator Anim;
		[SerializeField] protected CharacterController _charController;
		
		[NonSerialized] protected Transform _myTransform;
		[NonSerialized] protected CompositeDisposable _compositeDisposable = new CompositeDisposable();
		[NonSerialized] private float _charInitialHeight;
		
		#endregion <<---------- References ---------->>
		
		#region <<---------- Input ---------->>
		public Vector2 InputMovementRaw { get; set; }
		public Vector3 InputMovementDirRelativeToCam { get; set; }
		public bool InputRun { get; set; }
		public bool InputAim { get; set; }
		#endregion <<---------- Input ---------->>

		#region <<---------- Movement Properties ---------->>
		public Vector3 Position { get; protected set; }
		protected Vector3 _previousPosition { get; private set; }

		[NonSerialized] protected Vector3 myVelocity;

		protected Vector3 myVelocityXZ {
			get {
				return this._myVelocityXZ;
			}
			set {
				if (this._myVelocityXZ == value) return;
				this._myVelocityXZ = value;

				if (this.Anim) {
					var magnitude = this._myVelocityXZ.magnitude.CImprecise();
					var currentFloat = this.Anim.GetFloat(this.ANIM_CHAR_MOV_SPEED_XZ);
					this.Anim.SetFloat(this.ANIM_CHAR_MOV_SPEED_XZ, currentFloat.CLerp(magnitude, ANIMATION_BLENDTREE_LERP * CTime.DeltaTimeScaled));
				}
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

				if (value == CMovState.Sliding) {
					// is sliding now
					this._slideBeginTime = Time.timeSinceLevelLoad;
				}
				else if (oldValue == CMovState.Sliding) {
					// was sliding
					if (Time.timeSinceLevelLoad >= this._slideBeginTime + this._slideTimeToStumble) {
						if(this.Anim) this.Anim.SetTrigger(this.ANIM_CHAR_STUMBLE);
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
		[SerializeField][Min(1f)] private float _movementSpeed = 1f;
		
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

		protected ReactiveProperty<bool> _canSlideRx;

		public float SlideSpeedMultiplier {
			get { return this._slideSpeedMultiplier; }
		}
		[SerializeField] private float _slideSpeedMultiplier = 10f;
		public virtual float SlideControlAmmount => 0.5f;
		/// <summary>
		/// tempo para tropecar
		/// </summary>
		[NonSerialized] protected float _slideTimeToStumble = 1.0f;
		[NonSerialized] protected float _slideBeginTime;
		
		#endregion <<---------- Sliding ---------->>

		#region <<---------- Aerial Movement ---------->>
		[NonSerialized] protected ReactiveProperty<bool> _isTouchingTheGroundRx;
		[NonSerialized] protected Vector3 _groundNormal;
		#endregion <<---------- Aerial Movement ---------->>
		
		#region <<---------- Rotation ---------->>
		[SerializeField] private float _rotateTowardsSpeed = 20f;
		
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
			this._charInitialHeight = this._charController.height;
		}

		protected virtual void OnEnable() {
			this.SubscribeToEvents();
		}

		protected virtual void Update() {
			this.Position = this._myTransform.position;
			
			this.myVelocity = this.Position - this._previousPosition;
			this.myVelocityXZ = new Vector3(this.myVelocity.x, 0f, this.myVelocity.z);

			this._previousPosition = this.Position;
			
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
		protected virtual void OnDrawGizmosSelected() {
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
			this._canSlideRx = new ReactiveProperty<bool>(true);
			this._isTouchingTheGroundRx = new ReactiveProperty<bool>(true);
			this._isPlayingBlockingAnimationRx = new ReactiveProperty<bool>(false);
			this._isAimingRx = new ReactiveProperty<bool>();
			this.IsStrafingRx = new ReactiveProperty<bool>();
			
			// strafe
			this.IsStrafingRx.Subscribe(isStrafing => {
				if(this.Anim) this.Anim.SetBool(this.ANIM_CHAR_IS_STRAFING, isStrafing);
			}).AddTo(this._compositeDisposable);

			// can slide
			this._canSlideRx.Subscribe(canSlide => {
				// stopped sliding
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
				if(this.Anim) this.Anim.SetBool(this.ANIM_CHAR_IS_FALLING, !isTouchingTheGround);
			}).AddTo(this._compositeDisposable);
			
		}

		protected virtual void UnsubscribeToEvents() {
			this._compositeDisposable?.Dispose();
			this._compositeDisposable = null;
		}
		
		#endregion <<---------- Events ---------->>


		

		#region <<---------- Movement ---------->>
		
		protected void ProcessMovement() {
			if (!this._charController.enabled) return;
			
			Vector3 targetMotion = this.InputMovementDirRelativeToCam;
			float targetMovSpeed = 0f;
	
			// manual movement
			if (this.CanMoveRx.Value && this.CurrentMovState != CMovState.Sliding) {
				
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
						targetMovSpeed = this.MovementSpeed * this.WalkMultiplier;
						break;
					}
					case CMovState.Running: {
						targetMovSpeed = this.MovementSpeed * this.RunSpeedMultiplier;
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
						+ (new Vector3(this._groundNormal.x, 0f, this._groundNormal.z).normalized);
				targetMovSpeed = this.MovementSpeed * (this.RunSpeedMultiplier * this.SlideSpeedMultiplier);
			}

			if (this.AdditiveMovement != Vector3.zero) {
				targetMotion += this.AdditiveMovement;
				Debug.Log($"Applying additive movement of {this.AdditiveMovement}, targetMotion now is {targetMotion}");
				this.AdditiveMovement = Vector3.zero;
			}
			
			// move character
			this._charController.Move((targetMotion * targetMovSpeed + Physics.gravity) 
				* CTime.DeltaTimeScaled);
		}
		
		protected void CheckIfIsGrounded() {
			bool isGrounded = Physics.SphereCast(
				this.Position + new Vector3(0f, 0.5f),
				this._charController.radius,
				Vector3.down,
				out var hitInfo,
				1f,
				1,
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

		//public float angleFromGround;
		//public float angleRelativeVelocity;
		[SerializeField] private float _slideAngleFromGround = 20f;
		[SerializeField] private float _angleRelativeVelocity = 70f;
		
		protected void ProcessSlide() {
			var angleFromGround = Vector3.Angle(this._myTransform.up, this._groundNormal);
			var angleRelativeVelocity = Vector3.Angle(this.myVelocityXZ, this._groundNormal);
			if (
				this._canSlideRx.Value 
				&& this.CurrentMovState >= CMovState.Running
				&& this._isTouchingTheGroundRx.Value
				&& angleFromGround >= this._slideAngleFromGround
				&& angleRelativeVelocity <= this._angleRelativeVelocity
			) {
				this.CurrentMovState = CMovState.Sliding;
			}else if (this.CurrentMovState == CMovState.Sliding) {
				this.CurrentMovState = CMovState.Walking;
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
				if (this.CurrentMovState == CMovState.Sliding) {
					this.RotateTowardsDirection(this.myVelocityXZ);
				}
				else {
					if(!this._isPlayingBlockingAnimationRx.Value) this.RotateTowardsDirection(this.InputMovementDirRelativeToCam);
				}
			}
		}
		
		protected void RotateTowardsDirection(Vector3 dir) {
			if (dir == Vector3.zero) return;
			this._targetLookRotation = Quaternion.LookRotation(dir);
			
			// lerp rotation
			this._myTransform.rotation = Quaternion.Lerp(
				this._myTransform.rotation,
				this._targetLookRotation,
				this._rotateTowardsSpeed * CTime.DeltaTimeScaled);
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




		#region <<---------- ICStunnable ---------->>
		
		public void Stun(CEnumStunType stunType) {
			switch (stunType) {
				case CEnumStunType.medium:
					this.Anim.SetTrigger(this.ANIM_CHAR_IS_STUNNED_MEDIUM);
					break;
				case CEnumStunType.heavy:
					this.Anim.SetTrigger(this.ANIM_CHAR_IS_STUNNED_HEAVY);
					break;
				default:
					this.Anim.SetTrigger(this.ANIM_CHAR_IS_STUNNED_LIGHT);
					break;
			}
		}
		
		#endregion <<---------- ICStunnable ---------->>
		
		
		

		#region <<---------- Animations State Machine Behaviours ---------->>
		
		public void SetPlayingBlockingAnimation(bool state) {
			this._isPlayingBlockingAnimationRx.Value = state;
		}
		
		public void SetAnimationRootMotion(bool state) {
			if(this.Anim) this.Anim.applyRootMotion = state;
		}
		
		#endregion <<---------- Animations State Machine Behaviours ---------->>
		
	}
}
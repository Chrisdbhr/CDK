using System;
using UniRx;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace CDK {
    [RequireComponent(typeof(CharacterController))]
	public class CCharacter3D : CCharacterBase {

        #region <<---------- Properties and Fields ---------->>
        
        protected CharacterController _charController;
        
        [Header("Aerial Movement")]
        [SerializeField] private float _gravityMultiplier = 1f;
        [SerializeField] [Range(0f,1f)] private float _aerialMomentumMaintainPercentage = 0.90f;
        private const float HEIGHT_PERCENTAGE_TO_CONSIDER_FREE_FALL = 0.25f;
        protected Vector3 _groundNormal;
        private float _lastYPositionCharWasNotFalling;
        protected BoolReactiveProperty _isTouchingTheGroundRx;
        protected BoolReactiveProperty _isOnFreeFall;
        protected FloatReactiveProperty _distanceOnFreeFall;
        
        private float _charInitialHeight;

        #region <<---------- Sliding ---------->>

        [Header("Sliding")]
        [SerializeField] protected float _slideSpeed = 3f;
        protected ReactiveProperty<bool> _enableSlideRx;

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

        protected float _slideTimeToStumble = 1.0f;
        protected float _slideBeginTime;

        private const float DELAY_TO_TOGGLE_SLIDE = 0.4f;
        private const float SLIDE_FROM_CHAR_SLOPE_LIMIT_MULTIPLIER = 0.6f;

        #endregion <<---------- Sliding ---------->>
        
        #endregion <<---------- Properties and Fields ---------->>


        

        #region <<---------- MonoBehaviour ---------->>
        protected override void Awake() {
            base.Awake();			
            this._charController = this.GetComponent<CharacterController>();
            this._charInitialHeight = this._charController.height;
            
            if (this._animator && !this._animator.applyRootMotion) {
                Debug.LogWarning($"{this.name} had an Animator with Root motion disabled, it will be enable because Characters use root motion.", this);
                this.SetAnimationRootMotion(true);
            }
        }

        protected override void Update() {
            base.Update();			
            this.ProcessAerialAndFallMovement();
            //this.ProcessSlide();
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
        protected override void SubscribeToEvents() {
            base.SubscribeToEvents();
            
            this._enableSlideRx = new ReactiveProperty<bool>(true);
            this._isTouchingTheGroundRx = new BoolReactiveProperty(true);
            this._isOnFreeFall = new BoolReactiveProperty(false);
            this._distanceOnFreeFall = new FloatReactiveProperty();

            
            // can slide
            this._enableSlideRx.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(canSlide => {
                // stopped sliding
                if (!canSlide && this.CurrentMovState == CMovState.Sliding) {
                    this.SetMovementState(CMovState.Walking);
                }
            });
            
            #region <<---------- Fall ---------->>

            this._isTouchingTheGroundRx.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(isTouchingTheGround => {
                if (isTouchingTheGround && this._animator != null) {
                    if(this._debug) Debug.Log($"<color={"#D76787"}>{this.name}</color> touched the ground, velocityY: '{this.GetMyVelocity().y}', {nameof(this._distanceOnFreeFall)}: '{this._distanceOnFreeFall.Value}', {nameof(this._lastYPositionCharWasNotFalling)}: '{this._lastYPositionCharWasNotFalling}'");
                    int fallAnimIndex = 0;
                    if (this._distanceOnFreeFall.Value >= 6f) {
                        fallAnimIndex = 2;
                    }else if (this._distanceOnFreeFall.Value >= 2f) {
                        fallAnimIndex = 1;
                    }
                    this._animator.SetInteger(ANIM_FALL_LANDING_ANIM_INDEX, fallAnimIndex);
                }
                this._movementMomentumXZ = isTouchingTheGround ? Vector3.zero : this.MyVelocityXZ;
            });
			
            this._isOnFreeFall.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(isFallingNow => {
                this._animator.CSetBoolSafe(this.ANIM_CHAR_IS_FALLING, isFallingNow);
                this._distanceOnFreeFall.Value = 0f;
            });

            this._distanceOnFreeFall.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(distanceOnFreeFall => {
                this._animator.CSetFloatSafe(this.ANIM_DISTANCE_ON_FREE_FALL, distanceOnFreeFall);
            });

            #endregion <<---------- Fall ---------->>
        }
        #endregion <<---------- Events ---------->>
        
        
        
        
        #region <<---------- Aerial and Falling ---------->>

        protected override void ResetFallCalculation() {
            this._lastYPositionCharWasNotFalling = this.Position.y;
        }
		
        protected virtual void ProcessAerialAndFallMovement() {
            if (this._isTouchingTheGroundRx.Value || this.GetMyVelocity().y >= 0f
                || this._blockingEventsManager.IsPlayingCutscene // check if is playing cutscene so char dont die when teleport or during cutscene
               ) {
                
                // not falling
                this.ResetFallCalculation();
                this._isOnFreeFall.Value = false;
                return;
            }

            // is falling
            this._isOnFreeFall.Value = true;
            this._distanceOnFreeFall.Value = (this._lastYPositionCharWasNotFalling - this.Position.y).CAbs();
        }
		
        #endregion <<---------- Aerial and Falling ---------->>
        
        
        
        
        #region <<---------- Movement ---------->>
        public override Vector3 GetMyVelocity() {
            return this._charController.velocity;
        }

        protected override void SetMovementState(CMovState value) {
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
                    if (this._animator) this._animator.SetTrigger(this.ANIM_CHAR_STUMBLE);
                }
            }

            // set animators
            if (this._animator) {
                this._animator.SetBool(this.ANIM_CHAR_IS_SLIDING, value == CMovState.Sliding);
                this._animator.SetBool(this.ANIM_CHAR_IS_WALKING, value == CMovState.Walking);
                this._animator.SetBool(this.ANIM_CHAR_IS_RUNNING, value == CMovState.Running);
                this._animator.SetBool(this.ANIM_CHAR_IS_SPRINTING, value == CMovState.Sprint);
            }
        }

        protected override void ProcessMovement() {
			this._animator.CSetFloatWithLerp(this.ANIM_CHAR_MOV_SPEED_XZ, this.MyVelocityXZ.magnitude, ANIMATION_BLENDTREE_LERP * CTime.DeltaTimeScaled * this.TimelineTimescale);
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

		protected Vector3 ProcessHorizontalMovement(float deltaTime) {
			Vector3 targetMotion = this.InputMovementDirRelativeToCam;
			float targetMovSpeed = 0f;

			
			// manual movement
			if (this.CanMoveRx.Value && this.CurrentMovState != CMovState.Sliding && !this._blockingEventsManager.IsDoingBlockingAction.IsRetained()) {
				// input movement
				if (targetMotion != Vector3.zero) {
                    var maxMovSpeed = this.GetMaxMovementSpeed();
                    if (maxMovSpeed <= CMovState.Walking || this._isAimingRx.Value || this.InputWalk){
                        this.SetMovementState(CMovState.Walking);
					} else if(maxMovSpeed < CMovState.Sprint) {
                        this.SetMovementState(this.InputRun ? CMovState.Running : CMovState.Walking);
                    }
                    else {
                        this.SetMovementState(this.InputRun ? CMovState.Sprint : CMovState.Running);
                    }
				}
				else {
					this.SetMovementState(CMovState.Idle);
				}

				// set is strafing
				this.IsStrafingRx.Value = this._isAimingRx.Value && this.CurrentMovState <= CMovState.Walking;

				// target movement speed
                targetMovSpeed = this.GetSpeedForCurrentMovementState();
				if (this.IsAiming) {
                    targetMovSpeed *= 0.5f;
				}

				if (Debug.isDebugBuild && Input.GetKey(KeyCode.RightShift)) {
					targetMovSpeed *= 20f;
				}
			}

			// is sliding
			if (this.CurrentMovState == CMovState.Sliding) {
				targetMotion = (this.InputMovementDirRelativeToCam * this.SlideControlAmmount)
						+ this.transform.forward + (this._groundNormal * 2f);
				targetMovSpeed = this._slideSpeed;
			}

			// momentum
			if (!this._isTouchingTheGroundRx.Value && (this._movementMomentumXZ.x.CImprecise() != 0.0f || this._movementMomentumXZ.z.CImprecise() != 0.0f)) {
				targetMotion += this._movementMomentumXZ;
				if(this._debug) Debug.Log($"Applying {this._movementMomentumXZ}, targetMotion now is {targetMotion}");
				this._movementMomentumXZ *= this._aerialMomentumMaintainPercentage * deltaTime;
			}
			
			// root motion
			var rootMotionDeltaPos = this._rootMotionDeltaPosition;
			rootMotionDeltaPos.y = 0f;

			// move character
            if(this.IsStrafingRx.Value) return (targetMotion * (targetMovSpeed * deltaTime)) + rootMotionDeltaPos;
			return (this.transform.forward * (targetMotion.magnitude * targetMovSpeed * deltaTime)) + rootMotionDeltaPos;
		}

        protected float ProcessVerticalMovement(float deltaTime) {

			var verticalDelta = this._rootMotionDeltaPosition.y;
			verticalDelta += this.GetMyVelocity().y > 0f ? 0f : this.GetMyVelocity().y; // consider only fall velocity
			verticalDelta += Physics.gravity.y * this._gravityMultiplier;
			
			return verticalDelta * deltaTime;
		}

        protected override void UpdateIfIsGrounded() {
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
        
        protected (Vector3 origin, Vector3 direction) GetGroundCheckRay(float heightFraction) {
            var radius = this._charController.radius;
            return (this.Position + Vector3.up * (radius + heightFraction * 0.5f),
                    Vector3.down * (heightFraction + radius));
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
                this.SetMovementState(CMovState.Sliding);
            }
            else if (this.CurrentMovState == CMovState.Sliding) {
                this.SetMovementState(CMovState.Walking);
            }
        }
        
        #endregion <<---------- Movement ---------->>


        

        #region <<---------- Rotation ---------->>
        
        protected override void ProcessRotation() {
            if (this._blockingEventsManager.IsAnyBlockingEventHappening) return;
            if (this.IsStrafingRx.Value) {
                this.RotateTowardsDirection(this._aimTargetDirection);
            }
            else {
                if (this.CurrentMovState == CMovState.Sliding) {
                    this.RotateTowardsDirection(this._groundNormal + this.MyVelocityXZ);
                }
                else {
                    this.RotateTowardsDirection(this.InputMovementDirRelativeToCam);
                }
            }
        }
        
        #endregion <<---------- Rotation ---------->>
        
	}
}

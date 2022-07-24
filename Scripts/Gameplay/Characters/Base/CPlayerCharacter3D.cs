using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
    [RequireComponent(typeof(CharacterController))]
	public class CPlayerCharacter3D : CCharacterBase {

        #region <<---------- Properties and Fields ---------->>
        
        protected CharacterController _charController;
        private float _charInitialHeight;
        protected readonly int ANIM_INPUT_MAGNITUDE = Animator.StringToHash("input magnitude");

        #region <<---------- Aerial ---------->>

        [Header("Aerial Movement")]
        [SerializeField] private float _gravityMultiplier = 0.075f;
        [SerializeField] [Range(0f,1f)] private float _aerialMomentumLoosePercentage = 0.50f;
        [SerializeField] [Range(0f,1f)] private float _airControl = 0.50f;
        private const float HEIGHT_PERCENTAGE_TO_CONSIDER_FREE_FALL = 0.25f;
        protected Vector3 _groundNormal;
        private float _lastYPositionCharWasNotFalling;
        protected BoolReactiveProperty _isTouchingTheGroundRx;
        protected BoolReactiveProperty _isOnFreeFall;
        protected FloatReactiveProperty _distanceOnFreeFall;

        #endregion <<---------- Aerial ---------->>
        
        #region <<---------- Rotation ---------->>
        [Header("Rotation")]
        [SerializeField] private AnimationCurve _curveRotationRateOverSpeed = AnimationCurve.Linear(0f,900f,0.15f,90f);
        protected Quaternion _targetLookRotation;
        #endregion <<---------- Rotation ---------->>

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
            if (this._charController == null) this._charController = this.GetComponent<CharacterController>();
            this._charInitialHeight = this._charController.height;
            
            if (this._animator && !this._animator.applyRootMotion) {
                Debug.LogWarning($"{this.name} had an Animator with Root motion disabled, it will be enable because Characters use root motion.", this);
                this.SetAnimationRootMotion(true);
            }
        }

        protected override void OnEnable() {
            base.OnEnable();
            
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
                    if(this._debug) Debug.Log($"<color={"#D76787"}>{this.name}</color> touched the ground, velocityY: '{this.Velocity.y}', {nameof(this._distanceOnFreeFall)}: '{this._distanceOnFreeFall.Value}', {nameof(this._lastYPositionCharWasNotFalling)}: '{this._lastYPositionCharWasNotFalling}'");
                    int fallAnimIndex = 0;
                    if (this._distanceOnFreeFall.Value >= 6f) {
                        fallAnimIndex = 2;
                    }else if (this._distanceOnFreeFall.Value >= 2f) {
                        fallAnimIndex = 1;
                    }
                    this._animator.SetInteger(ANIM_FALL_LANDING_ANIM_INDEX, fallAnimIndex);
                }
                this.MovementMomentumXZ = isTouchingTheGround ? Vector3.zero : this.GetMyVelocityXZ();
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


        

        #region <<---------- CCharacterBase ---------->>

        protected override void UpdateCharacter() {
            this.UpdateIfIsGrounded();
            this.ProcessAerialAndFallMovement();
            base.UpdateCharacter();
            this.ProcessRotation();
            this.ProcessAim();
            //this.ProcessSlide();
        }

        protected override void OnActiveSceneChanged(Scene oldScene, Scene newScene) {
            base.OnActiveSceneChanged(oldScene, newScene);
            this.ResetFallCalculation();
        }

        public override void TeleportToLocation(Vector3 targetPos, Quaternion targetRotation = default) {
            base.TeleportToLocation(targetPos, targetRotation);
            this.ResetFallCalculation();
        }

        #endregion <<---------- CCharacterBase ---------->>


        
        
        #region <<---------- Input ---------->>

        protected void ProcessAim() {
            this._isAimingRx.Value = this.InputAim;
        }
        
        #endregion <<---------- Input ---------->>

        


        #region <<---------- Aerial and Falling ---------->>

        protected void ResetFallCalculation() {
            this._lastYPositionCharWasNotFalling = this.Position.y;
        }
		
        protected void ProcessAerialAndFallMovement() {
            if (this._isTouchingTheGroundRx.Value || this.Velocity.y >= 0f
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

        protected Vector3 GetMyVelocityXZ() {
            var velocity = this.Velocity;
            velocity.y = 0f;
            return velocity;
        }
        
        protected float GetMyVelocity_XZ_Magnitude() {
            return this.GetMyVelocityXZ().magnitude;
        }

        protected override bool SetMovementState(CMovState value) {
            var oldValue = this._currentMovState;
            if(!base.SetMovementState(value)) return false;
           
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

            return true;
        }

        protected override void ProcessMovement() {
			this._animator.CSetFloatWithLerp(this.ANIM_CHAR_MOV_SPEED_XZ, this.GetMyVelocity_XZ_Magnitude() * 100f, ANIMATION_BLENDTREE_LERP * CTime.DeltaTimeScaled * this.TimelineTimescale);
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
			Vector3 targetMotion = this.CanMoveRx.Value ? this.InputMovement : Vector3.zero;
			float movSpeedMultiplier = 1f;

            bool isTouchingTheGround = this._isTouchingTheGroundRx.Value;
            
            // air control
            if (!isTouchingTheGround) {
                targetMotion *= this._airControl;
            }

            // manual movement
			if (isTouchingTheGround && this.CurrentMovState != CMovState.Sliding && !this._blockingEventsManager.IsAnyBlockingEventHappening) {
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
                movSpeedMultiplier = this.GetSpeedForCurrentMovementState();
				if (this.IsAiming) {
                    movSpeedMultiplier *= 0.5f;
				}

				if (Debug.isDebugBuild && Input.GetKey(KeyCode.RightShift)) {
					movSpeedMultiplier *= 20f;
				}
			}
            
			// is sliding
			if (isTouchingTheGround && this.CurrentMovState == CMovState.Sliding) {
				targetMotion = (this.InputMovement * this.SlideControlAmmount)
						+ this.transform.forward + (this._groundNormal * 2f);
				movSpeedMultiplier = this._slideSpeed;
			}
            
			// momentum
			if (!isTouchingTheGround && (MovementMomentumXZ.x != 0f || MovementMomentumXZ.z != 0f)) {
                this.MovementMomentumXZ -= this.MovementMomentumXZ * (this._aerialMomentumLoosePercentage * deltaTime);
				if(this._debug) Debug.Log($"Applying {this.MovementMomentumXZ}, targetMotion now is {targetMotion}");
			}
			
            // additional movement
            var additionalMovement = this.AdditionalMovementFromAnimator;
            additionalMovement.y = 0f;
            
			// root motion
			var rootMotionDeltaPos = this.RootMotionDeltaPosition;
			rootMotionDeltaPos.y = 0f;
            
			// move character
			return (targetMotion * (movSpeedMultiplier * deltaTime))
                + (this.MovementMomentumXZ * deltaTime)
                + rootMotionDeltaPos 
                + (additionalMovement * deltaTime)
            ;
		}

        protected float ProcessVerticalMovement(float deltaTime) {

            var verticalDelta = this.Velocity.y > 0f ? 0f : this.Velocity.y; // consider only fall velocity
			verticalDelta += this.RootMotionDeltaPosition.y;
            verticalDelta += this.AdditionalMovementFromAnimator.y * deltaTime;
			verticalDelta += (Physics.gravity.y * this._gravityMultiplier) * deltaTime;
            
			return verticalDelta;
		}

        protected void UpdateIfIsGrounded() {
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

            var angleFromGround = Vector3.SignedAngle(Vector3.up, this._groundNormal, transform.right);

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
        
        protected void ProcessRotation() {
            if (this._blockingEventsManager.IsAnyBlockingEventHappening) return;
            if (this.IsStrafingRx.Value) {
                this.RotateTowardsDirection(this._aimTargetDirection);
            }
            else {
                if (this.CurrentMovState == CMovState.Sliding) {
                    this.RotateTowardsDirection(this._groundNormal + this.GetMyVelocityXZ());
                }
                else {
                    this.RotateTowardsDirection(this.InputMovement);
                }
            }
        }
        
        protected void RotateTowardsDirection(Vector3 dir) {
            dir.y = 0f;
            if (dir == Vector3.zero) return;
            this._targetLookRotation = Quaternion.LookRotation(dir);

            var rotateSpeed = this._curveRotationRateOverSpeed.Evaluate(this.Velocity.magnitude);

            if (!this._isTouchingTheGroundRx.Value) {
                rotateSpeed *= this._airControl;
            }
            
            // lerp rotation
            this.transform.rotation = Quaternion.RotateTowards(
                this.transform.rotation,
                this._targetLookRotation,
                rotateSpeed * CTime.DeltaTimeScaled * this.TimelineTimescale);
        }
        
        #endregion <<---------- Rotation ---------->>
        
	}
}

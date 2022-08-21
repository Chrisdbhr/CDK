using Rewired.Utils.Libraries.TinyJson;
using UniRx;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
	public class CPlayerCharacter3D : CCharacterBase {

        #region <<---------- Properties and Fields ---------->>
        
        private float _charInitialHeight;
        protected readonly int ANIM_INPUT_MAGNITUDE = Animator.StringToHash("input magnitude");
        protected readonly int ANIM_JUMP_START = Animator.StringToHash("Jump Start");
        protected readonly int ANIM_TIME_WAITING_FOR_JUMP = Animator.StringToHash("Time Waiting For Jump");
        protected readonly int ANIM_MOVEMENT_STATE = Animator.StringToHash("Movement State");

        #region <<---------- Aerial ---------->>

        [Header("Aerial Movement")]
        [SerializeField] 
        float _gravityMultiplier = 1.5f;
        [SerializeField] float airRotationControl = 0.1f;

        [SerializeField] private float _jumpCooldown = 0.1f;
        
        #endregion <<---------- Aerial ---------->>
        
        #region <<---------- Rotation ---------->>
        [Header("Rotation")]
        [SerializeField] private float _rotationSpeed = 1000f;
        protected Quaternion _targetLookRotation;
        #endregion <<---------- Rotation ---------->>

        #region <<---------- Sliding ---------->>

        [Header("Sliding")]
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

        protected float _slideTimeToStumble = 1.0f;
        protected float _slideBeginTime;

        private const float DELAY_TO_TOGGLE_SLIDE = 0.4f;
        private const float SLIDE_FROM_CHAR_SLOPE_LIMIT_MULTIPLIER = 0.6f;

        #endregion <<---------- Sliding ---------->>





        #region <<---------- New Movement ---------->>

        [SerializeField, Range(0f, 100f)]
        float maxSpeed = 10f;

        [SerializeField, Range(0f, 100f)]
        float maxAcceleration = 10f, maxAirAcceleration = 1f;

        [SerializeField, Range(0f, 10f)]
        float jumpHeight = 4f;

        [SerializeField, Range(0, 5)]
        int maxAirJumps = 0;

        [SerializeField, Range(0, 90)]
        float maxGroundAngle = 25f, maxStairsAngle = 50f;

        [SerializeField, Range(0f, 100f)]
        float maxSnapSpeed = 100f;

        [SerializeField, Min(0f)]
        float probeDistance = 1f;

        [SerializeField]
        LayerMask probeMask = -1, stairsMask = -1;

        [SerializeField]
        float _slopeRecoverThreshold = 0.1f;
        
        Vector3 Gravity => Physics.gravity * this._gravityMultiplier;

        public CapsuleCollider capsuleCol;

        Vector3 velocity, desiredVelocity;

        bool desiredJump;

        Vector3 contactNormal, steepNormal;

        int groundContactCount;
        int steepContactCount;

        public bool IsOnGround => this.stepsSinceLastGrounded <= 0;
        protected BoolReactiveProperty _isOnGroundRx = new BoolReactiveProperty();
        bool _onGround => groundContactCount > 0;

        bool OnSteep => steepContactCount > 0;

        int jumpPhase;

        float minGroundDotProduct, minStairsDotProduct;

        int stepsSinceLastGrounded, stepsSinceLastJump;

        #endregion <<---------- New Movement ---------->>

        
        
        
        #endregion <<---------- Properties and Fields ---------->>


        

        #region <<---------- MonoBehaviour ---------->>
        protected override void Awake() {
            base.Awake();
            OnValidate();
            this._charInitialHeight = this.capsuleCol.height;
        }

        protected override void OnEnable() {
            base.OnEnable();
            
            // // can slide
            // this._enableSlideRx = new ReactiveProperty<bool>(true);
            // this._enableSlideRx.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(canSlide => {
            //     // stopped sliding
            //     if (!canSlide && this.CurrentMovState == CMovState.Sliding) {
            //         this.SetMovementState(CMovState.Walking);
            //     }
            // });

            _isOnGroundRx.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(onGround => {
                if(!onGround && InputJump) this._animator.CSetTriggerSafe(ANIM_JUMP_START);
            });
        }

        #if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected() {
            if (!this._debug) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this._aimTargetPos, 0.05f);
            Handles.Label(this._aimTargetPos, $"{this.name} aimTargetPos");
        }
		#endif

        protected override void Update() {
            base.Update();
            desiredVelocity = GetHorizontalNextMovementDelta() * this.maxSpeed; //new Vector3(InputMovement.x, 0f, InputMovement.y) * maxSpeed;
            this._isOnGroundRx.Value = this.IsOnGround;
            this.UpdateAnimator();
        }

        private void UpdateAnimator() {
            this._animator.CSetFloatWithLerp(this.ANIM_CHAR_MOV_SPEED_XZ, this.GetMyVelocity_XZ_Magnitude() * 100f, ANIMATION_BLENDTREE_LERP * CTime.DeltaTimeScaled * this.TimelineTimescale);
            this._animator.CSetIntegerSafe(this.ANIM_MOVEMENT_STATE, (int)this.CurrentMovState);
            
        }

        protected void OnValidate () {
            if (capsuleCol == null) capsuleCol = this.GetComponent<CapsuleCollider>();
            minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
            minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            
            UpdateState();
            AdjustVelocity();
            ProcessJump();
            
            // Dont slide on slopes
            if (this._onGround && velocity.sqrMagnitude < _slopeRecoverThreshold) {
                velocity += contactNormal * (Vector3.Dot(Gravity, contactNormal) * CTime.DeltaTimeScaled);
            } else {
                velocity += Gravity * CTime.DeltaTimeScaled;
            }

            body.velocity = velocity;
            this.ProcessRotation();

            // need to be last
            ClearState();
        }
        
        void OnCollisionEnter (Collision collision) {
            EvaluateCollision(collision);
        }

        void OnCollisionStay (Collision collision) {
            EvaluateCollision(collision);
        }
        
        #endregion <<---------- MonoBehaviour ---------->>
        
        
        
        
        #region <<---------- Input ---------->>

        protected void ProcessAim() {
            this._isAimingRx.Value = this.InputAim;
        }
        
        #endregion <<---------- Input ---------->>

        


        
        #region <<---------- Physics ---------->>

        void ProcessJump() {
            if (!InputJump) return;
            InputJump = false;

            if (this._animator.GetFloat(this.ANIM_TIME_WAITING_FOR_JUMP) < this._jumpCooldown) return;
            
            if (this._blockingEventsManager.IsAnyBlockingEventHappening) return;

            Jump();
        }
        
        void ClearState () {
            groundContactCount = steepContactCount = 0;
            contactNormal = steepNormal = Vector3.zero;
        }
        
        void UpdateState () {
            stepsSinceLastGrounded += 1;
            stepsSinceLastJump += 1;
            velocity = body.velocity;
            if (this._onGround || SnapToGround() || CheckSteepContacts()) {
                stepsSinceLastGrounded = 0;
                if (stepsSinceLastJump > 1) {
                    jumpPhase = 0;
                }
                if (groundContactCount > 1) {
                    contactNormal.Normalize();
                }
            }
            else {
                contactNormal = Vector3.up;
            }
        }
        
        bool SnapToGround () {
		    if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) {
			    return false;
		    }
		    float speed = velocity.magnitude;
		    if (speed > maxSnapSpeed) {
			    return false;
		    }
		    if (!Physics.Raycast(
			    body.position + (Vector3.up * (probeDistance * 0.5f)), Vector3.down, out RaycastHit hit,
			    probeDistance, probeMask
		    )) {
			    return false;
		    }
		    if (hit.normal.y < GetMinDot(hit.collider.gameObject.layer)) {
			    return false;
		    }

		    groundContactCount = 1;
		    contactNormal = hit.normal;
		    float dot = Vector3.Dot(velocity, hit.normal);
		    if (dot > 0f) {
			    velocity = (velocity - (hit.normal * dot)).normalized * speed;
		    }
		    return true;
	    }

	    bool CheckSteepContacts () {
		    if (steepContactCount > 1) {
			    steepNormal.Normalize();
			    if (steepNormal.y >= minGroundDotProduct) {
				    steepContactCount = 0;
				    groundContactCount = 1;
				    contactNormal = steepNormal;
				    return true;
			    }
		    }
		    return false;
	    }

	    void AdjustVelocity () {
		    Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
		    Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

		    float currentX = Vector3.Dot(velocity, xAxis);
		    float currentZ = Vector3.Dot(velocity, zAxis);

		    float acceleration = this._onGround ? maxAcceleration : maxAirAcceleration;
		    float maxSpeedChange = acceleration * Time.deltaTime;

		    float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
		    float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

		    velocity += (xAxis * (newX - currentX)) + (zAxis * (newZ - currentZ));
	    }

	    void Jump () {
		    Vector3 jumpDirection;
		    if (this._onGround) {
			    jumpDirection = contactNormal;
		    }
		    else if (OnSteep) {
			    jumpDirection = steepNormal;
			    jumpPhase = 0;
		    }
		    else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps) {
			    if (jumpPhase == 0) {
				    jumpPhase = 1;
			    }
			    jumpDirection = contactNormal;
		    }
		    else {
			    return;
		    }

		    stepsSinceLastJump = 0;
		    jumpPhase += 1;
		    float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
		    jumpDirection = (jumpDirection + Vector3.up).normalized;
		    float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
		    if (alignedSpeed > 0f) {
			    jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
		    }
		    velocity += jumpDirection * jumpSpeed;
	    }

        void EvaluateCollision (Collision collision) {
            float minDot = GetMinDot(collision.gameObject.layer);
            for (int i = 0; i < collision.contactCount; i++) {
                Vector3 normal = collision.GetContact(i).normal;
                if (normal.y >= minDot) {
                    groundContactCount += 1;
                    contactNormal += normal;
                }
                else if (normal.y > -0.01f) {
                    steepContactCount += 1;
                    steepNormal += normal;
                }
            }
        }
        
        Vector3 ProjectOnContactPlane (Vector3 vector) {
            return vector - contactNormal * Vector3.Dot(vector, contactNormal);
        }

        float GetMinDot (int layer) {
            return (stairsMask & (1 << layer)) == 0 ?
            minGroundDotProduct : minStairsDotProduct;
        }
        
        #endregion <<---------- Physics ---------->>

        

        
        #region <<---------- Movement ---------->>

        protected Vector3 GetMyVelocityXZ() {
            var velocity = this.Velocity;
            velocity.y = 0f;
            return velocity;
        }
        
        protected float GetMyVelocity_XZ_Magnitude() {
            return this.GetMyVelocityXZ().magnitude;
        }

		protected virtual Vector3 GetHorizontalNextMovementDelta() {
			Vector3 targetMotion = this.CanMoveRx.Value ? this.InputMovement : Vector3.zero;
			float movSpeedMultiplier = 1f;

            // air control

            // manual movement
			if (this._onGround && !this._blockingEventsManager.IsAnyBlockingEventHappening) {
				// input movement
				if (targetMotion != Vector3.zero) {
                    var maxMovSpeed = this.GetMaxMovementSpeed();
                    if (maxMovSpeed <= CMovState.Walking || this._isAimingRx.Value || this.InputWalk){
                        this.CurrentMovState = CMovState.Walking;
					} else if(maxMovSpeed < CMovState.Sprint) {
                        this.CurrentMovState = (this.InputRun ? CMovState.Running : CMovState.Walking);
                    }
                    else {
                        this.CurrentMovState = (this.InputRun ? CMovState.Sprint : CMovState.Running);
                    }
				}
				else {
					this.CurrentMovState = (CMovState.Idle);
				}

				// set is strafing
				this.IsStrafingRx.Value = this._isAimingRx.Value && this.CurrentMovState <= CMovState.Walking;

				// target movement speed
                movSpeedMultiplier = this.GetSpeedForCurrentMovementState();
				if (this.IsAiming) {
                    movSpeedMultiplier *= 0.5f;
				}
                
			}
            
			// is sliding
			// if (this._onGround && this.CurrentMovState == CMovState.Sliding) {
			// 	targetMotion = (this.InputMovement * this.SlideControlAmmount)
			// 			+ this.transform.forward + (this.contactNormal * 2f);
			// 	movSpeedMultiplier = this._slideSpeed;
			// }

            // additional movement
            var additionalMovement = this.AdditionalMovementFromAnimator;
            additionalMovement.y = 0f;
            
			// root motion
			var rootMotionDeltaPos = this.RootMotionDeltaPosition * this._rootMotionMultiplier;
			rootMotionDeltaPos.y = 0f;
            
            if (Debug.isDebugBuild && Input.GetKey(KeyCode.RightShift)) {
                movSpeedMultiplier *= 20f;
            }
            
			// get move delta
			return (targetMotion * (movSpeedMultiplier))
                + rootMotionDeltaPos 
                + (additionalMovement)
            ;
		}

        protected virtual float GetVerticalNextMovementDelta(float deltaTime) {

            var verticalDelta = this.Velocity.y > 0f ? 0f : this.Velocity.y; // consider only fall velocity
			verticalDelta += this.RootMotionDeltaPosition.y * this._rootMotionMultiplier;
            verticalDelta += this.AdditionalMovementFromAnimator.y * deltaTime;
			verticalDelta += (Physics.gravity.y * this._gravityMultiplier) * deltaTime;
            
			return verticalDelta;
		}
        
        #endregion <<---------- Movement ---------->>




        #region <<---------- Rotation ---------->>
        
        protected void ProcessRotation() {
            if (this._blockingEventsManager.IsAnyBlockingEventHappening) return;
            if (this.IsStrafingRx.Value) {
                this.RotateTowardsDirection(this._aimTargetDirection);
            }
            else {
                this.RotateTowardsDirection(this.InputMovement, true);
            }
        }
        
        protected void RotateTowardsDirection(Vector3 dir, bool modifySpeed = false) {
            dir.y = 0f;
            if (dir == Vector3.zero) return;
            var deltaTime = CTime.DeltaTimeScaled * this.TimelineTimescale;
            this._targetLookRotation = Quaternion.LookRotation(dir);

            var currentRotateSpeed = this._rotationSpeed;
            if (modifySpeed) {
                if (this.CurrentMovState.IsMovingFast()) {
                    currentRotateSpeed *= 0.5f;
                    if (this.CurrentMovState == CMovState.Sprint) {
                        currentRotateSpeed *= 0.33f;
                    }
                }

                if (!this._onGround) {
                    currentRotateSpeed *= this.airRotationControl;
                }
            }
            
            // lerp rotation
            this.transform.rotation = Quaternion.RotateTowards(
                this.transform.rotation,
                this._targetLookRotation,
                currentRotateSpeed * deltaTime);
        }
        
        #endregion <<---------- Rotation ---------->>
        
	}
}

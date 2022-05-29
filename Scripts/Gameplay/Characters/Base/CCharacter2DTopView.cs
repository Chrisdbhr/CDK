using System;
using UnityEngine;

namespace CDK {
    [RequireComponent(typeof(Rigidbody2D))]
    public class CCharacter2DTopView : CCharacterBase {

        [SerializeField] private Rigidbody2D _rb;
        protected readonly int ANIM_IS_MOVING = Animator.StringToHash("isMoving");
        protected readonly int ANIM_VELOCITY_MAGNITUDE = Animator.StringToHash("velocityMagnitude");
        protected readonly int ANIM_LOOK_DIRECTION_X = Animator.StringToHash("facingDirectionX");
        protected readonly int ANIM_LOOK_DIRECTION_Y = Animator.StringToHash("facingDirectionY");
        
        

        #region <<---------- MonoBehaviour ---------->>
        protected override void Awake() {
            base.Awake();
            if (this._rb == null) _rb = this.GetComponent<Rigidbody2D>();
        }

        protected override void Update() {
            this.UpdateAnimator();
            base.Update();
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
        }
 
        #endregion <<---------- MonoBehaviour ---------->>

        
        
        
        #region <<---------- CCharacterBase ---------->>
        
        #region <<---------- Movement ---------->>
        
        protected override void SetMovementState(CMovState value) {
            if (this._currentMovState == value) return;
            var oldValue = this._currentMovState;
            this._currentMovState = value;

            // set animators
            if (this._animator) {
                this._animator.SetBool(this.ANIM_CHAR_IS_WALKING, value == CMovState.Walking);
                this._animator.SetBool(this.ANIM_CHAR_IS_RUNNING, value == CMovState.Running);
                this._animator.SetBool(this.ANIM_CHAR_IS_SPRINTING, value == CMovState.Sprint);
            }
        }

        public override Vector3 GetMyVelocity() {
            return this._previousPosition - this.transform.position;
        }

        protected override void ProcessMovement() {
            if (!this.CanMoveRx.Value) return;
            this._rb.velocity = this.GetInputMovement2d() * (WalkSpeed * CTime.DeltaTimeScaled);
            return;
        }

        #endregion <<---------- Movement ---------->>

        protected override void UpdateIfIsGrounded() {
            return;
        }

        protected override void ResetFallCalculation() {
            return;
        }

        protected override void ProcessRotation() {
            return;
        }

        #endregion <<---------- CCharacterBase ---------->>




        #region <<---------- Animation ---------->>
        
        protected virtual void UpdateAnimator() {
            if (!this._animator) return;
            var velocity = this.GetMyVelocity();
            float velocityMagnitude = this.GetMyVelocityMagnitude();
                
            bool isMoving = velocityMagnitude.CAbs() > 0f;
                
            this._animator.CSetFloatSafe(this.ANIM_VELOCITY_MAGNITUDE, isMoving ? velocityMagnitude : 0f);

            this._animator.CSetBoolSafe(this.ANIM_IS_MOVING, isMoving);
                
            this._animator.CSetFloatSafe(this.ANIM_CHAR_MOV_SPEED_X, isMoving ? velocity.x : 0f);
            this._animator.CSetFloatSafe(this.ANIM_CHAR_MOV_SPEED_Y, isMoving ? velocity.y : 0f);

            var input2d = this.GetInputMovement2d();
            // last facing direction
            if (input2d.x != 0f || input2d.y != 0f) {
                this._animator.CSetFloatSafe(this.ANIM_LOOK_DIRECTION_X, input2d.x);
                this._animator.CSetFloatSafe(this.ANIM_LOOK_DIRECTION_Y, input2d.y);
            }
        }
       
        #endregion <<---------- Animation ---------->>

    }
}
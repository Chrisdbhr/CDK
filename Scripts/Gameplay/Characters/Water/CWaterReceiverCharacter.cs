using System;
using UnityEngine;

namespace CDK.Water {
    public class CWaterReceiverCharacter : CMonoBehaviourUpdateExecutionLoopTime, ICWaterInteraction {

        #region <<---------- Initializers ---------->>

        public void Initialize(Animator animator, CharacterController charController, CPlayerCharacter3D ownerCharacter) {
            this.TargetAnimator = animator;
            this.TargetCharacterController = charController;
            this.OwnerCharacter = ownerCharacter;
        }

        #endregion <<---------- Initializers ---------->>

        
        
        
        #region <<---------- Properties and Fields ---------->>

        protected readonly int ANIM_CHAR_IS_SWIMMING = Animator.StringToHash("is swimming");

        public Animator TargetAnimator;
        public CharacterController TargetCharacterController;
        public CPlayerCharacter3D OwnerCharacter;

        [Range(0f,1f)]
        public float PercentageToBeginSwim = 0.2f;
        
        public bool IsSwimming {
            get { return this._isSwimming; }
            set {
                if (this._isSwimming == value) return;
                this._isSwimming = value;
                this.TargetAnimator.CSetBoolSafe(this.ANIM_CHAR_IS_SWIMMING, this._isSwimming);
            }
        }
        private bool _isSwimming;
        
        private bool _isTouchingWater;
        private Transform _waterTransformCenter;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- ICWaterInteraction ---------->>

        public void OnEnterWater(Transform waterTransform) {
            this._isTouchingWater = true;
            this._waterTransformCenter = waterTransform;
        }

        public void OnExitWater(Transform waterTransform) {
            this._isTouchingWater = false;
            this._waterTransformCenter = waterTransform;
            this.IsSwimming = false;
        }

        #endregion <<---------- ICWaterInteraction ---------->>

        
        
        
        #region <<---------- Mono Behaviour ---------->>

        protected override void Execute(float deltaTime) {
            if (!this._isTouchingWater) return;
            if (TargetCharacterController == null || this._waterTransformCenter == null) return;
            this.IsSwimming = !OwnerCharacter.IsOnGround || (this.GetCharYPositionToSwim()) <= this._waterTransformCenter.position.y;
        }

        private void OnDrawGizmosSelected() {
            if (TargetCharacterController == null || this._waterTransformCenter == null) return;
            var point = this.transform.position;
            point.y = this.GetCharYPositionToSwim();
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(point, point + Vector3.right);
        }
        
        #endregion <<---------- Mono Behaviour ---------->>


        
        
        #region <<---------- General ---------->>

        protected float GetCharYPositionToSwim() {
            float charPoint = TargetCharacterController.transform.position.y;
            charPoint += TargetCharacterController.center.y * 2f;
            charPoint -= (TargetCharacterController.height * this.PercentageToBeginSwim);
            return charPoint;
        }

        public float GetWaterYLevel() {
            if (this._waterTransformCenter == null) return this.GetCharYPositionToSwim();
            return this._waterTransformCenter.position.y;
        }

        #endregion <<---------- General ---------->>

    }
}
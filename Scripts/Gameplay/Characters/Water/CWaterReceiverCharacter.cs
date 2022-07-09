using System;
using UnityEngine;

namespace CDK.Water {
    public class CWaterReceiverCharacter : CMonoBehaviourUpdateExecutionLoopTime, ICWaterInteraction {

        #region <<---------- Properties and Fields ---------->>

        protected readonly int ANIM_CHAR_IS_SWIMMING = Animator.StringToHash("is swimming");

        public Animator TargetAnimator;
        public CharacterController TargetCharacterController;

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
        }

        #endregion <<---------- ICWaterInteraction ---------->>

        
        
        
        #region <<---------- Mono Behaviour ---------->>

        protected override void Execute(float deltaTime) {
            if (!this._isTouchingWater) return;
            if (TargetCharacterController == null || this._waterTransformCenter == null) return;
            this.IsSwimming = (this.GetCharYPoint()) <= this._waterTransformCenter.position.y;
        }

        private void OnDrawGizmosSelected() {
            if (TargetCharacterController == null || this._waterTransformCenter == null) return;
            var point = this.transform.position;
            point.y = this.GetCharYPoint();
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(point, point + Vector3.right);
        }
        
        #endregion <<---------- Mono Behaviour ---------->>


        
        
        #region <<---------- General ---------->>

        protected float GetCharYPoint() {
            float charPoint = TargetCharacterController.transform.position.y;
            charPoint += TargetCharacterController.center.y * 2f;
            charPoint -= (TargetCharacterController.height * this.PercentageToBeginSwim);
            return charPoint;
        }

        #endregion <<---------- General ---------->>

    }
}
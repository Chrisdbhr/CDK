using System;
using UnityEngine;

namespace CDK {
	public class CGameObjectRotator : CMonoBehaviourUpdateExecutionLoopTime {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private Vector3 _rotateDirectionAndSpeed;
		[SerializeField] private bool _onlyUpdateWhenVisible = true;
		[SerializeField] private bool _ignoreTimescale;
		
        private bool _isVisible;
        private RectTransform _rectTransform;
        private bool _useRectTransform;
        
		#endregion <<---------- Properties and Fields ---------->>

        private void Awake() {
            this._useRectTransform = (this._rectTransform = this.GetComponent<RectTransform>());
        }

        private void OnBecameVisible() {
			this._isVisible = true;
		}

		private void OnBecameInvisible() {
			this._isVisible = false;
		}

		protected override void Execute(float deltaTime) {
			if (this._onlyUpdateWhenVisible && !this._isVisible) return;
            
			if (this._useRectTransform) this._rectTransform.Rotate(GetRotation());
            else this.transform.Rotate(GetRotation());
		}

        private Vector3 GetRotation() {
            return this._rotateDirectionAndSpeed * (_ignoreTimescale ? Time.unscaledDeltaTime : CTime.DeltaTimeScaled);
        }
	}
}
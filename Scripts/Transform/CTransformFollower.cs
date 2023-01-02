using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
    [DefaultExecutionOrder(350)]
	public class CTransformFollower : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
        
		[SerializeField] private CMonobehaviourExecutionLoop executionLoop = CMonobehaviourExecutionLoop.LateUpdate;
		[Obsolete("OBSOLETE, use public property instead.")]
		[SerializeField] private Transform _transformToFollow;
		public Transform TransformToFollow {
			get => this._transformToFollow;
			set {
				if (this._transformToFollow == value) return;
				this._transformToFollow = value;
				this.CheckIfWillMove();
			}
		}
		[SerializeField] private Vector3 _followOffset = Vector3.zero;
		[SerializeField] private FollowTypeEnum _followType;
		[SerializeField] private float _followSpeed = 10f;
		[NonSerialized] private Transform _myTransform;

        [SerializeField] private bool _ignoreTimeScale;
		[SerializeField] private bool _ignoreXAxis;
		[SerializeField] private bool _ignoreYAxis;
		[SerializeField] private bool _ignoreZAxis;
       
        [SerializeField] private Vector3 _positionMultiplier = UnityEngine.Vector3.one;

		#endregion <<---------- Properties and Fields ---------->>

		
		
		
		#region <<---------- Enums ---------->>

		private enum FollowTypeEnum {
			instant, smooth
		}
		
		#endregion <<---------- Enums ---------->>

		
		

		#region <<---------- MonoBehaviour ---------->>
		
		private void Awake() {
			this._myTransform = this.transform;
		}

		private void OnEnable() {
			this.CheckIfWillMove();
		}

		private void Update() {
			if (this.executionLoop != CMonobehaviourExecutionLoop.Update) return;
            Execute(this._ignoreTimeScale ? Time.unscaledDeltaTime : CTime.DeltaTimeScaled);
        }

		private void FixedUpdate() {
			if (this.executionLoop != CMonobehaviourExecutionLoop.FixedUpdate) return;
            Execute(this._ignoreTimeScale ? Time.fixedUnscaledDeltaTime : CTime.FixedDeltaTimeScaled);
		}

		private void LateUpdate() {
			if (this.executionLoop != CMonobehaviourExecutionLoop.LateUpdate) return;
            Execute(this._ignoreTimeScale ? Time.unscaledDeltaTime : CTime.DeltaTimeScaled);
		}

		#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			if (this._transformToFollow == null) {
				Handles.Label(this.transform.position, $"Follow Target is null!");
				return;
			}
			Handles.color = Gizmos.color = Color.cyan;
			var targetPos = this._transformToFollow.position + this._followOffset;
			Handles.Label(targetPos, $"Follow Target: {this._transformToFollow.name}");
			Gizmos.DrawLine(this.transform.position, targetPos);
		}
		#endif
 
		#endregion <<---------- MonoBehaviour ---------->>


		
		
		#region <<---------- General ---------->>

        protected virtual void Execute(float deltaTime) {
            this.FollowTarget(deltaTime);
        }

		private void CheckIfWillMove() {
			if (this._ignoreXAxis && this._ignoreYAxis && this._ignoreZAxis) {
				Debug.LogError($"'{this.name}' is set to ignore all axis when following so it will remain stationary."); 
			}
		}
		
		private void FollowTarget(float deltaTime) {
			if (this._transformToFollow == null) return;

			if (this._ignoreXAxis && this._ignoreYAxis && this._ignoreZAxis) return;
			
			var targetPos = Vector3.Scale(this._transformToFollow.position, this._positionMultiplier);
            targetPos += this._followOffset;
			if (this._ignoreXAxis) targetPos.x = this.transform.position.x;
			if (this._ignoreYAxis) targetPos.y = this.transform.position.y;
			if (this._ignoreZAxis) targetPos.z = this.transform.position.z;
			
			switch (this._followType) {
				case FollowTypeEnum.instant:
					this._myTransform.position = targetPos;
					break;
				case FollowTypeEnum.smooth:
					this._myTransform.position = Vector3.Lerp(this._myTransform.position, targetPos, this._followSpeed * deltaTime);
					break;
			}

		}
		
		#endregion <<---------- General ---------->>
		
	}
}

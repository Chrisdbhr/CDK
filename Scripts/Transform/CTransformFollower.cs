using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public class CTransformFollower : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private CMonobehaviourExecutionLoop executionLoop;
		[SerializeField] private Transform _transformToFollow;
		[SerializeField] private Vector3 _followOffset = Vector3.zero;
		[SerializeField] private FollowTypeEnum _followType;
		[SerializeField] private float _followSpeed = 10f;
		[NonSerialized] private Transform _myTransform;

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

		private void Update() {
			if (this.executionLoop != CMonobehaviourExecutionLoop.Update) return;
			this.FollowTarget(Time.deltaTime);
		}

		private void FixedUpdate() {
			if (this.executionLoop != CMonobehaviourExecutionLoop.FixedUpdate) return;
			this.FollowTarget(Time.fixedDeltaTime);
		}

		private void LateUpdate() {
			if (this.executionLoop != CMonobehaviourExecutionLoop.LateUpdate) return;
			this.FollowTarget(Time.deltaTime);
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
		
		private void FollowTarget(float deltaTime) {
			if (this._transformToFollow == null) return;

			switch (this._followType) {
				case FollowTypeEnum.instant:
					this._myTransform.position = this._transformToFollow.position + this._followOffset;
					break;
				case FollowTypeEnum.smooth:
					this._myTransform.position = Vector3.Lerp(this._myTransform.position, this._transformToFollow.position + this._followOffset, this._followSpeed * deltaTime);
					break;
			}

		}
		
		#endregion <<---------- General ---------->>
		
	}
}

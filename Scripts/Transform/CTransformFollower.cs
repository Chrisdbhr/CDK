using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
    [DefaultExecutionOrder(350)]
	public class CTransformFollower : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
        
		[SerializeField] CMonobehaviourExecutionLoop executionLoop = CMonobehaviourExecutionLoop.LateUpdate;
		[Obsolete("OBSOLETE, use public property instead.")]
		[SerializeField]
		Transform _transformToFollow;
        public Transform TransformToFollow => _transformToFollow;
		[Header("Position")]
		[SerializeField] Vector3 _followOffset = Vector3.zero;
		[SerializeField] FollowTypeEnum _followType;
		[SerializeField] float _followSpeed = 10f;
		[NonSerialized] Transform _myTransform;

        [SerializeField] bool _ignoreTimeScale;
		[SerializeField] bool _ignoreXAxis;
		[SerializeField] bool _ignoreYAxis;
		[SerializeField] bool _ignoreZAxis;

        [SerializeField] Vector3 _positionMultiplier = Vector3.one;

		[Header("Rotation")]
		[SerializeField] bool _followRotation;

		public event Action<Transform> TransformToFollowChanged = delegate { };

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- Enums ---------->>

		enum FollowTypeEnum {
			instant, smooth
		}

		#endregion <<---------- Enums ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			_myTransform = transform;
		}

        protected virtual void OnEnable() {
			CheckIfWillMove();
		}

        protected virtual void Update() {
			if (executionLoop != CMonobehaviourExecutionLoop.Update) return;
            Execute(_ignoreTimeScale ? Time.unscaledDeltaTime : CTime.DeltaTimeScaled);
        }

		protected virtual void FixedUpdate() {
			if (executionLoop != CMonobehaviourExecutionLoop.FixedUpdate) return;
            Execute(_ignoreTimeScale ? Time.fixedUnscaledDeltaTime : CTime.DeltaTimeScaled);
		}

        protected virtual void LateUpdate() {
			if (executionLoop != CMonobehaviourExecutionLoop.LateUpdate) return;
            Execute(_ignoreTimeScale ? Time.unscaledDeltaTime : CTime.DeltaTimeScaled);
		}

		#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected() {
			if (_transformToFollow == null) {
				Handles.Label(transform.position, $"Follow Target is null!");
				return;
			}
			Handles.color = Gizmos.color = Color.cyan;
			var targetPos = _transformToFollow.position + _followOffset;
			Handles.Label(targetPos, $"Follow Target: {_transformToFollow.name}");
			Gizmos.DrawLine(transform.position, targetPos);
		}
		#endif

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- General ---------->>

        protected virtual void Execute(float deltaTime) {
            FollowTarget(deltaTime);
        }

        void CheckIfWillMove() {
			if (_ignoreXAxis && _ignoreYAxis && _ignoreZAxis) {
				Debug.LogError($"'{name}' is set to ignore all axis when following so it will remain stationary.");
			}
		}

		void FollowTarget(float deltaTime) {
			if (_transformToFollow == null) return;

			if (_followRotation) {
				transform.rotation = _transformToFollow.rotation;
			}

			if (_ignoreXAxis && _ignoreYAxis && _ignoreZAxis) return;

			var targetPos = Vector3.Scale(_transformToFollow.position, _positionMultiplier);
            if (!_followOffset.CIsZero()) {
                targetPos += _transformToFollow.TransformVector(_followOffset);
            }
			if (_ignoreXAxis) targetPos.x = transform.position.x;
			if (_ignoreYAxis) targetPos.y = transform.position.y;
			if (_ignoreZAxis) targetPos.z = transform.position.z;

			switch (_followType) {
				case FollowTypeEnum.instant:
					_myTransform.position = targetPos;
					break;
				case FollowTypeEnum.smooth:
					_myTransform.position = Vector3.Lerp(_myTransform.position, targetPos, _followSpeed * deltaTime);
					break;
			}

		}

        public void SetTransformToFollow(Transform t) {
            if (_transformToFollow == t) return;
            _transformToFollow = t;
            TransformToFollowChanged?.Invoke(t);
            CheckIfWillMove();
        }
		
		#endregion <<---------- General ---------->>
		
	}
}

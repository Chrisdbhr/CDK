using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if FMOD
using FMODUnity;
#endif

#if LUDIQ_CHRONOS
using Chronos;
#endif

namespace CDK {
	[SelectionBase]
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public abstract class CCharacter_Base : MonoBehaviour {

		#region <<---------- Properties ---------->>

        public CPlayerInputValues Input;
        
		#region <<---------- Cache and References ---------->>

        public Rigidbody Body { get; private set; }
        public CapsuleCollider CapsuleCollider { get; private set; }

        protected CBlockingEventsManager _blockingEventsManager;
#pragma warning disable CS0108, CS0114
        protected Transform transform;
#pragma warning restore CS0108, CS0114

        public Vector3 Position {
			get {
				return this.transform.position;
			}
			protected set {
                this.transform.position = value;
                Physics.SyncTransforms();
			}
		}

		#endregion <<---------- References ---------->>
        
		#region <<---------- Time ---------->>

		protected float TimelineTimescale {
			get {
#if LUDIQ_CHRONOS
				return (this._timeline != null ? this._timeline.timeScale : 1f);
#else
				return 1f;
#endif
			}
		}

		#if LUDIQ_CHRONOS
		[SerializeField] private Chronos.Timeline _timeline;
		#endif
		
		#endregion <<---------- Time ---------->>

		#region <<---------- Movement Properties ---------->>
        public Vector3 Velocity {
            get => this.Body.velocity;
        }

        [SerializeField] protected float _rootMotionMultiplier = 1f;
        [HideInInspector] public Vector3 RootMotionDeltaPosition = Vector3.zero;
		

		#region <<---------- Run and Walk ---------->>
        [Header("Movement")]
        [SerializeField] private CMovementSpeed _currentMovementSpeed;
		public CMovementSpeed CurrentMovementSpeed {
			get { return this._currentMovementSpeed; }
            protected set {
                if (this._currentMovementSpeed == value) return;
                this.CurrentMovementStateChanged?.Invoke(this._currentMovementSpeed, value);
                this._currentMovementSpeed = value;
            }
		}
        public event Action<CMovementSpeed, CMovementSpeed> CurrentMovementStateChanged;
        public CMovementSpeed MaxMovementSpeed;
        
        protected float GetSpeedForCurrentMovementSpeed() {
            switch (this.MaxMovementSpeed) {
                case CMovementSpeed.Sprint:
                    return this.SprintSpeed;
                case CMovementSpeed.Running:
                    return this.RunSpeed;
            }
            return this.WalkSpeed;
        }

		public float WalkSpeed {
			get { return this._walkSpeed; }
		}

        [SerializeField] [Min(0f)] private float _walkSpeed = 0.6f;
		public float RunSpeed {
			get { return this._runSpeed; }
		}
        [SerializeField] [Min(0f)] private float _runSpeed = 1.5f;

        public float SprintSpeed {
            get { return this._sprintSpeed; }
        }
        [SerializeField] [Min(0f)] private float _sprintSpeed = 3f;

		public ReactiveProperty<bool> CanMoveRx { get; protected set; }

        #endregion <<---------- Run and Walk ---------->>
        
		#endregion <<---------- Movement Properties ---------->>

		#region <<---------- Aim ---------->>
		public bool IsAiming {
			get { return this._isAimingRx.Value; }
		}

		protected ReactiveProperty<bool> _isAimingRx;
		protected Vector3 _aimTargetPos;
		protected Vector3 _aimTargetDirection;
		#endregion <<---------- Aim ---------->>
        
		#region <<---------- Sound ---------->>

		#if FMOD
		[Header("Sound")]
		[SerializeField] protected StudioEventEmitter _mounthVoiceEmitter;
		#endif

		#endregion <<---------- Sound ---------->>

		#region <<---------- Observables ---------->>

		private CompositeDisposable _disposables;

		#endregion <<---------- Observables ---------->>

		#endregion <<---------- Properties ---------->>




		#region <<---------- MonoBehaviour ---------->>
		protected virtual void Awake() {
			this.transform = base.transform;
            this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
            if (this.Body == null) this.Body = this.GetComponent<Rigidbody>();
            if (this.CapsuleCollider == null) this.CapsuleCollider = this.GetComponent<CapsuleCollider>();
        }

		protected virtual void OnEnable() {
            this._disposables?.Dispose();
            this._disposables = new CompositeDisposable();

            #region <<---------- RX Creations ---------->>

            this.CanMoveRx = new ReactiveProperty<bool>(true);
			
            this._isAimingRx = new ReactiveProperty<bool>();

            #endregion <<---------- RX Creations ---------->>

            #region <<---------- Movement ---------->>

            // can mov
            this.CanMoveRx.TakeUntilDisable(this).DistinctUntilChanged().Subscribe(canMove => {
                if (!canMove && this.CurrentMovementSpeed > CMovementSpeed.Idle) {
                    this.CurrentMovementSpeed = CMovementSpeed.Idle;
                }
            });

            #endregion <<---------- Movement ---------->>

            // events
            this._blockingEventsManager.OnDoingBlockingAction += this.DoingBlockingAction;
            SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
		}

		protected virtual void Start() { }

		protected virtual void Update() { }

        protected virtual void LateUpdate() { }

        protected virtual void FixedUpdate() { }
        
		protected virtual void OnDisable() {
            this._disposables?.Dispose();
            this._disposables = null;

            this._blockingEventsManager.OnDoingBlockingAction -= this.DoingBlockingAction;
            SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
		}

		protected virtual void OnDestroy() {
			this.StopTalking();
		}

        protected virtual void Reset() {
            #if LUDIQ_CHRONOS
            if (this._timeline == null) this._timeline = this.GetComponentInChildren<Timeline>();
            #endif
            #if FMOD
            if (this._mounthVoiceEmitter == null) this._mounthVoiceEmitter = this.GetComponentInChildren<StudioEventEmitter>();
            #endif
        }

        #endregion <<---------- MonoBehaviour ---------->>



        
		#region <<---------- Events ---------->>

		protected void DoingBlockingAction(bool isDoing) {
			this.CanMoveRx.Value = !isDoing;
		}

        protected virtual void OnActiveSceneChanged(Scene oldScene, Scene newScene) {
            if (this == null) return;
            this.StopTalking();

            this.RootMotionDeltaPosition = Vector3.zero;

            this._blockingEventsManager.ReleaseFromUnityObject(this);
        }

		#endregion <<---------- Events ---------->>

        
        

		#region <<---------- Aim ---------->>
        
        public void SetAimTargetPosition(Vector3 targetPos) {
			this._aimTargetPos = targetPos;
		}

		public void SetAimDirection(Vector3 dir) {
			this._aimTargetDirection = dir;
		}

		#endregion <<---------- Aim ---------->>




		#region <<---------- Crouch ---------->>

		public void ToggleCrouch() {
//			var oldCrouchState = this.IsCrouched;
			
			// if (this.IsCrouched) {
			// 	// get up
			// 	var transformUp = this._myTransform.up;
			// 	var origin = this._myTransform.position + (transformUp * this.charController.height);
			// 	var collidersWithSelf = Physics.SphereCastAll(
			// 		origin,
			// 		this.charController.radius,
			// 		transformUp.normalized,
			// 		this.charController.height,
			// 		1
			// 	).Select(x => x.collider);
			//
			// 	var allSelfColliders = this.transform.root.GetComponentsInChildren<Collider>();
			//
			// 	var colliders = collidersWithSelf.Except(allSelfColliders).ToArray();
			//
			// 	if (colliders.Length > 0) {
			// 		try {
			// 			var sb = new StringBuilder();
			// 			sb.AppendLine($"{this.name} cant get up, head collided with {colliders.Length} objects:");
			// 			foreach (var col in colliders) {
			// 				sb.AppendLine($"{col.name}");
			// 			}
			// 			Debug.Log(sb.ToString());
			// 		} catch (Exception e) {
			// 			Console.WriteLine(e);
			// 		}
			// 		return;
			// 	}
			// }
			
			// // get down
			// this.charController.height = this._charInitialHeight * (oldCrouchState ? 1f : 0.5f);  
			//
			// var center = this.charController.center;
			// center.y = this.charController.height * 0.5f;
			// this.charController.center = center;
			// 	
			// this._isCrouched.Value = !oldCrouchState;
		}
		
		#endregion <<---------- Crouch ---------->>

		
		
		
		#region <<---------- Voice ---------->>

		public virtual void StopTalking() {
			
			#if FMOD
			if (this._mounthVoiceEmitter == null) return;
			if (this._mounthVoiceEmitter.IsPlaying()) {
				this._mounthVoiceEmitter.AllowFadeout = false;
				this._mounthVoiceEmitter.Stop();
			}
			#else
			Debug.LogError("Character StopTalking() not implemented without FMOD");
			#endif
		}

		#endregion <<---------- Voice ---------->>




		#region <<---------- Transform ---------->>

		public virtual void TeleportToLocation(Vector3 targetPos, Quaternion targetRotation = default) {
			if (targetRotation != default) {
                transform.rotation = targetRotation;
			}
			this.Position = targetPos;
		}
		
		#endregion <<---------- Transform ---------->>

    }
}

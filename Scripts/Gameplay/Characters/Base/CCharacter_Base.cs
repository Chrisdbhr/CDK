using System;
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
    public abstract class CCharacter_Base : MonoBehaviour {

		#region <<---------- Properties ---------->>

        public CPlayerInputValues Input = new CPlayerInputValues();

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

		#if FMOD
        [Header("Sound")]
        [SerializeField] protected StudioEventEmitter _mounthVoiceEmitter;
		#endif

        public event Action OnTeleport {
            add {
                this._onTeleport -= value;
                this._onTeleport += value;
            }
            remove {
                this._onTeleport -= value;
            }
        }
        private Action _onTeleport;
        
        private CompositeDisposable _disposables;
        
		#endregion <<---------- Properties ---------->>




		#region <<---------- MonoBehaviour ---------->>
		protected virtual void Awake() {
			this.transform = base.transform;
            this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
        }

		protected virtual void OnEnable() {
            this._disposables?.Dispose();
            this._disposables = new CompositeDisposable();
            
            // events
            SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
		}

		protected virtual void Start() { }

		protected virtual void Update() { }

        protected virtual void LateUpdate() { }

        protected virtual void FixedUpdate() { }
        
		protected virtual void OnDisable() {
            this._disposables?.Dispose();
            this._disposables = null;

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
        
        protected virtual void OnActiveSceneChanged(Scene oldScene, Scene newScene) {
            if (this == null) return;
            this.StopTalking();
        }

		#endregion <<---------- Events ---------->>




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
            this._onTeleport?.Invoke();
		}
		
		#endregion <<---------- Transform ---------->>

    }
}

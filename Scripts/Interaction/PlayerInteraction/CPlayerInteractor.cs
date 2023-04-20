using System;
using UniRx;
using UnityEngine;

namespace CDK.Interaction {
	public class CPlayerInteractor : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private CCharacter_Base _characterBase;
		[SerializeField] private LayerMask _interactionLayerMask;
		[SerializeField] private float _interactionMaxDistance = 1.0f;
		[SerializeField] private Transform _rayOriginTransform;

		[NonSerialized] private float _interactionCapsuleCheckRadius = 0.10f;

		[NonSerialized] private ReactiveProperty<CInteractable> _currentInteractable;
		[NonSerialized] private Transform _transform;

		[NonSerialized] private bool _jumpNextFrame;
		[NonSerialized] private CBlockingEventsManager _blockingEventsManager;

        protected IDisposable _disposeOnDisable;

		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- Events ---------->>

		public event Action<CInteractable> OnInteractableChanged;

		#endregion <<---------- Events ---------->>
		
		
		

		#region <<---------- MonoBehaviour ---------->>
		private void Awake() {
			this._blockingEventsManager = CBlockingEventsManager.get;
			this._transform = this.transform;
			
			this._characterBase = this.GetComponent<CCharacter_Base>();
			if (this._characterBase == null) {
				Debug.LogError($"Cant find any Character on {this.name}, removing component, character cannot interact with anything!");
				Destroy(this);
			}
		}

		private void OnEnable() {
			this._currentInteractable?.Dispose();
			this._currentInteractable = new ReactiveProperty<CInteractable>();
			
            this._disposeOnDisable = this._currentInteractable
            .Subscribe(newInteractable => {
				if (newInteractable != null) {
					newInteractable.OnBecameInteractionTarget(this._transform);
				}
				this.OnInteractableChanged?.Invoke(newInteractable);
			});
			
		}
		
		private void Update() {
			if (this._blockingEventsManager.IsAnyHappening) {
				this._jumpNextFrame = true;
				return;
			}

			if (this._jumpNextFrame) {
				this._jumpNextFrame = false;
				return;
			}
			
			this.ProcessLookingInteractable();

			// input interaction
			bool inputDownInteract = Input.GetButtonDown(CInputKeys.INTERACT);
			if (inputDownInteract) {
				this.TryToInteract();	
			}
		}

        private void OnDisable() {
            this._disposeOnDisable?.Dispose();
        }

		#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(
				this._rayOriginTransform.position, 
				this._interactionCapsuleCheckRadius);
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>


		

		#region <<---------- Private ---------->>
		
		private void ProcessLookingInteractable() {
			var originPos = this._rayOriginTransform.position;
			var direction = this._rayOriginTransform.forward;

			var collided = Physics.SphereCast(
				originPos,
				this._interactionCapsuleCheckRadius,
				direction,
				out var raycastHit,
				this._interactionMaxDistance,
				this._interactionLayerMask,
				QueryTriggerInteraction.Collide
			);
			if (!collided) {
				this._currentInteractable.Value = null;
				return;
			}
            
            if(raycastHit.transform.TryGetComponent<CInteractable>(out var interactable) && interactable.isActiveAndEnabled) {
                this._currentInteractable.Value = interactable;
                return;
            }

            this._currentInteractable.Value = null;
        }
		
		private void TryToInteract() {
			if (!this._currentInteractable.Value) return;
			if (this._blockingEventsManager.IsAnyHappening) return;
			this._currentInteractable.Value.OnInteract(this._characterBase.transform);
		}
		
		#endregion <<---------- Private ---------->>
		
	}
}

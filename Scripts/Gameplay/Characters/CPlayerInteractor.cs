using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace CDK {
	public class CPlayerInteractor : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private CCharacterBase _characterBase;
		[SerializeField] private LayerMask _interactionLayerMask;
		[SerializeField] private float _interactionMaxDistance = 1.0f;
		[SerializeField] private Transform _rayOriginTransform;

		[NonSerialized] private float _interactionCapsuleCheckRadius = 0.10f;

		[NonSerialized] private ReactiveProperty<CInteractableObject> _currentInteractable;
		[NonSerialized] private Transform _transform;

		[NonSerialized] private bool _jumpNextFrame;
		
		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- Events ---------->>

		public event Action<CInteractableObject> OnInteractableChanged;

		#endregion <<---------- Events ---------->>
		
		
		

		#region <<---------- MonoBehaviour ---------->>
		private void Awake() {
			this._transform = this.transform;
			
			this._characterBase = this.GetComponent<CCharacterBase>();
			if (this._characterBase == null) {
				Debug.LogError($"Cant find any Character on {this.name}, removing component, character cannot interact with anything!");
				Destroy(this);
			}
		}

		private void OnEnable() {
			this._currentInteractable?.Dispose();
			this._currentInteractable = new ReactiveProperty<CInteractableObject>();
			this._currentInteractable.TakeUntilDisable(this).Subscribe(newInteractable => {
				if (newInteractable != null) {
					newInteractable.OnLookTo(this._transform);
				}
				this.OnInteractableChanged?.Invoke(newInteractable);
			});
			
		}
		
		private void Update() {

			if (CBlockingEventsManager.IsBlockingEventHappening) {
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
			var direction = this._rayOriginTransform.forward.normalized;

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
			
			this._currentInteractable.Value = raycastHit.transform.GetComponent<CInteractableObject>();
		}
		
		private void TryToInteract() {
			if (!this._currentInteractable.Value) return;
			this._currentInteractable.Value.OnInteract(this._characterBase.transform);
		}
		
		#endregion <<---------- Private ---------->>
		
	}
}

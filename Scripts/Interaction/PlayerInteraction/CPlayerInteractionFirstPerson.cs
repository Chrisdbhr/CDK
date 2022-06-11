using System;
using UnityEditor;
using UnityEngine;

namespace CDK.Interaction {
	public class CPlayerInteractionFirstPerson : CPlayerInteractionBase {
		
		[SerializeField] private Transform _originTransform;
		[NonSerialized] private float _maxInteractionDistance = 2.2f;
		[NonSerialized] private RaycastHit _hitInfo;


		#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			if (this._originTransform == null) return;
			Gizmos.color = Color.red;
			Gizmos.DrawRay(this._originTransform.position, this._originTransform.forward * this._maxInteractionDistance);
			var interactable = this.GetCollisionInteractable();
			if (interactable == null) return;
			Gizmos.color = Color.green;
			Gizmos.DrawLine(this._originTransform.position, this._hitInfo.point);
			Handles.Label(this._hitInfo.point, $"Interactable object: {this._hitInfo.transform.root.name}");
		}
		#endif

		public override void TryToInteract() {
			if (this._blockingEventsManager.IsAnyBlockingEventHappening) return;
			var interactable = this.GetCollisionInteractable();
			if (interactable == null) return;
			interactable.OnInteract(this.transform.root);

		}

		private ICInteractable GetCollisionInteractable() {
			var collided = Physics.Raycast(
				this._originTransform.transform.position,
				this._originTransform.forward,
				out this._hitInfo,
				this._maxInteractionDistance,
				this._interactionLayerMask,
				QueryTriggerInteraction.Collide
			);

			if (!collided) return null;

			return this._hitInfo.transform.GetComponent<ICInteractable>();
		}
	}
}

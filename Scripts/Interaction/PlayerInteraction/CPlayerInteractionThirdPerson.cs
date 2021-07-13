using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDK {
	public class CPlayerInteractionThirdPerson : CPlayerInteractionBase {

		[NonSerialized] private float _interactionSphereCheckRadius = 0.5f;
		[NonSerialized] private const float INTERACT_SPHERE_CHECK_MULTIPLIER = 0.75f;
		[NonSerialized] private float _yCheckOffset = 0.5f;
		
		#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			this._myTransform = this.transform;
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(
				this.GetCenterSphereCheckPosition(),
				this._interactionSphereCheckRadius);
		}
		#endif

		public override void TryToInteract() {
			if (this._blockingEventsManager.IsAnyBlockingEventHappening) return;

			var originPos = this.GetCenterSphereCheckPosition();

			var colliders = Physics.OverlapSphere(
				originPos,
				this._interactionSphereCheckRadius,
				this._interactionLayerMask,
				QueryTriggerInteraction.Collide
			);

			if (colliders == null || colliders.Length <= 0) return;

			// get list of interactables
			var interactableColliders = new List<Collider>();
			foreach (var col in colliders) {
				var interactable = col.GetComponent<CIInteractable>();
				if (interactable == null) continue;
				interactableColliders.Add(col);
			}

			if (interactableColliders.Count <= 0) return;

			originPos.x = this._myTransform.position.x;
			originPos.z = this._myTransform.position.z;

			// get closest interactable collider index
			int closestColliderIndex = 0;
			if (interactableColliders.Count == 1) closestColliderIndex = 0;
			else {
				float closestDistance = this._interactionSphereCheckRadius * 2f;
				for (int i = 0; i < interactableColliders.Count; i++) {
					float distance = (originPos - interactableColliders[i].transform.position).sqrMagnitude;
					if (distance >= closestDistance) continue;
					closestDistance = distance;
					closestColliderIndex = i;
				}
			}

			// get target interactable to try to interact
			var targetInteractableCollider = interactableColliders[closestColliderIndex];
			var direction = targetInteractableCollider.transform.position - originPos;
			
			bool hasSomethingBlockingLineOfSight = Physics.Raycast(
				originPos,
				direction,
				out var rayInfo,
				direction.magnitude,
				1,
				QueryTriggerInteraction.Collide
			);

			if (hasSomethingBlockingLineOfSight && rayInfo.collider != targetInteractableCollider) {
				Debug.Log($"{this.name} cant interact, {rayInfo.collider.name} is blocking line of sight.", rayInfo.collider);
				return;
			}

			var chosenInteractable = targetInteractableCollider.GetComponent<CIInteractable>();
			chosenInteractable.OnInteract(this._myTransform);
		}
		
		protected Vector3 GetCenterSphereCheckPosition() {
			return this._myTransform.position + this._myTransform.forward * (this._interactionSphereCheckRadius * INTERACT_SPHERE_CHECK_MULTIPLIER) + (Vector3.up * this._yCheckOffset);
		}
	}
}

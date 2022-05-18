using System.Linq;
using UnityEngine;

namespace CDK {
	public class CPlayerInteractionThirdPerson : CPlayerInteractionBase {

		#region <<---------- Properties and Fields ---------->>

		private float _interactionSphereCheckRadius = 0.4f;
		private float _yCheckOffset = 0.5f;

		#endregion <<---------- Properties and Fields ---------->>




		#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
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
			var interactableColliders = colliders.Where(c => c != null && c.GetComponent<CIInteractable>() != null).ToArray();
			if (interactableColliders.Length <= 0) return;
            
			originPos.x = this.transform.position.x;
			originPos.z = this.transform.position.z;

            // get closer interaction point
            interactableColliders = interactableColliders.OrderBy(c => (originPos - c.ClosestPoint(originPos)).sqrMagnitude).ToArray();
            
			// get target interactable to try to interact
            bool foundValidInteractable = false;
            Collider chosenInteractable = null;
            foreach (var interactableCol in interactableColliders) {
                var direction = (interactableCol.ClosestPoint(originPos) - originPos).normalized;

                var ray = new Ray(
                    originPos,
                    direction
                );
                
                #if UNITY_EDITOR
                Debug.DrawRay(ray.origin, ray.direction * (this._interactionSphereCheckRadius * 2f), Color.white, 3f);
                #endif

                bool hitSomething = Physics.Raycast(
                    ray,
                    out var rayInfo,
                    this._interactionSphereCheckRadius * 2f,
                    1,
                    QueryTriggerInteraction.Collide
                );

                if (!hitSomething) continue;

                foundValidInteractable = rayInfo.collider == interactableCol;

                #if UNITY_EDITOR
                Debug.DrawLine(ray.origin, rayInfo.point, foundValidInteractable ? Color.green : Color.yellow, 3f);
                #endif
                
                if (!foundValidInteractable) continue;
                chosenInteractable = interactableCol;
                break;
            }

            if (!foundValidInteractable) return;
            
            chosenInteractable.GetComponent<CIInteractable>().OnInteract(this.transform);
		}

		private Vector3 GetCheckHeight() {
			return this.transform.position + (this.transform.up * this._yCheckOffset);
		}

		protected Vector3 GetCenterSphereCheckPosition() {
			return this.GetCheckHeight() + (this.transform.forward * this._interactionSphereCheckRadius);
		}
	}
}

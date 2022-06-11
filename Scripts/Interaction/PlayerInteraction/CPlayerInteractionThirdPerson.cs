using System;
using System.Linq;
using UnityEngine;

namespace CDK.Interaction {
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
			var interactableColliders = colliders.Where(c => c != null && c.GetComponent<ICInteractable>() != null).ToArray();
			if (interactableColliders.Length <= 0) return;
            
			originPos.x = this.transform.position.x;
			originPos.z = this.transform.position.z;

            // get closer interaction point
            interactableColliders = interactableColliders.OrderBy(c => (originPos - this.GetScaledColliderCenterPosition(c)).sqrMagnitude).ToArray();
            
			// get target interactable to try to interact
            bool foundValidInteractable = false;
            Collider chosenInteractable = null;
            foreach (var c in interactableColliders) {
                bool isInsideInteractableCollider = IsPointInsideCollider(originPos, c);
                if (isInsideInteractableCollider) {
                    Debug.Log($"OriginPos is inside interaction collider '{c.name}', interacting with it.");
                    foundValidInteractable = true;
                    chosenInteractable = c;
                    break;
                }
                
                var direction = (this.GetScaledColliderCenterPosition(c) - originPos).normalized;

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
                    this._interactionLayerMask,
                    QueryTriggerInteraction.Collide
                );

                if (!hitSomething) continue;

                foundValidInteractable = rayInfo.collider == c;

                #if UNITY_EDITOR
                Debug.DrawLine(ray.origin, rayInfo.point, foundValidInteractable ? Color.green : Color.yellow, 3f);
                #endif
                
                if (!foundValidInteractable) continue;
                chosenInteractable = c;
                break;
            }

            if (!foundValidInteractable) return;
            
            chosenInteractable.GetComponent<ICInteractable>().OnInteract(this.transform);
		}

		private Vector3 GetCheckHeight() {
			return this.transform.position + (this.transform.up * this._yCheckOffset);
		}

		protected Vector3 GetCenterSphereCheckPosition() {
			return this.GetCheckHeight() + (this.transform.forward * this._interactionSphereCheckRadius);
		}

        protected Vector3 GetScaledColliderCenterPosition(Collider c) {
            var localScale = c.transform.localScale;
            var center = Vector3.zero;
            switch (c) {
                case BoxCollider bc:
                    center = bc.center;
                    break;
                case SphereCollider sc:
                    center = sc.center;
                    break;
                case CapsuleCollider cc:
                    center = cc.center;
                    break;
            }
            
            return c.transform.position + new Vector3(
                center.x * localScale.x,
                center.y * localScale.y,
                center.z * localScale.z
            );;
        }
        
        protected bool IsPointInsideCollider(Vector3 point, Collider c) {
            return c.bounds.Contains(point);
            // switch (c) {
            //     case BoxCollider boxCollider:
            //         return boxCollider.bounds.Contains(point);
            //     case SphereCollider sphereCollider:
            //         return sphereCollider.bounds.Contains(point);
            // }
            //
            // var isInside = c.bounds.Contains(point);
            // Debug.LogWarning($"{nameof(IsPointInsideCollider)} using Collider '{c.name}' bounds to check if contains point, this can be imprecise. IsInside: {isInside}", c);
            // return isInside;
        }
	}
}

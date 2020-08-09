using UnityEngine;

namespace CDK {
	public class CPlayerInteraction2D : CPlayerInteractionBase {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private float _interactionSphereCheckRadius = 1f;
		[SerializeField] private float _yInteractionCenterOffset = 0.5f;	

		#endregion <<---------- Properties and Fields ---------->>
		
		
		

		#region <<---------- MonoBehaviour ---------->>
		
		#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			this._myTransform = this.transform;
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(
				this.GetCenterCircleCheckPosition(),
				this._interactionSphereCheckRadius);
		}
		#endif

		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		#region <<---------- CPlayerInteractionBase ---------->>
		
		protected override void TryToInteract() {
			var allObjs = Physics2D.CircleCastAll(
				this.GetCenterCircleCheckPosition(), 
				this._interactionSphereCheckRadius,
				Vector2.zero, 
				this._interactionSphereCheckRadius, 
				this._interactionLayerMask
			);

			foreach (var obj in allObjs) {
				var interactable = obj.transform.GetComponent<CIInteractable>();
				if (interactable == null) continue;
				interactable.OnInteract(this.transform);
			}
		}
		
		#endregion <<---------- CPlayerInteractionBase ---------->>

		
		

		#region <<---------- General ---------->>

		private Vector2 GetCenterCircleCheckPosition() {
			return this.transform.position + new Vector3(0f, this._yInteractionCenterOffset);
		}
		
		#endregion <<---------- General ---------->>
		
	}
}

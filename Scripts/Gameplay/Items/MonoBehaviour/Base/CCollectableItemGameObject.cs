using System;
using CDK.Interaction;
using UnityEngine;

namespace CDK {
	[SelectionBase]
	public abstract class CCollectableItemGameObject : CInteractable {



		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private bool _collectOnCollision;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		private void OnCollisionEnter(Collision other) {
			if (!this._collectOnCollision) return;
			this.OnInteract(other.transform);
		}

		private void OnCollisionEnter2D(Collision2D other) {
			if (!this._collectOnCollision) return;
			this.OnInteract(other.transform);
		}
		
		#endregion <<---------- MonoBehaviour ---------->>
		
		
		

		#region <<---------- CIInteractable ---------->>
		public override void OnBecameInteractionTarget(Transform lookingTransform) {
			base.OnBecameInteractionTarget(lookingTransform);
		}

		public override bool OnInteract(Transform interactingTransform) {
            if (!base.OnInteract(interactingTransform)) return false;
			// try to get object
			var inventory = interactingTransform.root.GetComponent<CInventory>();
			if (inventory == null) {
				Debug.Log($"{interactingTransform}");
				return false;
			}

			bool itemCollected = false;

			switch (this.GetItemHere()) {
				case CWeaponData gun:
					itemCollected = inventory.AddItem(gun);
					break;
				case CAmmoData ammo:
					itemCollected = inventory.AddItem(ammo);
					break;
				default:
					Debug.LogError($"Could not add item to inventory because there is no switch case implemented for it.");
					break;
			}
			if (!itemCollected) return false;
			Destroy(this.gameObject);
			return true;
		}

		#endregion <<---------- CIInteractable ---------->>



		public abstract void SetItemHere(CIItemBase itemData);
		public abstract CIItemBase GetItemHere();
	}
}

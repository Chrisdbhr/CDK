using System;
using CDK.Interaction;
using UnityEngine;

namespace CDK {
	[SelectionBase]
	public abstract class CCollectableItemGameObject : CInteractable {



		#region <<---------- Properties and Fields ---------->>

		[SerializeField] bool _collectOnCollision;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		void OnCollisionEnter(Collision other) {
			if (!_collectOnCollision) return;
			OnInteract(other.transform);
		}

		void OnCollisionEnter2D(Collision2D other) {
			if (!_collectOnCollision) return;
			OnInteract(other.transform);
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

			switch (GetItemHere()) {
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
			Destroy(gameObject);
			return true;
		}

		#endregion <<---------- CIInteractable ---------->>



		public abstract void SetItemHere(CIItemBase itemData);
		public abstract CIItemBase GetItemHere();
	}
}

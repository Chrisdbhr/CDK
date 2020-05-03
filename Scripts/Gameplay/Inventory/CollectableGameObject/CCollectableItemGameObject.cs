using CDK.Inventory;
using UnityEngine;

namespace CDK {
	[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
	[SelectionBase]
	public abstract class CCollectableItemGameObject : MonoBehaviour, CIInteractable  {

		#region <<---------- CIInteractable ---------->>

		public void OnInteract(Transform interactingTransform) {
			if (interactingTransform == null) {
				Debug.Log($"Something interacted with {this.name} but interacting transform was null.");
				return;
			}
			// try to get object
			var inventory = interactingTransform.root.GetComponent<CInventory>();
			if (inventory == null) {
				Debug.Log($"{interactingTransform}");
				return;
			}

			bool itemCollected = false;

			switch (this.GetItemHere()) {
				case CGunData gun:
					itemCollected = inventory.AddItem(gun);
					break;
				case CAmmoData ammo:
					itemCollected = inventory.AddItem(ammo);
					break;
				default:
					Debug.LogError($"Could not add item to inventory because there is no switch case implemented for it.");
					break;
			}
			if (!itemCollected) return;
			Destroy(this.gameObject);
		}

		#endregion <<---------- CIInteractable ---------->>



		public abstract void SetItemHere(CItemBaseData itemData);
		public abstract CItemBaseData GetItemHere();
	}
}

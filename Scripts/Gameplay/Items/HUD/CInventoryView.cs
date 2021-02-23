using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDK {
	public class CInventoryView : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private CInventoryViewItem _prefabInventoryViewItem;
		[SerializeField] private Transform _itemsGridParent;

		[NonSerialized] private List<CInventoryViewItem> _inventoryViewItemList = new List<CInventoryViewItem>();
		[NonSerialized] private CInventory _inventory;
		[NonSerialized] public Action OnInventoryClose;
		
		#endregion <<---------- Properties and Fields ---------->>


		
		
		#region <<---------- MonoBehaviour ---------->>
		private void Awake() {
			// destroy all child on item grid parent
			foreach (var child in this._itemsGridParent.GetComponentsInChildren<Transform>()) {
				if (child == this._itemsGridParent.transform) continue;
				child.gameObject.CDestroy();
			}
		}

		private void Update() {
			if (Input.GetButtonDown(CInputKeys.MENU_PAUSE)) {
				this.Unpause();
			}
		}

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- Managment ---------->>

		public void Unpause() {
			this.OnInventoryClose?.Invoke();
			Destroy(this.gameObject);
		}
		
		#endregion <<---------- Managment ---------->>
		
		
		

		#region <<---------- Inventory Update ---------->>

		public void CreateInventoryViewItens(CInventory inventory) {
			this._inventory = inventory;
			
			this._inventoryViewItemList.Clear();
			// create new child game objects
			for (int i = 0; i < this._inventory.Size; i++) {
				this._inventoryViewItemList.Add(Instantiate(this._prefabInventoryViewItem.gameObject, this._itemsGridParent).GetComponent<CInventoryViewItem>());
			}
			this.UpdateInventoryView();
		}
		
		public void UpdateInventoryView() {
			
			// update itens on grid
			for (int i = 0; i < this._inventory.Size; i++) {

				var item = this._inventory.InventoryItems[i];
				this._inventoryViewItemList[i].UpdateSlotInfo(item != null ? this._inventory.InventoryItems[i] : null);
			}
		}
		
		#endregion <<---------- Inventory Update ---------->>

	}
}

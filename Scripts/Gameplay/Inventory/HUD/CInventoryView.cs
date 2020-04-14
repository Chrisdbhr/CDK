using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Linq;

namespace CDK {
	public class CInventoryView : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private CInventoryViewItem _prefabInventoryViewItem;
		[SerializeField] private Transform _itemsGridTransform;

		[NonSerialized] private List<CInventoryViewItem> _inventoryViewItemList = new List<CInventoryViewItem>();
		[NonSerialized] private CInventory _inventory;
		[NonSerialized] public Action OnInventoryClose;
		
		#endregion <<---------- Properties and Fields ---------->>


		
		
		#region <<---------- MonoBehaviour ---------->>
		
		private void Update() {
			if (Input.GetButtonDown(CInputKeys.MENU)) {
				this.OnInventoryClose?.Invoke();
				Destroy(this.gameObject);
			}
		}

		#endregion <<---------- MonoBehaviour ---------->>

		
		

		#region <<---------- Inventory Update ---------->>

		public void CreateInventoryViewItens(CInventory inventory) {
			this._inventory = inventory;
			
			this._inventoryViewItemList.Clear();
			// create new child game objects
			for (int i = 0; i < this._inventory.Size; i++) {
				this._inventoryViewItemList.Add(Instantiate(this._prefabInventoryViewItem.gameObject, this._itemsGridTransform).GetComponent<CInventoryViewItem>());
			}
			this.UpdateInventoryView();
		}
		
		public void UpdateInventoryView() {
			var viewItems = this._itemsGridTransform.gameObject.Children().ToArray();
			Debug.Log($"View itens count: {viewItems.Length}");
			
			// update itens on grid
			for (int i = 0; i < this._inventory.Size; i++) {

				var item = this._inventory.InventoryItems[i];
				this._inventoryViewItemList[i].UpdateSlotInfo(item != null ? this._inventory.InventoryItems[i] : null);
			}
		}
		
		#endregion <<---------- Inventory Update ---------->>

	}
}

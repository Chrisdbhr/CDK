using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDK {
	public class CInventoryView : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] CInventoryViewItem _prefabInventoryViewItem;
		[SerializeField] Transform _itemsGridParent;

		[NonSerialized] List<CInventoryViewItem> _inventoryViewItemList = new List<CInventoryViewItem>();
		[NonSerialized] CInventory _inventory;
		[NonSerialized] public Action OnInventoryClose;
		
		#endregion <<---------- Properties and Fields ---------->>


		
		
		#region <<---------- MonoBehaviour ---------->>

		void Awake() {
			// destroy all child on item grid parent
			foreach (var child in _itemsGridParent.GetComponentsInChildren<Transform>()) {
				if (child == _itemsGridParent.transform) continue;
				child.gameObject.CDestroy();
			}
		}

		void Update() {
			if (Input.GetButtonDown(CInputKeys.MENU_PAUSE)) {
				Unpause();
			}
		}

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- Managment ---------->>

		public void Unpause() {
			OnInventoryClose?.Invoke();
			Destroy(gameObject);
		}
		
		#endregion <<---------- Managment ---------->>
		
		
		

		#region <<---------- Inventory Update ---------->>

		public void CreateInventoryViewItens(CInventory inventory) {
			_inventory = inventory;
			
			_inventoryViewItemList.Clear();
			// create new child game objects
			for (int i = 0; i < _inventory.Size; i++) {
				_inventoryViewItemList.Add(Instantiate(_prefabInventoryViewItem.gameObject, _itemsGridParent).GetComponent<CInventoryViewItem>());
			}
			UpdateInventoryView();
		}
		
		public void UpdateInventoryView() {
			
			// update itens on grid
			for (int i = 0; i < _inventory.Size; i++) {

				var item = _inventory.InventoryItems[i];
				_inventoryViewItemList[i].UpdateSlotInfo(item != null ? _inventory.InventoryItems[i] : null);
			}
		}
		
		#endregion <<---------- Inventory Update ---------->>

	}
}

using System;
using UnityEngine;
using UnityEngine.UI;

namespace CDK {
	public class CInventoryViewItemOptionsMenu : MonoBehaviour {

		[SerializeField] private RectTransform _viewItensParent;
		[SerializeField] private Button _useItemButton;
		[SerializeField] private Button _equipItemButton;
		[SerializeField] private Button _examineButton;
		[SerializeField] private Button _discardItemButton;
		
		[NonSerialized] private CIItemBase _item;
		[NonSerialized] private int _itemIndex;
		[NonSerialized] private CInventory _inventory;
		

		#region <<---------- MonoBehaviour ---------->>
		private void Awake() {
			this._inventory = this.transform.root.GetComponent<CInventory>();
			if (this._inventory == null) {
				Debug.LogError($"ViewItemOption Could not find its root inventory.");
			}
		}

		private void Update() {
			if (Input.GetButtonDown(CInputKeys.INTERACT)) {
				this.Close();
			}
		}
		#endregion <<---------- MonoBehaviour ---------->>
		
		

		#region <<---------- Visibility ---------->>

		public void Open(CIItemBase itemBase, int itemIndex, Vector2 position) {
			this._viewItensParent.transform.position = position;
			
			this._item = itemBase;
			this._itemIndex = itemIndex;
			Debug.Log($"Opening {itemBase.GetScriptableObject().name} options. Itemindex is {itemIndex}.");
			
			// set visible buttons
			this._useItemButton.gameObject.SetActive(itemBase.GetScriptableObject().IsConsumable());
			this._equipItemButton.gameObject.SetActive(itemBase.GetScriptableObject() is CWeaponScriptableObject);
			this._examineButton.gameObject.SetActive(false);// TODO //this._examineButton.gameObject.SetActive(itemData.ItemMeshGameObject != null);
			this._discardItemButton.gameObject.SetActive(itemBase.GetScriptableObject().CanBeDropped);
			
			this.gameObject.SetActive(true);
		}

		private void Close() {
			Destroy(this.gameObject);
		}

		#endregion <<---------- Visibility ---------->>

		
		

		#region <<---------- Actions ---------->>

		public void UseItem() {
			this._inventory.UseOrEquipItem(this._itemIndex);
		}

		public  void ExamineItem() {
			this._inventory.ExamineItem(this._itemIndex);
		}

		public  void DropItem() {
			this._inventory.DiscardOrDropItem(this._itemIndex, true);
		}
		
		#endregion <<---------- Actions ---------->>

	}
}

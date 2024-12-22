using System;
using UnityEngine;
using UnityEngine.UI;

namespace CDK {
	public class CInventoryViewItemOptionsMenu : MonoBehaviour {

		[SerializeField] RectTransform _viewItensParent;
		[SerializeField] Button _useItemButton;
		[SerializeField] Button _equipItemButton;
		[SerializeField] Button _examineButton;
		[SerializeField] Button _discardItemButton;
		
		[NonSerialized] CIItemBase _item;
		[NonSerialized] int _itemIndex;
		[NonSerialized] CInventory _inventory;
		

		#region <<---------- MonoBehaviour ---------->>

		void Awake() {
			_inventory = transform.root.GetComponent<CInventory>();
			if (_inventory == null) {
				Debug.LogError($"ViewItemOption Could not find its root inventory.");
			}
		}

		void Update() {
			if (Input.GetButtonDown(CInputKeys.INTERACT)) {
				Close();
			}
		}
		#endregion <<---------- MonoBehaviour ---------->>
		
		

        
		#region <<---------- Visibility ---------->>

		public void Open(CIItemBase itemBase, int itemIndex, Vector2 position) {
			_viewItensParent.transform.position = position;
			
			_item = itemBase;
			_itemIndex = itemIndex;
			Debug.Log($"Opening {itemBase.GetScriptableObject().name} options. Itemindex is {itemIndex}.");
			
			// set visible buttons
			_useItemButton.gameObject.SetActive(itemBase.GetScriptableObject().IsConsumable());
			_equipItemButton.gameObject.SetActive(itemBase.GetScriptableObject() is CWeaponScriptableObject);
			_examineButton.gameObject.SetActive(false);// TODO //this._examineButton.gameObject.SetActive(itemData.ItemMeshGameObject != null);
			_discardItemButton.gameObject.SetActive(itemBase.GetScriptableObject().CanBeDropped);
			
			gameObject.SetActive(true);
		}

		void Close() {
			Destroy(gameObject);
		}

		#endregion <<---------- Visibility ---------->>

		
		

		#region <<---------- Actions ---------->>

		public void UseItem() {
			_inventory.UseOrEquipItem(_itemIndex);
		}

		public  void ExamineItem() {
			_inventory.ExamineItem(_itemIndex);
		}

		public  void DropItem() {
			_inventory.DiscardOrDropItem(_itemIndex, true);
		}
		
		#endregion <<---------- Actions ---------->>

	}
}

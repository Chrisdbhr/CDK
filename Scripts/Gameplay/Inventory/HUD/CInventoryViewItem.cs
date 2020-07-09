using System;
using CDK.Inventory;
using CDK.Weapons;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDK {
	public class CInventoryViewItem : MonoBehaviour, ISubmitHandler {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private CInventoryViewItemOptionsMenu _itemOptionsPrefab;
		[SerializeField] private TextMeshProUGUI _nameText;
		[SerializeField] private TextMeshProUGUI _quantityText;
		[SerializeField] private Image _thumbnailSprite;
	
		[NonSerialized] private readonly Color _thumbColor = new Color(1,1,1, 0.9f);
		[NonSerialized] private CItemBaseData _itemHere;
		
		#endregion <<---------- Properties and Fields ---------->>

		


		public void UpdateSlotInfo(CItemBaseData item) {
			this._itemHere = item;

			bool slotEmpty = item == null;
			
			// is an empty slot
			if (this._quantityText) {
				this._quantityText.text = item != null ? (item is CGunData gunScriptObj ? gunScriptObj.GetAmmoCount().ToString() : item.Count.ToString()) : string.Empty;
			}
			if (this._thumbnailSprite) {
				this._thumbnailSprite.color = slotEmpty ? Color.clear : this._thumbColor;
				this._thumbnailSprite.sprite = !slotEmpty && item.ScriptableObject ? item.ScriptableObject.ItemThumbnail : null;
			}
			if (this._nameText != null) {
				this._nameText.text = !slotEmpty  && item.ScriptableObject ? item.ScriptableObject.ItemName : string.Empty;
				this._nameText.gameObject.SetActive(!slotEmpty);
			}
		}

		private void OpenItemOptionsMenu() {
			if (this._itemHere == null) return;
			var optionMenu = Instantiate(this._itemOptionsPrefab, this.transform.parent.parent).GetComponent<CInventoryViewItemOptionsMenu>();
			optionMenu.Open(this._itemHere, this.transform.GetSiblingIndex(), this.transform.position);
		}
		

		#region <<---------- Event Handlers ---------->>

		public void OnSubmit(BaseEventData eventData) {
			this.OpenItemOptionsMenu();
		}
		
		#endregion <<---------- Event Handlers ---------->>
		
	}
}

using System;
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
		[NonSerialized] private CIItemBase _itemHere;
		
		#endregion <<---------- Properties and Fields ---------->>

		


		public void UpdateSlotInfo(CIItemBase item) {
			this._itemHere = item;

			bool slotEmpty = item == null;
			
			if (this._quantityText != null) {
				if (item == null) {
					this._quantityText.text = string.Empty;
				}
				else {
					if (item is CWeaponData weaponData) {
						this._quantityText.text = weaponData.IsLoadedWithInfiniteAmmo() ? string.Empty : weaponData.GetAmmoCount().ToString();
					}
					else {
						this._quantityText.text = item.Count.ToString();
					}
				}
			}
			if (this._thumbnailSprite) {
				this._thumbnailSprite.color = slotEmpty ? Color.clear : this._thumbColor;
				this._thumbnailSprite.sprite = !slotEmpty && item.GetScriptableObject() ? item.GetScriptableObject().ItemThumbnail : null;
			}
			if (this._nameText != null) {
				this._nameText.text = !slotEmpty  && item.GetScriptableObject() ? item.GetScriptableObject().ItemName : string.Empty;
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

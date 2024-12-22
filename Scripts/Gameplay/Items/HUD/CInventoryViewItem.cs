using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDK {
	public class CInventoryViewItem : MonoBehaviour, ISubmitHandler {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] CInventoryViewItemOptionsMenu _itemOptionsPrefab;
		[SerializeField] TextMeshProUGUI _nameText;
		[SerializeField] TextMeshProUGUI _quantityText;
		[SerializeField] Image _thumbnailSprite;
	
		[NonSerialized] readonly Color _thumbColor = new Color(1,1,1, 0.9f);
		[NonSerialized] CIItemBase _itemHere;
		
		#endregion <<---------- Properties and Fields ---------->>

		


		public void UpdateSlotInfo(CIItemBase item) {
			_itemHere = item;

			bool slotEmpty = item == null;
			
			if (_quantityText != null) {
				if (item == null) {
					_quantityText.text = string.Empty;
				}
				else {
					if (item is CWeaponData weaponData) {
						_quantityText.text = weaponData.IsLoadedWithInfiniteAmmo() ? string.Empty : weaponData.GetAmmoCount().ToString();
					}
					else {
						_quantityText.text = item.Count.ToString();
					}
				}
			}
			if (_thumbnailSprite) {
				_thumbnailSprite.color = slotEmpty ? Color.clear : _thumbColor;
				_thumbnailSprite.sprite = !slotEmpty && item.GetScriptableObject() ? item.GetScriptableObject().ItemThumbnail : null;
			}
			if (_nameText != null) {
				_nameText.text = !slotEmpty  && item.GetScriptableObject() ? item.GetScriptableObject().ItemName : string.Empty;
				_nameText.gameObject.SetActive(!slotEmpty);
			}
		}

		void OpenItemOptionsMenu() {
			if (_itemHere == null) return;
			var optionMenu = Instantiate(_itemOptionsPrefab, transform.parent.parent).GetComponent<CInventoryViewItemOptionsMenu>();
			optionMenu.Open(_itemHere, transform.GetSiblingIndex(), transform.position);
		}
		

		#region <<---------- Event Handlers ---------->>

		public void OnSubmit(BaseEventData eventData) {
			OpenItemOptionsMenu();
		}
		
		#endregion <<---------- Event Handlers ---------->>
		
	}
}

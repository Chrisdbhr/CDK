using UnityEngine;

namespace CDK {
	public abstract class CItemBaseScriptableObject : ScriptableObject {
		public string ItemName {
			get { return _itemName; }
		}
		[Header("Item information")]
		[SerializeField] string _itemName = "?";

		public string ItemDescription {
			get { return _itemDescription; }
		}
		[SerializeField] string _itemDescription = "?";
		
		public GameObject ItemMeshWhenDropped {
			get { return _itemMeshWhenDropped; }
		}
		[SerializeField] GameObject _itemMeshWhenDropped;
		
		public bool CanBeDropped {
			get { return _canBeDropped; }
		}
		[SerializeField] bool _canBeDropped = true;

		public Sprite ItemThumbnail {
			get { return _itemThumbnail; }		
		}
		[SerializeField] Sprite _itemThumbnail;

	}

	public static class CInventoryItemDataBaseExtensions {
		public static bool IsConsumable(this CItemBaseScriptableObject value) {
			Debug.Log($"TODO IsConsumable is not yet implemented.");
			return false;
		}
	}
}

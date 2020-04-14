using System;
using UnityEngine;

namespace CDK {
	public abstract class CItemBaseScriptableObject : ScriptableObject {
		public string ItemName {
			get { return this._itemName; }
		}
		[SerializeField] private string _itemName = "?";

		public string ItemDescription {
			get { return this._itemDescription; }
		}
		[SerializeField] private string _itemDescription = "?";
		
		public GameObject ItemMeshWhenDropped {
			get { return this._itemMeshWhenDropped; }
		}
		[SerializeField] private GameObject _itemMeshWhenDropped;
		
		public bool CanBeDropped {
			get { return this._canBeDropped; }
		}
		[SerializeField] private bool _canBeDropped = true;

		public Sprite ItemThumbnail {
			get { return this._itemThumbnail; }		
		}
		[SerializeField] private Sprite _itemThumbnail;

	}

	public static class CInventoryItemDataBaseExtensions {
		public static bool IsConsumable(this CItemBaseScriptableObject value) {
			Debug.Log($"TODO IsConsumable is not yet implemented.");
			return false;
		}
	}
}

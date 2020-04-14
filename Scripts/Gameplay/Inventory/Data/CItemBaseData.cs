using UnityEngine;

namespace CDK.Inventory {
	[System.Serializable]
	public abstract class CItemBaseData {
		public CItemBaseScriptableObject ScriptableObject;

		public int Count {
			get { return this._count; }
			set {
				this._count = value < 0 ? 0 : value;
			}
		}
		[SerializeField] private int _count;
	}
}

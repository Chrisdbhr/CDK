using CDK.Weapons;
using UnityEngine;

namespace CDK.Inventory {
	[System.Serializable]
	public class CGunData : CWeaponBaseData {
	
		[SerializeField] public CAmmoData EquippedAmmoData;

		public CGunData(CGunScriptableObject item, int quantity) {
			this.ScriptableObject = item;
			this.Count = quantity;
		}

		public bool HasAmmo() {
			return this.EquippedAmmoData != null && this.EquippedAmmoData.Count > 0;
		}

		public int GetAmmoCount() {
			return this.EquippedAmmoData?.Count ?? 0;
		}

	}
}

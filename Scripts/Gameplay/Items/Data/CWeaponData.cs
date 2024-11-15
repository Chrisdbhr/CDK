using UnityEngine;

namespace CDK {
	[System.Serializable]
	public class CWeaponData : CIItemBase {
		
		#region <<---------- Initializers ---------->>

		public CWeaponData(CWeaponScriptableObject item, int quantity) {
			this._weaponScriptableObject = item;
			this.Count = quantity;
		}

		#endregion <<---------- Initializers ---------->>

		
		
		
		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private CWeaponScriptableObject _weaponScriptableObject;
		public CAmmoData EquippedAmmoData;

		public int Count {
			get { return this._count; }
			private set {
				this._count = value < 0 ? 0 : value;
			}
		}
		private int _count;
		

		#endregion <<---------- Properties and Fields ---------->>

		
		
		
		#region <<---------- CIItemBase ---------->>
		
		public CItemBaseScriptableObject GetScriptableObject() {
			return this._weaponScriptableObject;
		}
		
		public int Add(int quantity) {
			return this.Count += quantity;
		}

		public int Remove(int quantity) {
			return this.Count -= quantity;
		}

		#endregion <<---------- CIItemBase ---------->>
		
		
		
	
		public bool HasAmmo() {
			if (this.EquippedAmmoData == null) return false;
			if (this.EquippedAmmoData.GetScriptableObject() is CAmmoScriptableObject ammoScriptObj && ammoScriptObj.IsInfinite) return true;
			return this.EquippedAmmoData.Count > 0;
		}

		public bool IsLoadedWithInfiniteAmmo() {
			if (this.EquippedAmmoData == null) return false;
			return this.EquippedAmmoData.GetScriptableObject() is CAmmoScriptableObject ammoScriptObj && ammoScriptObj.IsInfinite;
		}
		
		public int GetAmmoCount() {
			return this.EquippedAmmoData?.Count ?? 0;
		}
		
	}
}

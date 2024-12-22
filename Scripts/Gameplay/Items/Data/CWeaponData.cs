using UnityEngine;

namespace CDK {
	[System.Serializable]
	public class CWeaponData : CIItemBase {
		
		#region <<---------- Initializers ---------->>

		public CWeaponData(CWeaponScriptableObject item, int quantity) {
			_weaponScriptableObject = item;
			Count = quantity;
		}

		#endregion <<---------- Initializers ---------->>

		
		
		
		#region <<---------- Properties and Fields ---------->>

		[SerializeField] CWeaponScriptableObject _weaponScriptableObject;
		public CAmmoData EquippedAmmoData;

		public int Count {
			get { return _count; }
			private set {
				_count = value < 0 ? 0 : value;
			}
		}
		int _count;
		

		#endregion <<---------- Properties and Fields ---------->>

		
		
		
		#region <<---------- CIItemBase ---------->>
		
		public CItemBaseScriptableObject GetScriptableObject() {
			return _weaponScriptableObject;
		}
		
		public int Add(int quantity) {
			return Count += quantity;
		}

		public int Remove(int quantity) {
			return Count -= quantity;
		}

		#endregion <<---------- CIItemBase ---------->>
		
		
		
	
		public bool HasAmmo() {
			if (EquippedAmmoData == null) return false;
			if (EquippedAmmoData.GetScriptableObject() is CAmmoScriptableObject ammoScriptObj && ammoScriptObj.IsInfinite) return true;
			return EquippedAmmoData.Count > 0;
		}

		public bool IsLoadedWithInfiniteAmmo() {
			if (EquippedAmmoData == null) return false;
			return EquippedAmmoData.GetScriptableObject() is CAmmoScriptableObject ammoScriptObj && ammoScriptObj.IsInfinite;
		}
		
		public int GetAmmoCount() {
			return EquippedAmmoData?.Count ?? 0;
		}
		
	}
}

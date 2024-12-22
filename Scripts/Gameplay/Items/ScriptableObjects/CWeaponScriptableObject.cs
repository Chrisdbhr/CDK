
using UnityEngine;

namespace CDK {
	[CreateAssetMenu(fileName = "weapon_", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "Weapon data", order = 51)]
	public class CWeaponScriptableObject : CEquipableBaseScriptableObject {
		
		public CWeaponGameObject EquippedWeaponPrefab {
			get { return _equippedWeaponPrefab; }
		}
		[SerializeField] CWeaponGameObject _equippedWeaponPrefab;
	
		
		// ammo
		public CAmmoType SupportedAmmo {
			get { return supportedAmmo; }
		}
		[Header("Ammo")]
		[SerializeField] CAmmoType supportedAmmo;

		public CAmmoScriptableObject InitiallyLoadedAmmo {
			get { return _initiallyLoadedAmmoData; }	
		}
		[SerializeField] CAmmoScriptableObject _initiallyLoadedAmmoData;
	
		public float AmmoRateOfFire {
			get { return _ammoRateOfFire; }
		}
		[SerializeField] float _ammoRateOfFire;
		
		public int MaxAmmo {
			get { return _maxAmmo; }
		}
		[SerializeField] int _maxAmmo;
		
		public bool IsAutoFire {
			get { return _isAutoFire; }
		}
		
		[SerializeField] bool _isAutoFire;

		public bool AmmoSpawnAsChild {
			get { return ammoSpawnAsChild; }
		}
		[SerializeField] bool ammoSpawnAsChild;

		// recoil
		public float RecoilDefault {
			get { return _recoilDefault; }
		}
		[Header("Recoil")]
		[SerializeField] float _recoilDefault;
		
		public float RecoilChangeOnFire {
			get { return _recoilChangeOnFire; }
		}
		[SerializeField] float _recoilChangeOnFire;

		public float RecoilMaxValue {
			get { return _recoilMaxValue; }
		}
		[SerializeField] float _recoilMaxValue;
		
		public float RecoilRecoverRate {
			get { return _recoilRecoverRate; }
		}
		[SerializeField] float _recoilRecoverRate;

	}
}

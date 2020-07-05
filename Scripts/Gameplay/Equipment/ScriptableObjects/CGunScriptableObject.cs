using CDK.Weapons.Enums;
using UnityEngine;

namespace CDK.Weapons {
	
	[CreateAssetMenu(fileName = "Gun data", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "Gun data", order = 51)]
	public class CGunScriptableObject : CWeaponBaseScriptableObject {
		
		// ammo
		public CAmmoType SupportedAmmo {
			get { return this.supportedAmmo; }
		}
		[Header("Ammo")]
		[SerializeField] private CAmmoType supportedAmmo;

		public CAmmoScriptableObject InitiallyLoadedAmmo {
			get { return this._initiallyLoadedAmmoData; }	
		}
		[SerializeField] private CAmmoScriptableObject _initiallyLoadedAmmoData;
		
		public int MaxAmmo {
			get { return this._maxAmmo; }
		}
		[SerializeField] private int _maxAmmo;
		
		public bool IsAutoFire {
			get { return _isAutoFire; }
		}
		
		[SerializeField] private bool _isAutoFire;

		public bool AmmoSpawnAsChild {
			get { return this.ammoSpawnAsChild; }
		}
		[SerializeField] private bool ammoSpawnAsChild;

		// recoil
		public float RecoilDefault {
			get { return this._recoilDefault; }
		}
		[Header("Recoil")]
		[SerializeField] private float _recoilDefault;
		
		public float RecoilChangeOnFire {
			get { return this._recoilChangeOnFire; }
		}
		[SerializeField] private float _recoilChangeOnFire;

		public float RecoilMaxValue {
			get { return this._recoilMaxValue; }
		}
		[SerializeField] private float _recoilMaxValue;
		
		public float RecoilRecoverRate {
			get { return this._recoilRecoverRate; }
		}
		[SerializeField] private float _recoilRecoverRate;

	}
}

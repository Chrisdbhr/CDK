using CDK.Damage;
using CDK.Data;
using CDK.Weapons.Enums;
using UnityEngine;

namespace CDK.Weapons {
	[CreateAssetMenu(fileName = "Ammo data", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "Ammo data", order = 51)]
	public class CAmmoScriptableObject : CItemBaseScriptableObject, CIDamageDealer {
		public CHitInfoData HitInfo {
			get { return this.hitInfo; }
		}
		[SerializeField] private CHitInfoData hitInfo;

		public CAmmoType CAmmoType {
			get { return this.cAmmoType; }
		}
		[SerializeField] private CAmmoType cAmmoType;


		public CProjectileType CProjectileType {
			get { return this.cProjectileType; }
		}
		[SerializeField] private CProjectileType cProjectileType;
	}
}

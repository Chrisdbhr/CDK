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

		public GameObject ProjectilePrefabToSpawn {
			get { return this._projectilePrefabToSpawn;  }
		}
		[SerializeField] private GameObject _projectilePrefabToSpawn;

		public float ProjectileInitialSpeed {
			get { return this._projectileInitialSpeed; }
		}
		[SerializeField] private float _projectileInitialSpeed;
	
		public float ProjectileLifetime {
			get { return this._projectileLifetime; }
		}
		[SerializeField] private float _projectileLifetime;

		public float SelfRepulsionMultiplier {
			get { return this._selfRepulsionMultiplier; }
		}
		[SerializeField] private float _selfRepulsionMultiplier = 10f;

	}
}

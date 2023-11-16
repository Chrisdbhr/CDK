using CDK.Damage;
using CDK.Data;
using UnityEngine;

namespace CDK {
	[CreateAssetMenu(fileName = "ammo_", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "Ammo data", order = 51)]
	public class CAmmoScriptableObject : CItemBaseScriptableObject, ICDamageDealer {
		public CHitInfoData HitInfo {
			get { return this.hitInfo; }
		}
		[SerializeField] private CHitInfoData hitInfo;

		public CAmmoType CAmmoType {
			get { return this._ammoType; }
		}
		[SerializeField] private CAmmoType _ammoType;


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
		[SerializeField] private float _projectileInitialSpeed = 1f;
	
		public float ProjectileLifetime {
			get { return this._projectileLifetime; }
		}
		[SerializeField] private float _projectileLifetime = 0.1f;

		public bool IsInfinite {
			get { return this._isInfinite; }
		}
		[SerializeField] private bool _isInfinite;

		public float SelfRepulsionMultiplier {
			get { return this._selfRepulsionMultiplier; }
		}
		[SerializeField] private float _selfRepulsionMultiplier = 10f;

	}
}

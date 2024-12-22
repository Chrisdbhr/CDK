using CDK.Damage;
using CDK.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace CDK {
	[CreateAssetMenu(fileName = "ammo_", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "Ammo data", order = 51)]
	public class CAmmoScriptableObject : CItemBaseScriptableObject, ICDamageDealer {
		public CAttackData AttackData {
			get { return attackData; }
		}
		[FormerlySerializedAs("attack")] [FormerlySerializedAs("hitInfo")] [SerializeField]
		CAttackData attackData;

		public CAmmoType CAmmoType {
			get { return _ammoType; }
		}
		[SerializeField] CAmmoType _ammoType;


		public CProjectileType CProjectileType {
			get { return cProjectileType; }
		}
		[SerializeField] CProjectileType cProjectileType;

		public GameObject ProjectilePrefabToSpawn {
			get { return _projectilePrefabToSpawn;  }
		}
		[SerializeField] GameObject _projectilePrefabToSpawn;

		public float ProjectileInitialSpeed {
			get { return _projectileInitialSpeed; }
		}
		[SerializeField] float _projectileInitialSpeed = 1f;
	
		public float ProjectileLifetime {
			get { return _projectileLifetime; }
		}
		[SerializeField] float _projectileLifetime = 0.1f;

		public bool IsInfinite {
			get { return _isInfinite; }
		}
		[SerializeField] bool _isInfinite;

		public float SelfRepulsionMultiplier {
			get { return _selfRepulsionMultiplier; }
		}
		[SerializeField] float _selfRepulsionMultiplier = 10f;

	}
}

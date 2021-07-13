using System;
using CDK.Damage;
using UnityEngine;

namespace CDK.Weapons {
	public class CPlayerAttackController : MonoBehaviour {
		[SerializeField] private CCharacterBase _characterBase;
		[SerializeField] private CInventory _inventory;
		[SerializeField] private CAim _aim;
		[SerializeField] private Animator _characterAnimator;
		[NonSerialized] private CBlockingEventsManager _blockingEventsManager;

		
		protected readonly int ANIM_ATTACK = Animator.StringToHash("attack");

		
		
		
		#region <<---------- MonoBehaviour ---------->>
		private void Awake() {
			this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
		}

		private void Update() {
			if (Input.GetButtonDown(CInputKeys.ATTACK) && 
				this._characterBase.IsAiming &&
				this._inventory.EquippedWeapon != null &&
				!this._blockingEventsManager.IsAnyBlockingEventHappening
				) {
				if (this._inventory.EquippedWeapon is CWeaponData gun) {
					if (!gun.HasAmmo()) {
						// no ammo
						Debug.Log($"TODO empty magazine sound");
						return;
					}
					// gun fire
					var usedAmmo = this._inventory.GunShootConsumeAmmo();
					if (usedAmmo == null) {
						Debug.LogError($"Had ammo but could not fire the gun!");
						return;
					}
					
					// set attack animation
					this._characterAnimator.SetTrigger(this.ANIM_ATTACK);
					
					// bullet spawn or raycast
					if (usedAmmo.GetScriptableObject() is CAmmoScriptableObject ammo) {
						switch (ammo.CProjectileType) {
							case CProjectileType.raycast:
								bool hitSomething = Physics.Raycast(
									this._aim.transform.position,
									this._aim.transform.forward,
									out var hitInfo,
									Mathf.Infinity,
									1,
									QueryTriggerInteraction.Ignore
								);
								if (hitSomething) {
									
									// prevent self damage
									var rootHitCharacter = hitInfo.transform.root.GetComponent<CCharacterBase>();
									if (rootHitCharacter != null && rootHitCharacter == this._characterBase) {
										Debug.Log($"Will not apply damage to character itself.");
										return;
									}
									
									// set hit info properties
									ammo.HitInfo.AttackerTransform = this._characterBase.transform;
									ammo.HitInfo.HitPointPosition = hitInfo.point;

									float damage = ammo.HitInfo.ScriptableObject.Damage;
									
									// apply force
									var rigidbody = hitInfo.collider.GetComponent<Rigidbody>();
									if (rigidbody != null) {
										rigidbody.AddForceAtPosition(
											this._aim.transform.forward * damage,
											hitInfo.point,
											ForceMode.Impulse
										);
									}

									// apply damage
									hitInfo.collider.SendMessage(nameof(ICDamageable.TakeDamage), ammo.HitInfo, SendMessageOptions.DontRequireReceiver);
									Debug.Log($"TODO spawn effect on point when hit and play sound");
								}
								break;
							case CProjectileType.linearProjectile:
								//// apply explosive force
								// var allColliderInArea = Physics.OverlapSphere(
								// 	hitInfo.point,
								// 	damage * 0.01f,
								// 	GameSettings.get.AttackableLayers,
								// 	QueryTriggerInteraction.Ignore
								// );
								// if (allColliderInArea != null && allColliderInArea.Length > 0) {
								//
								// 	// iterate over all self bodies
								// 	foreach (var col in allColliderInArea) {
								// 		if (col == null || col.transform.root != hitInfo.transform.root) continue;
								// 		var rigidbody = col.transform.GetComponent<Rigidbody>();
								// 		if (rigidbody != null) {
								// 			rigidbody.AddForceAtPosition(
								// 				this._aim.transform.forward * ammo.HitInfo.ScriptableObject.Damage,
								// 				hitInfo.point,
								// 				ForceMode.Impulse
								// 			);
								// 		}
								// 	}
								// 		
								// }
								break;
							case CProjectileType.launchableProjectile:
								break;
							default:
								Debug.LogError($"Not implemented bullet spawn of {ammo.CProjectileType}!");
								break;
						}
					}
				}
				else {
					Debug.Log($"TODO Implement MELEE weapon");
				}
			}
		}
		#endregion <<---------- MonoBehaviour ---------->>

	}
}

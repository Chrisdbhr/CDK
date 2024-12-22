using System.Collections.Generic;
using CDK.Damage;
using CDK.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace CDK {
	public class CDamageDealerTrigger : MonoBehaviour, ICDamageDealer {

		#region <<---------- Initializers ---------->>
		
		public void Initialize(CAttackData attack, float damageMultiplier = 1f) {
			attackData = attack;

		}
		
		#endregion <<---------- Initializers ---------->>
		
		

		
		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private bool _debug;
		public CAttackData AttackData => attackData;
		[FormerlySerializedAs("attack")] [FormerlySerializedAs("_hitInfo")] [SerializeField] private CAttackData attackData;

		private enum DestroyType {
			dontDestroy,
			onAnyCollisionOrTrigger,
			onlyIfDidDamage
		}
		[SerializeField] private DestroyType _destroyType = DestroyType.onAnyCollisionOrTrigger;

		private List<ICDamageable> _damageds = new List<ICDamageable>();

		[Min(0.01f)] public float DamageMultiplier = 1f;
		
		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- MonoBehaviour ---------->>

		private void OnEnable() {
			_damageds.Clear();
		}

		private void OnTriggerEnter(Collider other) {
			if(_debug) Debug.Log($"'{name}' OnTriggerEnter '{other.name}'");
			DoDamageOnContact(other);
		}

		private void OnCollisionEnter(Collision other) {
			if(_debug) Debug.Log($"'{name}' OnCollisionEnter '{other.transform.name}'");
			DoDamageOnContact(other.collider);
		}

		private void OnTriggerEnter2D(Collider2D other) {
			if(_debug) Debug.Log($"'{name}' OnTriggerEnter2D '{other.name}'");
			DoDamageOnContact(other);
		}

		private void OnCollisionEnter2D(Collision2D other) {
			if(_debug) Debug.Log($"'{name}' OnCollisionEnter2D '{other.transform.name}'");
			DoDamageOnContact(other.collider);
		}

		private void OnControllerColliderHit(ControllerColliderHit hit) {
			if(_debug) Debug.Log($"'{name}' OnControllerColliderHit '{hit.transform.name}'");
			DoDamageOnContact(hit.collider);
		}
		
		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		private void DoDamageOnContact(Component go) {
			if(_debug) Debug.Log($"'{name}' starting {nameof(DoDamageOnContact)} in '{go.name}'");
			if (!go.TryGetComponent<ICDamageable>(out var damageable)) {
				if (_destroyType == DestroyType.onAnyCollisionOrTrigger) {
					gameObject.CDestroy();
				}
				return;
			}

			if (_destroyType == DestroyType.dontDestroy) {
				damageable.TakeHit(attackData.data, attackData.AttackerTransform, DamageMultiplier);
			} else if (!_damageds.Contains(damageable)) {
				_damageds.Add(damageable);
				damageable.TakeHit(attackData.data, attackData.AttackerTransform, DamageMultiplier);
				if (_destroyType == DestroyType.onlyIfDidDamage) {
					gameObject.CDestroy();
				}
			}

		}
		
	}
}
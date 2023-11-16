using System.Collections.Generic;
using CDK.Damage;
using CDK.Data;
using UnityEngine;

namespace CDK {
	public class CDamageDealerTrigger : MonoBehaviour, ICDamageDealer {

		#region <<---------- Initializers ---------->>
		
		public void Initialize(CHitInfoData hitInfo, Transform attackerTransform) {
			_hitInfo = hitInfo;
			_hitInfo.AttackerTransform = attackerTransform;
		}
		
		#endregion <<---------- Initializers ---------->>
		
		

		
		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private bool _debug;
		public CHitInfoData HitInfo => _hitInfo;
		[SerializeField] private CHitInfoData _hitInfo;

		private enum DestroyType {
			dontDestroy,
			onAnyCollisionOrTrigger,
			onlyIfDidDamage
		}
		[SerializeField] private DestroyType _destroyType = DestroyType.onAnyCollisionOrTrigger;

		private List<ICDamageable> _damageds = new List<ICDamageable>();
		
		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- MonoBehaviour ---------->>

		private void OnEnable() {
			_damageds.Clear();
		}

		private void OnTriggerEnter(Collider other) {
			if(_debug) Debug.Log($"'{this.name}' OnTriggerEnter '{other.name}'");
			DoDamageOnContact(other);
		}

		private void OnCollisionEnter(Collision other) {
			if(_debug) Debug.Log($"'{this.name}' OnCollisionEnter '{other.transform.name}'");
			DoDamageOnContact(other.collider);
		}

		private void OnTriggerEnter2D(Collider2D other) {
			if(_debug) Debug.Log($"'{this.name}' OnTriggerEnter2D '{other.name}'");
			DoDamageOnContact(other);
		}

		private void OnCollisionEnter2D(Collision2D other) {
			if(_debug) Debug.Log($"'{this.name}' OnCollisionEnter2D '{other.transform.name}'");
			DoDamageOnContact(other.collider);
		}

		private void OnControllerColliderHit(ControllerColliderHit hit) {
			if(_debug) Debug.Log($"'{this.name}' OnControllerColliderHit '{hit.transform.name}'");
			DoDamageOnContact(hit.collider);
		}
		
		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		private void DoDamageOnContact(Component go) {
			if(_debug) Debug.Log($"'{this.name}' starting {nameof(DoDamageOnContact)} in '{go.name}'");
			if (!go.TryGetComponent<ICDamageable>(out var damageable)) {
				if (_destroyType == DestroyType.onAnyCollisionOrTrigger) {
					this.gameObject.CDestroy();
				}
				return;
			}

			if (_destroyType == DestroyType.dontDestroy) {
				damageable.TakeHit(_hitInfo);
			} else if (!_damageds.Contains(damageable)) {
				_damageds.Add(damageable);
				damageable.TakeHit(_hitInfo);
				if (_destroyType == DestroyType.onlyIfDidDamage) {
					this.gameObject.CDestroy();
				}
			}

		}
		
	}
}
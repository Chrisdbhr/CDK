using System;
using CDK.Damage;
using CDK.Data;
using UnityEngine;

namespace CDK {
	public class DamageDealerMonoBehaviour : MonoBehaviour, CIDamageDealer {

		#region <<---------- Initializers ---------->>

		public void Initialize(Transform ownerAttacker) {
			this._rootAttacker = ownerAttacker;
		}
		
		#endregion <<---------- Initializers ---------->>

		
		
		
		#region <<---------- Properties and Fields ---------->>
		
		public CHitInfoData HitInfo {
			get {
				return this._hitInfo;
			}
		}
		[SerializeField] private CHitInfoData _hitInfo;

		[SerializeField] private bool _destroyOnCollide;
		[SerializeField] private Transform _rootAttacker;
		

		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- MonoBehaviour ---------->>
		
		private void OnTriggerEnter(Collider other) {
			if (other.transform.root == this._rootAttacker) return;
			this.DoDamage(other.gameObject);
		}

		private void OnCollisionEnter(Collision other) {
			if (other.transform.root == this._rootAttacker) return;
			bool didDamage = this.DoDamage(other.gameObject);
			if (!didDamage) return;
			if (this._destroyOnCollide) {
				this.gameObject.CDestroy();
			}
		}
		
		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		private bool DoDamage(GameObject go) {
			var damageable = go.GetComponent<CIDamageable>();
			if (damageable == null) return false;
			damageable.TakeDamage(this._hitInfo);
			if (this._destroyOnCollide) {
				this.gameObject.CDestroy();
			}
			return true;
		}
		
	}
}

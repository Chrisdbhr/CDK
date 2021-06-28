using CDK.Damage;
using CDK.Data;
using UnityEngine;

namespace CDK {
	public class CDamageDealerTrigger : MonoBehaviour, ICDamageDealerItem {


		#region <<---------- Initializers ---------->>
		
		public void Initialize(CHitInfoData hitInfo, Transform attackerTransform) {
			this._hitInfo = hitInfo;
			this._hitInfo.AttackerTransform = attackerTransform;
		}
		
		#endregion <<---------- Initializers ---------->>
		
		

		
		#region <<---------- Properties and Fields ---------->>
		
		public CHitInfoData HitInfo {
			get {
				return this._hitInfo;
			}
		}
		[SerializeField] private CHitInfoData _hitInfo;

		[SerializeField] private bool _isTrigger;

		private enum DestroyType {
			dontDestroy,
			onAnyCollisionOrTrigger,
			onlyIfDidDamage
		}
		[SerializeField] private DestroyType _destroyType = DestroyType.onAnyCollisionOrTrigger;

		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- MonoBehaviour ---------->>
		
		private void OnTriggerEnter(Collider other) {
			if (!this._isTrigger) return;
			var rootTransform = other.transform.root;
			if (rootTransform == this._hitInfo.AttackerTransform) return;
			this.DoDamageOnContact(rootTransform);
		}

		private void OnCollisionEnter(Collision other) {
			if (this._isTrigger) return;
			var rootTransform = other.transform.root;
			if (rootTransform == this._hitInfo.AttackerTransform) return;
			this.DoDamageOnContact(rootTransform);
		}

		private void OnTriggerEnter2D(Collider2D other) {
			if (!this._isTrigger) return;
			var rootTransform = other.transform.root;
			if (rootTransform == this._hitInfo.AttackerTransform) return;
			this.DoDamageOnContact(rootTransform);
		}

		private void OnCollisionEnter2D(Collision2D other) {
			if (this._isTrigger) return;
			var rootTransform = other.transform.root;
			if (rootTransform == this._hitInfo.AttackerTransform) return;
			this.DoDamageOnContact(rootTransform);
		}
		
		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		private void DoDamageOnContact(Component go) {
			var damageable = go.GetComponent<ICDamageable>();
			if (damageable != null) {
				damageable.TakeDamage(this._hitInfo);
				if (this._destroyType == DestroyType.onlyIfDidDamage) {
					this.gameObject.CDestroy();
					return;
				}
			}
			if (this._destroyType == DestroyType.onAnyCollisionOrTrigger) {
				this.gameObject.CDestroy();
			}
		}
		
	}
}

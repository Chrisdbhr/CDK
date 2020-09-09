using CDK.Damage;
using CDK.Data;
using UnityEngine;

namespace CDK {
	public class CDamageDealerMonoBehaviour : MonoBehaviour, ICDamageDealerItem {


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

		[SerializeField] private bool _destroyOnCollide;
		

		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- MonoBehaviour ---------->>
		
		private void Awake() {
			
		}

		private void OnTriggerEnter(Collider other) {
			var rootTransform = other.transform.root;
			if (rootTransform == this._hitInfo.AttackerTransform) return;
			this.DoDamage(rootTransform);
		}

		private void OnCollisionEnter(Collision other) {
			var rootTransform = other.transform.root;
			if (rootTransform == this._hitInfo.AttackerTransform) return;
			this.DoDamage(rootTransform);
		}

		private void OnTriggerEnter2D(Collider2D other) {
			var rootTransform = other.transform.root;
			if (rootTransform == this._hitInfo.AttackerTransform) return;
			this.DoDamage(rootTransform);
		}

		private void OnCollisionEnter2D(Collision2D other) {
			var rootTransform = other.transform.root;
			if (rootTransform == this._hitInfo.AttackerTransform) return;
			this.DoDamage(rootTransform);
		}
		
		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		private void DoDamage(Component go) {
			var damageable = go.GetComponent<ICDamageable>();
			if (damageable == null) return;
			damageable.TakeDamage(this._hitInfo);
			if (this._destroyOnCollide) {
				this.gameObject.CDestroy();
			}
		}
		
	}
}
using System;
using CDK.Damage;
using CDK.Data;
using UnityEngine;

namespace CDK {
	public class CDamageDealerTrigger : MonoBehaviour, ICDamageDealerItem {


		#region <<---------- Initializers ---------->>
		
		public void Initialize(CHitInfoData hitInfo, Transform attackerTransform) {
			this._hitInfo = hitInfo;
			this._hitInfo.AttackerRootTransform = attackerTransform;
		}
		
		#endregion <<---------- Initializers ---------->>
		
		

		
		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private bool _debug;
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
		
		private void Awake() {
			if (this._hitInfo.AttackerRootTransform == null) {
				var root = this.transform.root;
				//Debug.Log($"'{this.name}' DamageDealer auto setting AttackerRootTransform as '{root.name}' because it was null on Awake()");
				this._hitInfo.AttackerRootTransform = root;
			}
		}

		private void OnTriggerEnter(Collider other) {
			if(this._debug) Debug.Log($"'{this.name}' OnTriggerEnter '{other.name}'");
			if (!this._isTrigger) return;
			if (other.transform.root == this._hitInfo.AttackerRootTransform) return;
			this.DoDamageOnContact(other);
		}

		private void OnCollisionEnter(Collision other) {
			if(this._debug) Debug.Log($"'{this.name}' OnCollisionEnter '{other.transform.name}'");
			if (this._isTrigger) return;
			if (other.transform.root == this._hitInfo.AttackerRootTransform) return;
			this.DoDamageOnContact(other.collider);
		}

		private void OnTriggerEnter2D(Collider2D other) {
			if(this._debug) Debug.Log($"'{this.name}' OnTriggerEnter2D '{other.name}'");
			if (!this._isTrigger) return;
			if (other.transform.root == this._hitInfo.AttackerRootTransform) return;
			this.DoDamageOnContact(other);
		}

		private void OnCollisionEnter2D(Collision2D other) {
			if(this._debug) Debug.Log($"'{this.name}' OnCollisionEnter2D '{other.transform.name}'");
			if (this._isTrigger) return;
			if (other.transform.root == this._hitInfo.AttackerRootTransform) return;
			this.DoDamageOnContact(other.collider);
		}

		private void OnControllerColliderHit(ControllerColliderHit hit) {
			if(this._debug) Debug.Log($"'{this.name}' OnControllerColliderHit '{hit.transform.name}'");
			if (hit.transform.root == this._hitInfo.AttackerRootTransform) return;
			this.DoDamageOnContact(hit.collider);
		}
		
		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		private void DoDamageOnContact(Component go) {
			if(this._debug) Debug.Log($"'{this.name}' starting {nameof(DoDamageOnContact)} in '{go.name}'");
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

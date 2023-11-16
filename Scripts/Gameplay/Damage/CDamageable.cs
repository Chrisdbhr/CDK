using System;
using CDK.Damage;
using CDK.Data;
using UnityEngine;

namespace CDK {
	public class CDamageable : MonoBehaviour, ICDamageable {

		public CHealthComponent HealthComponent => this._healthToNotify;
		[SerializeField] private CHealthComponent _healthToNotify;
		[SerializeField] private CUnityEvent _takeDamageEvent;
		[SerializeField] private float _damageMultiplier = 1f;
		
		
		
		
		public event Action<CHitInfoData> OnHit {
			add {
				this._onHit += value;
			}
			remove {
				this._onHit -= value;
				this._onHit += value;
			}
		}
		private Action<CHitInfoData> _onHit;

		
		
		
		public bool TakeHit(CHitInfoData hitInfo, float damageMultiplier) {
			this._onHit?.Invoke(hitInfo);
			if (this._healthToNotify != null) {
				this._healthToNotify.TakeDamage(hitInfo, this._damageMultiplier * damageMultiplier);
			}
			this._takeDamageEvent?.Invoke();
			return _healthToNotify != null && _healthToNotify.IsDead;
		}
		
	}
}
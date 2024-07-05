using System;
using CDK.Damage;
using CDK.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace CDK {
	public class CHitbox : MonoBehaviour, ICDamageable {

		public CHealthComponent HealthComponent => this._healthToNotify;
		[SerializeField] private CHealthComponent _healthToNotify;
		[FormerlySerializedAs("_takeDamageEvent")] [SerializeField] private CUnityEvent _takeHitEvent;
		[SerializeField] private float _damageMultiplier = 1f;
		public float DamageMultiplier => _damageMultiplier;
		
		
		
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

		
		
		
		public float TakeHit(CHitInfoData attack, Transform attacker, float damageMultiplier) {
			this._onHit?.Invoke(attack);
			float damage = attack.RawDamage;
			if (this._healthToNotify != null) {
				damage = this._healthToNotify.TakeHit(attack, attacker, this._damageMultiplier * damageMultiplier);
			}
			this._takeHitEvent?.Invoke();
			return damage;
		}
		
	}
}
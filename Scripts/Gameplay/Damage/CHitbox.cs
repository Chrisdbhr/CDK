using System;
using CDK.Damage;
using CDK.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace CDK {
	public class CHitbox : MonoBehaviour, ICDamageable {

		public const float CriticalMultiplier = 1.5f;

		public virtual CHealthComponent Health => _healthToNotify;
		[SerializeField] CHealthComponent _healthToNotify;
		[FormerlySerializedAs("_takeDamageEvent")] [SerializeField] private CUnityEvent _takeHitEvent;
		public bool IsCriticalHitbox => _isCriticalHitbox;
		[SerializeField] bool _isCriticalHitbox;

		
		
		public event Action<CHitInfoData> OnHit {
			add {
				_onHit += value;
			}
			remove {
				_onHit -= value;
				_onHit += value;
			}
		}
		private Action<CHitInfoData> _onHit;

		
		
		
		public virtual float TakeHit(CHitInfoData attack, Transform attacker, float damageMultiplier) {
			_onHit?.Invoke(attack);
			float damage = attack.RawDamage;
			if (Health != null) {
				damage = Health.TakeHit(attack, attacker, GetDamageMultiplierForHit(attack) * damageMultiplier);
			}
			_takeHitEvent?.Invoke();
			return damage;
		}

		float GetDamageMultiplierForHit(CHitInfoData attack) {
			if (!attack.CanCritical) return 1f;
			return _isCriticalHitbox ? CriticalMultiplier : 1f;
		}
	}
}
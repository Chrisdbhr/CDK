using CDK.Damage;
using CDK.Data;
using UnityEngine;

namespace CDK {
	public class CDamageable : MonoBehaviour, ICDamageable {

		[SerializeField] private CHealthComponent _healthToNotify;
		[SerializeField] private CUnityEvent _takeDamageEvent;
		[SerializeField] private float _damageMultiplier = 1f;
		
		
		
		
		public bool TakeDamage(CHitInfoData hitInfo) {
			hitInfo.DamageMultiplier = _damageMultiplier;
			if (_healthToNotify != null) _healthToNotify.TakeDamage(hitInfo);
			_takeDamageEvent?.Invoke();
			return true;
		}
		
	}
}

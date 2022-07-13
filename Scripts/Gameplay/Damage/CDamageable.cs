using CDK.Damage;
using CDK.Data;
using UnityEngine;

namespace CDK {
	public class CDamageable : MonoBehaviour, ICDamageable {

		[SerializeField] private CHealthComponent _healthToNotify;
		[SerializeField] private CUnityEvent _takeDamageEvent;
		[SerializeField] private float _damageMultiplier = 1f;
		
		
		
		
		public bool TakeDamage(CHitInfoData hitInfo, float damageMultiplier) {
			if (this._healthToNotify != null) this._healthToNotify.TakeDamage(hitInfo, this._damageMultiplier * damageMultiplier);
			this._takeDamageEvent?.Invoke();
			return true;
		}
		
	}
}

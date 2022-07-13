using CDK.Data;

namespace CDK.Damage {
	public interface ICDamageable {
		/// <summary>
		/// Returns TRUE if damaged with success.
		/// </summary>
		bool TakeDamage(CHitInfoData hitInfo, float damageMultiplier = 1f);
	}
}

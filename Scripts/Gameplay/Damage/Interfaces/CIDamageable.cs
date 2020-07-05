using CDK.Data;

namespace CDK.Damage {
	public interface CIDamageable {
		/// <summary>
		/// Returns TRUE if damaged with success.
		/// </summary>
		bool TakeDamage(CHitInfoData hitInfo);
	}
}

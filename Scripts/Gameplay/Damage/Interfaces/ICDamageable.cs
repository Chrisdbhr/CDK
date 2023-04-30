using CDK.Data;

namespace CDK.Damage {
	public interface ICDamageable {
		/// <summary>
		/// Returns TRUE if toke hit.
		/// </summary>
		bool TakeHit(CHitInfoData hitInfo, float damageMultiplier = 1f);
	}
}

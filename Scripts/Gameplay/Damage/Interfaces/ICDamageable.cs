using CDK.Data;
using UnityEngine;

namespace CDK.Damage {
	public interface ICDamageable {
		public CHealthComponent Health { get; }
		/// <summary>
		/// Returns the amount of final damage taken.
		/// </summary>
		float TakeHit(CHitInfoData attack, Transform attackerTransform, float damageMultiplier = 1f);
	}
}
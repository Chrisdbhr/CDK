using System.Linq;
using UnityEngine;

namespace CDK {
	public static class CAnimatorExtensions {

		public static void CSetFloatWithLerp(this Animator self, int id, float target, float time) {
			if (self == null) return;
			target = target.CImprecise();
			var currentFloat = self.GetFloat(id);
			self.SetFloat(id, currentFloat.CLerp(target, time));
		}

		/// <summary>
		/// If Animator is null will not throw and exception, will ignore instruction instead.
		/// </summary>
		public static void CSetBoolSafe(this Animator self, int id, bool value) {
			if (self == null) return;
			self.SetBool(id, value);
		}
		
		/// <summary>
		/// If Animator is null will not throw and exception, will ignore instruction instead.
		/// </summary>
		public static void CSetFloatSafe(this Animator self, int id, float value) {
			if (self == null) return;
			self.SetFloat(id, value.CImprecise());
		}

}
}

using System.Linq;
using UnityEngine;

namespace CDK {
	public static class CAnimatorExtensions {

        /// <summary>
        /// Check if self is null and if was the parameter.
        /// </summary>
		public static void CSetFloatWithLerp(this Animator self, int id, float target, float time) {
			if (self == null) return;
            if (self.parameters.All(p => p.nameHash != id)) return;
			target = target.CImprecise();
			var currentFloat = self.GetFloat(id);
			self.SetFloat(id, currentFloat.CLerp(target, time).CImprecise());
		}

        /// <summary>
        /// Check if self is null and if was the parameter.
        /// </summary>
		public static void CSetBoolSafe(this Animator self, int id, bool value) {
			if (self == null) return;
            if (self.parameters.All(p => p.nameHash != id)) return;
			self.SetBool(id, value);
		}
		
		/// <summary>
		/// Check if self is null and if was the parameter.
		/// </summary>
		public static void CSetFloatSafe(this Animator self, int id, float value) {
			if (self == null) return;
            if (self.parameters.All(p => p.nameHash != id)) return;
			self.SetFloat(id, value.CImprecise());
		}
        
        /// <summary>
        /// Check if self is null and if was the parameter.
        /// </summary>
        public static void CSetTriggerSafe(this Animator self, int id) {
            if (self == null) return;
            if (self.parameters.All(p => p.nameHash != id)) return;
            self.SetTrigger(id);
        }

}
}

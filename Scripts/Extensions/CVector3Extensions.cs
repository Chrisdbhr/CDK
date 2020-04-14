using UnityEngine;

namespace CDK {
	public static class CVector3Extensions {

		public static float GetAbsBiggerValue(this Vector3 vec) {
			float biggerValue = Mathf.Abs(vec.x);
			float valueToCompare = Mathf.Abs(vec.y);
			if (valueToCompare > biggerValue) {
				biggerValue = valueToCompare;
			}
			valueToCompare = Mathf.Abs(vec.z);
			if (valueToCompare > biggerValue) {
				biggerValue = valueToCompare;
			}

			return biggerValue;
		}

	}
}
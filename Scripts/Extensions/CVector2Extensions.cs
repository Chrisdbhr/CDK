using UnityEngine;

namespace CDK {
	public static class CVector2Extensions {
		public static float GetAbsBiggerValue(this Vector2 vec) {
			float biggerValue = Mathf.Abs(vec.x);
			float valueToCompare = Mathf.Abs(vec.y);
			if (valueToCompare > biggerValue) {
				biggerValue = valueToCompare;
			}

			return biggerValue;
		}
	}
}
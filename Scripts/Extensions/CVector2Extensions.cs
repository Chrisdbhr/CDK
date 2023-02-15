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
        
        public static bool CImpreciseEqualCompare(this Vector2 a, Vector2 b) {
            return a.x.CImprecise() == b.x.CImprecise()
            && a.y.CImprecise() == b.y.CImprecise();
        }
        
        public static Vector2 CCastValuesToInt(this Vector2 a) {
            a.x = (int)a.x;
            a.y = (int)a.y;
            return a;
        }
	}
}
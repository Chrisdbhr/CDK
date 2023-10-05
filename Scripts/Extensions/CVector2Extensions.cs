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
        
        public static bool CIsZero(this Vector2 v) {
            return v.x == 0f && v.y == 0f;
        }
        
        public static bool CIsOne(this Vector2 v) {
            return v.x == 1f && v.y == 1f;
        }

        public static Vector2 CLerp(this Vector2 v, Vector2 other, float t) {
            v.x = Mathf.Lerp(v.x, other.x, t);
            v.y = Mathf.Lerp(v.y, other.y, t);
            return v;
        }

	}
}
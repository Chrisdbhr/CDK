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

		public static Vector3 GetCloserFrom(this Vector3[] positions, Vector3 from) {
			if (positions.Length <= 0) return default;
			if (positions.Length == 1) return positions[0];
			var closerPos = positions[0];
			float closerDist = Vector3.Distance(positions[0], from);
			for (int i = 1; i < positions.Length; i++) {
				var currentDistance = Vector3.Distance(positions[i], from);
				if (currentDistance >= closerDist) continue;
				closerPos = positions[i];
				closerDist = currentDistance;
			}
			return closerPos;
		}

        public static bool CImpreciseEqualCompare(this Vector3 a, Vector3 b) {
            return a.x.CImprecise() == b.x.CImprecise()
            && a.y.CImprecise() == b.y.CImprecise()
            && a.z.CImprecise() == b.z.CImprecise();
        }

        public static Vector3 CCastValuesToInt(this Vector3 v) {
            v.x = (int)v.x;
            v.y = (int)v.y;
            v.z = (int)v.z;
            return v;
        }
        
        public static Vector3 CAbs(this Vector3 v) {
            v.x = Mathf.Abs(v.x);
            v.y = Mathf.Abs(v.y);
            v.z = Mathf.Abs(v.z);
            return v;
        }
        
        public static bool CIsZero(this Vector3 v) {
            return v.x == 0f && v.y == 0f && v.z == 0f;
        }
        
        public static bool CIsOne(this Vector3 v) {
            return v.x == 1f && v.y == 1 && v.z == 1f;
        }

        public static float CMagnitudeXZ(this Vector3 v) {
            if (v.x == 0f && v.z == 0f) return 0f;
            return new Vector3(v.x, 0f, v.z).magnitude;
        }

        public static bool CClosestPointIsFirstParameter(this Vector3 originPos, Vector3 firstPos, Vector3 lastPos) {
            float d1 = Vector3.Distance(firstPos, originPos);
            float d2 = Vector3.Distance(lastPos, originPos);
            return d1 < d2;
        }

    }
}
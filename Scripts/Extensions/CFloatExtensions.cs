using UnityEngine;

namespace CDK
{
    public static class CFloatExtensions {
        /// <summary>
        /// Returns clamped angle between -360 and 360.
        /// </summary>
        public static float CClampAngle(this float value, float min, float max) {
            if (value < -360)
                value += 360;
            if (value > 360)
                value -= 360;
            return Mathf.Clamp(value, min, max);
        }

        public static float CRemap(this float value, float beginOld, float endOld, float beginNew, float endNew) {
            return (value - beginOld) / (endOld - beginOld) * (endNew - beginNew) + beginNew;
        }

        public static bool CIsInRange(this float value, float a, float b) {
            return value >= a && value <= b;
        }

        public static float CAbs(this float value) {
            return Mathf.Abs(value);
        }

        public static float CGetCloserValue(this float value, float a, float b) {
            float distanceFromA = (a - value).CAbs();
            float distanceFromB = (b - value).CAbs();
            return distanceFromA < distanceFromB ? a : b;
        }

        public static float CLerp(this float a, float b, float time) {
            return Mathf.Lerp(a, b, time);
        }
        
    }
}
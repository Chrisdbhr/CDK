using UnityEngine;

namespace CDK {
    public static class CIntExtensions {
        
        public static int CAbs(this int value) {
            return Mathf.Abs(value);
        }
        
        public static int CClamp(this int value, int min, int max) {
            return Mathf.Clamp(value, min, max);
        }
        
        public static int CClamp01(this int value) {
            return (int)Mathf.Clamp(value, 0f, 1f);
        }
        
    }
}
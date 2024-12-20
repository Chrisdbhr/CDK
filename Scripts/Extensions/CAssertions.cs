using UnityEngine;

namespace CDK
{
    public static class CAssertions
    {
        public static void CAssertIfFalse(this bool condition, string errorMessage = null, Object source = null)
        {
            if (condition) return;
            if (errorMessage == null) {
                Debug.LogError("Assertion failed", source);
                return;
            }

            Debug.LogError($"Assertion failed: {errorMessage}", source);
        }
    }
}
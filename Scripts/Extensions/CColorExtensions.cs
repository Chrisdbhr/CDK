using UnityEngine;

namespace CDK {
    public static class CColorExtensions {

        public static Color CRandomColor() {
            // Generate random RGB values between 0 and 1.
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);

            // Create and return a new Color object using the random RGB values.
            return new Color(r, g, b);
        }
        
    }
}
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

        public static Color CLerp(this Color c, Color target, float time) {
            return new Color(
                c.r.CLerp(target.r, time),
                c.g.CLerp(target.g, time),
                c.b.CLerp(target.b, time)
            );
        }
        
    }
}
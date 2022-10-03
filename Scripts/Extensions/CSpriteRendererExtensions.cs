using UnityEngine;

namespace CDK {
    public static class CSpriteRendererExtensions {
        
        /// <summary>
        /// Without checking for null, Try setting only the Alpha component of the color (0 is transparent, 1 is opaque).
        /// </summary>
        public static void CSetAlphaUnsafe(this SpriteRenderer sr, float targetAlpha) {
            var color = sr.color;
            color.a = targetAlpha;
            sr.color = color;
        }
        
        /// <summary>
        /// Check if not null and Set only the Alpha component of the color (0 is transparent, 1 is opaque).
        /// </summary>
        public static void CSetAlpha(this SpriteRenderer sr, float targetAlpha) {
            if (sr == null) return;
            var color = sr.color;
            color.a = targetAlpha;
            sr.color = color;
        }
    }
}
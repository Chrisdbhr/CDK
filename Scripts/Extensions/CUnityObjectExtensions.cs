using UnityEngine;

namespace CDK {
	public static class CUnityObjectExtensions {
		public static void CDestroy(this Object value) {
			if (value == null) return;
			if (Application.isPlaying) {
				Object.Destroy(value);
			}
			else {
				Object.DestroyImmediate(value);
			}
		}
	}
}

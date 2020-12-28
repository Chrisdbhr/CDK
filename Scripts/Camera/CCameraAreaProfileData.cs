using UnityEngine;

namespace CDK {
	public class CCameraAreaProfileData : ScriptableObject {

		public float fov = 60f;
		public float maxDistanceFromPlayer = 5f;
		public float RecoverFromWallSpeed = 1f;
		public float WallCheckOffset = 1f;
		public float FarClippingPlane = 500f;

	}
}
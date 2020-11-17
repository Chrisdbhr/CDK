using UnityEngine;

namespace CDK {
	public class CCameraAreaProfileData : ScriptableObject {

		public float fov = 60f;
		public float maxDistanceFromPlayer = 15f;
		public float RecoverFromWallSpeed = 10f;
		public float WallCheckOffset = 1f;
		public float FarClippingPlane = 500f;

	}
}
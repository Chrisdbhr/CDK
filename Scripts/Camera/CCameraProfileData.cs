using UnityEngine;

namespace CDK {
	public class CCameraProfileData : ScriptableObject {

		public float fieldOfView = 50f;
		public float maxDistanceFromPlayer = 5f;
		public float RecoverFromWallSpeed = 1f;
		public float WallCheckOffset = 1f;
		public float FarClippingPlane = 500f;

	}
}
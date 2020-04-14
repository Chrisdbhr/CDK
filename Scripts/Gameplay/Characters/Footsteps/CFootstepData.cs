using UnityEngine;

namespace CDK {
	[CreateAssetMenu(fileName = "FootstepData", menuName = "Data/Footstep data", order = 51)]
	public class CFootstepData : ScriptableObject {
		public AudioClip[] audiosHere;
		public ParticleSystem particleEffect;
	}

}

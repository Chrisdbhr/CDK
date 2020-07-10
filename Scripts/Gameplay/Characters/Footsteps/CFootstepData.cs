using UnityEngine;

namespace CDK {
	[CreateAssetMenu(fileName = "FootstepData", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "Footstep data", order = 51)]
	public class CFootstepData : ScriptableObject {
		public AudioClip[] audiosHere;
		public ParticleSystem particleEffect;
	}

}

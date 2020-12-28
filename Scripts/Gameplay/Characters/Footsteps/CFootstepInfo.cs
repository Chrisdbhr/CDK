using FMODUnity;
using UnityEngine;

namespace CDK {
	[System.Serializable]
	public class CFootstepInfo {
		public Material[] Materials;
		[EventRef]
		public string Audio;

		[SerializeField] private ParticleSystem[] _particleSystems;
		
		// TODO get random particle system
		public ParticleSystem GetRandomParticleSystem() {
			return this._particleSystems.RandomElement();
		}
	}
}

using UnityEngine;

#if FMOD
using FMODUnity;
#endif

namespace CDK {
	[System.Serializable]
	public class CFootstepInfo {
		
		public string Name;
		
		public float BloodAmount;
		
		public float WetAmount;
		
		#if FMOD
		public EventReference Audio;
		#else
		public string Audio;
		#endif
		
		public Material[] Materials;
		
		public TerrainLayer[] TerrainLayers;
		
		[SerializeField] private ParticleSystem[] _particleSystems;
		
		
		
		
		public ParticleSystem GetRandomParticleSystem() {
			return this._particleSystems.CRandomElement();
		}
		
	}
}

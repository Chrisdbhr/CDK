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
		[EventRef]
		#endif
		public string Audio;
		
		public Material[] Materials;
		
		public TerrainLayer[] TerrainLayers;
		
		[SerializeField] private ParticleSystem[] _particleSystems;
		
		
		
		
		public ParticleSystem GetRandomParticleSystem() {
			return this._particleSystems.CRandomElement();
		}
		
	}
}

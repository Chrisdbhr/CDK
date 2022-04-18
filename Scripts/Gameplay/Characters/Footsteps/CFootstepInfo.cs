using System;
using UnityEngine;

#if FMOD
using FMODUnity;
#endif

namespace CDK {
	[System.Serializable]
	public class CFootstepInfo {
		
		#if FMOD
		public EventReference Audio;
		#else
		public string Audio;
		#endif
		
        [SerializeField] private ParticleSystem[] _particleSystems;

        
        
        
        [Obsolete]
		public Material[] Materials;
		
        [Obsolete]
		public TerrainLayer[] TerrainLayers;
		
		
		
        
		
		public ParticleSystem GetRandomParticleSystem() {
			return this._particleSystems.CRandomElement();
		}
		
	}
}

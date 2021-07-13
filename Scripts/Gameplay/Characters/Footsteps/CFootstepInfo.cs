using FMODUnity;
using UnityEngine;

namespace CDK {
	[System.Serializable]
	public class CFootstepInfo {
		
		public string Name;
		public float BloodAmount;
		public float WetAmount;
		[EventRef] public string Audio;
		public Material[] Materials;
		public TerrainLayer[] TerrainLayers;
		[SerializeField] private ParticleSystem[] _particleSystems;
		
		public ParticleSystem GetRandomParticleSystem() {
			return this._particleSystems.RandomElement();
		}
		
	}
}

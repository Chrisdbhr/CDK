using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CDK {
	public class CFootstepsSource : MonoBehaviour {

		#region <<---------- Enums ---------->>
		
		public enum FootstepFeet {
			left, right
		}
		
		#endregion <<---------- Enums ---------->>
		
		
		
		
		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private bool _debugFootstep;
		[SerializeField] private Transform _footL;
		[SerializeField] private Transform _footR;
		[SerializeField] private LayerMask _footCollisionLayers = 1;

		[NonSerialized] private CFootstepDatabase _database;
		[NonSerialized] private float _rayOffset = 0.25f;
		[NonSerialized] private float _feetSizeForSphereCast = 0.1f;
		[NonSerialized] private Vector3 _lastValidHitPoint;

		[NonSerialized] private Dictionary<ParticleSystem, ParticleSystem> _spawnedParticleInstances = new Dictionary<ParticleSystem, ParticleSystem>();
		
		
		public event Action<CFootstepInfo, FootstepFeet> OnFootstep {
			add {
				this._onFootstep -= value;
				this._onFootstep += value;
			}
			remove {
				this._onFootstep -= value;
			}
		}
		private Action<CFootstepInfo, FootstepFeet> _onFootstep;

		#endregion <<---------- Properties and Fields ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>
		private void Awake() {
			this._database = CDependencyResolver.Get<CFootstepDatabase>();
		}

		private void OnEnable() {
			this.DestroyAllParticleInstances();
			SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
		}
		
		private void OnDisable() {
			this.DestroyAllParticleInstances();
		}

		#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			Gizmos.color = Color.white;
			if(this._footL) Gizmos.DrawWireSphere(this._footL.position, this._feetSizeForSphereCast);
			if(this._footR) Gizmos.DrawWireSphere(this._footR.position, this._feetSizeForSphereCast);
			Gizmos.DrawWireSphere(this._lastValidHitPoint, this._feetSizeForSphereCast);
		}

		private void OnValidate() {
			if (this.GetComponent<Animator>() == null) {
				Debug.LogError($"[{this.GetType().Name}] need to have also an Animator to receive Animation Trigger.");
			}
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- General ---------->>
		
		/// <summary>
		/// Do a footstep
		/// </summary>
		public void Footstep(FootstepFeet feet) {
			var originTransform = this.transform.root;
			
			var rayOrigin = feet == FootstepFeet.left ? (this._footL ? this._footL : originTransform) : (this._footR ? this._footR : originTransform);

			var transformUp = originTransform.up;
			bool feetHitSomething = Physics.SphereCast(
				rayOrigin.position + (transformUp * this._rayOffset),
				this._feetSizeForSphereCast,
				(transformUp * -1).normalized, // down
				out var raycastHit,
				2f * this._rayOffset,
				this._footCollisionLayers,
				QueryTriggerInteraction.Ignore
			);
			if (!feetHitSomething) return;

			this._lastValidHitPoint = raycastHit.point;
			
			if(this._debugFootstep) Debug.Log($"Footstep {feet} on {raycastHit.collider.name}");

			// check for smashable object
			var smashableObj = raycastHit.collider.GetComponent<CICanBeSmashedWhenStepping>();
			if (smashableObj != null) {
				smashableObj.Smash(this.transform.root, feet);
				return;
			}

			var footstepInfo = this.GetFootstepInfoFromMeshRenderer(raycastHit);
			if (footstepInfo == null) {
				// Try to get footstep info from a terrain
				footstepInfo = this.GetFootstepInfoFromTerrain(raycastHit);
				if (footstepInfo == null) return;
			}

			this._onFootstep?.Invoke(footstepInfo, feet);

			// new footstep particle effect.
			var particlePrefab = footstepInfo.GetRandomParticleSystem();
			if (particlePrefab != null) {
				var createdPartSystem = this.GetOrCreateParticleInstance(particlePrefab);
				if (createdPartSystem != null) {
					createdPartSystem.transform.position = raycastHit.point;
					
					var rot = Quaternion.FromToRotation(this.transform.up, raycastHit.normal);
					var euler = rot.eulerAngles;
					euler.y = this.transform.rotation.eulerAngles.y;
					createdPartSystem.transform.rotation = Quaternion.Euler(euler);
					createdPartSystem.Play(true);
				}
			}

			// play random audio
			if (footstepInfo.Audio.CIsNullOrEmpty()) return;
			
			#if FMOD
			FMODUnity.RuntimeManager.PlayOneShot(footstepInfo.Audio, raycastHit.point);
			#endif
		}

		private CFootstepInfo GetFootstepInfoFromMeshRenderer(RaycastHit raycastHit) {
			// check for mesh
			var meshFilter = raycastHit.collider.GetComponent<MeshFilter>();
			if (meshFilter == null) return null;
			var sharedMesh = meshFilter.sharedMesh;
			if (sharedMesh == null) return null;
			var rend = meshFilter.GetComponent<Renderer>();
			if (!rend) return null;

			var materialId = -1;

			if (!sharedMesh.isReadable || !(raycastHit.collider is MeshCollider)) {
				materialId = 0;
			}
			else {
				var triangleIndex = raycastHit.triangleIndex;
				if (triangleIndex <= -1) return null;
				var triangles = sharedMesh.triangles;
				int lookupIndex1 = triangles[triangleIndex * 3];
				int lookupIndex2 = triangles[triangleIndex * 3 + 1];
				int lookupIndex3 = triangles[triangleIndex * 3 + 2];
				var subMeshCount = sharedMesh.subMeshCount;

				// get material index
				for (int i = 0; i < subMeshCount; i++) {
					var tr = sharedMesh.GetTriangles(i);
					for (int j = 0; j < tr.Length; j++) {
						if (tr[j] != lookupIndex1 || 
							tr[j + 1] != lookupIndex2 || 
							tr[j + 2] != lookupIndex3) continue;
						materialId = i;
						break;
					}
					if (materialId != -1) break;
				}

				if (materialId == -1) return null;
			}

			var material = rend.sharedMaterials[materialId];
			if (!material) return null;

			return this._database.GetFootstepInfoByMaterial(material);
		}

		private CFootstepInfo GetFootstepInfoFromTerrain(RaycastHit raycastHit) {
			var terrain = raycastHit.collider.GetComponent<Terrain>();
			if (terrain == null) return null;
		
			var terrainTextureDetector = terrain.GetComponent<CTerrainTextureDetector>();
			if (terrainTextureDetector == null) {
				Debug.LogWarning($"{this.name} step on a Terrain without a {nameof(CTerrainTextureDetector)} attached to it to detect footstep!");
				return null;
			}

			var dominantLayer = terrainTextureDetector.GetFirstTextureAt(raycastHit.point);
			if (dominantLayer == null) return null;

			return this._database.GetFootstepInfoByTerrainLayer(dominantLayer);
		}
		
		#endregion <<---------- General ---------->>


		

		#region <<---------- Pooling ---------->>

		private ParticleSystem GetOrCreateParticleInstance(ParticleSystem key) {
			if (key == null) return null;
			if (this._spawnedParticleInstances.TryGetValue(key, out var particleSystem) && particleSystem != null) {
				return particleSystem;
			}
			var spawnedParticle = Instantiate(key);
			if (this._spawnedParticleInstances.ContainsKey(key)) {
				this._spawnedParticleInstances[key] = spawnedParticle;
			}
			else {
				this._spawnedParticleInstances.Add(key, spawnedParticle);
			}
			return spawnedParticle;
		}
		
		private void DestroyAllParticleInstances() {
			foreach (var spawnedParticleInstance in this._spawnedParticleInstances.Values) {
				if (spawnedParticleInstance == null) continue;
				Destroy(spawnedParticleInstance.gameObject);
			}
		}
		
		#endregion <<---------- Pooling ---------->>
		
		


		#region <<---------- Callbacks ---------->>
		
		private void OnActiveSceneChanged(Scene oldScene, Scene newScene) {
			this.DestroyAllParticleInstances();
		}

		#endregion <<---------- Callbacks ---------->>
		
	}
}

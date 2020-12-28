using System;
using FMODUnity;
using UnityEngine;

namespace CDK {
	public class CFootstepsSource : MonoBehaviour {

		#region <<---------- Enums ---------->>
		
		public enum FootstepFeet {
			left, right
		}
		
		#endregion <<---------- Enums ---------->>
		
		
		
		
		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private CFootstepDatabase _database;
		[SerializeField] private StudioEventEmitter _audioEmitter;
		[SerializeField] private Transform _footL;
		[SerializeField] private Transform _footR;
		[SerializeField] private LayerMask _footCollisionLayers = 1;
		[SerializeField] private Transform _defaultRayOriginTransform;

		[NonSerialized] private float _rayOffset = 0.25f;
		[NonSerialized] private Transform _currentFeet;
		[NonSerialized] private RaycastHit _raycastHit;
		[NonSerialized] private float _feetSizeForSphereCast = 0.1f;
		[NonSerialized] private Vector3 _lastValidHitPoint;

		#endregion <<---------- Properties and Fields ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>
		private void Awake() {
			if (!this._defaultRayOriginTransform) {
				Debug.Log($"[FootstepSource] {this.name} doest have an {nameof(this._defaultRayOriginTransform)} value. Setting as root transform.", this);
				this._defaultRayOriginTransform = this.transform.root;
			}
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
			var rayOrigin = feet == FootstepFeet.left ? (this._footL ? this._footL : this._defaultRayOriginTransform) : (this._footR ? this._footR : this._defaultRayOriginTransform);

			var transformUp = this._defaultRayOriginTransform.up;
			bool feetHitSomething = Physics.SphereCast(
				rayOrigin.position + (transformUp * this._rayOffset),
				this._feetSizeForSphereCast,
				(transformUp * -1).normalized, // down
				out this._raycastHit,
				2f * this._rayOffset,
				this._footCollisionLayers,
				QueryTriggerInteraction.Ignore
			);
			if (!feetHitSomething) return;

			this._lastValidHitPoint = this._raycastHit.point;

			// check for smashable object
			var smashableObj = this._raycastHit.collider.GetComponent<CICanBeSmashedWhenStepping>();
			if (smashableObj != null) {
				smashableObj.Smash();
				return;
			}

			// check for mesh
			var meshFilter = this._raycastHit.collider.GetComponent<MeshFilter>();
			if (meshFilter == null) return;
			var sharedMesh = meshFilter.sharedMesh;
			if (sharedMesh == null) return;
			var rend = meshFilter.GetComponent<Renderer>();
			if (!rend) return;

			var materialId = -1;

			if (!sharedMesh.isReadable || !(this._raycastHit.collider is MeshCollider)) {
				materialId = 0;
			}
			else {
				var triangleIndex = this._raycastHit.triangleIndex;
				if (triangleIndex <= -1) return;
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

				if (materialId == -1) return;
			}

			var material = rend.sharedMaterials[materialId];
			if (!material) return;

			var footstepInfo = this._database.GetFootstepInfo(material);
			if (footstepInfo == null) return;

			// new footstep particle effect.
			var particlePrefab = footstepInfo.GetRandomParticleSystem();
			if (particlePrefab != null) {
				var createdPartSystem = Instantiate(particlePrefab);
				if (createdPartSystem != null) {
					createdPartSystem.transform.position = this._raycastHit.point;
					createdPartSystem.transform.rotation = Quaternion.FromToRotation(Vector3.up, this._raycastHit.normal);
					Destroy(createdPartSystem.gameObject, particlePrefab.main.duration);
				}
			}

			// play random audio
			if (!footstepInfo.Audio.CIsNullOrEmpty()) {
				this._audioEmitter.Event = footstepInfo.Audio;
			}
		}
		
		#endregion <<---------- General ---------->>
		
	}
}

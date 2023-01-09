﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace CDK {
	public class CFootstepsSource : MonoBehaviour {

		#region <<---------- Enums ---------->>
		
		public enum FootstepFeet {
			left, right
		}
		
		#endregion <<---------- Enums ---------->>



        #region <<---------- Initializers ---------->>

        public void Initialize(LayerMask footCollisionLayers, Transform footL, Transform footR) {
            this.FootCollisionLayers = footCollisionLayers;
            this.FootL = footL;
            this.FootR = footR;
        }
        
        #endregion <<---------- Initializers ---------->>
		
		
		
		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private bool _debugFootstep;
		public Transform FootL;
        public Transform FootR;
		public LayerMask FootCollisionLayers = 1;

		private float _rayOffset = 0.25f;
        [SerializeField] private float _feetSizeForSphereCast = 0.05f;
		private Vector3 _lastValidHitPoint;

		private Dictionary<ParticleSystem, ParticleSystem> _spawnedParticleInstances = new Dictionary<ParticleSystem, ParticleSystem>();
		
		
		public event Action<CFootstepInfo, FootstepFeet, Collider> OnFootstep {
			add {
				this._onFootstep -= value;
				this._onFootstep += value;
			}
			remove {
				this._onFootstep -= value;
			}
		}
		private Action<CFootstepInfo, FootstepFeet, Collider> _onFootstep;

		#endregion <<---------- Properties and Fields ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>

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
			if(this.FootL) Gizmos.DrawWireSphere(this.FootL.position, this._feetSizeForSphereCast);
			if(this.FootR) Gizmos.DrawWireSphere(this.FootR.position, this._feetSizeForSphereCast);
			Gizmos.DrawWireSphere(this._lastValidHitPoint, this._feetSizeForSphereCast);
		}

		private void OnValidate() {
			if (this.GetComponent<Animator>() == null) {
				Debug.LogError($"[{this.GetType().Name}] need to have also an Animator to receive Animation Trigger.");
			}
		}
		
		private void Reset(){
		    this.FindFootTransformsIfNeeded();
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- General ---------->>

        /// <summary>
        /// Do a footstep
        /// </summary>
        public void Footstep(FootstepFeet feet) {
            this.CStartCoroutine(FootstepProcess(feet));
        }

        public IEnumerator FootstepProcess(FootstepFeet feet) {
            yield return new WaitForFixedUpdate();
			
            var originTransform = this.transform;
			
			var rayOriginTransform = feet == FootstepFeet.left ? (this.FootL != null ? this.FootL : originTransform) : (this.FootR != null ? this.FootR : originTransform);
            var transformUp = originTransform.up;
            var rayOrigin = rayOriginTransform.position + (transformUp * this._rayOffset);
            var rayDirectionNormalized = (transformUp * -1).normalized;
            var rayDistance = 2f * this._rayOffset;
            
            bool feetHitSomething = Physics.Raycast(
				rayOrigin,
				//this._feetSizeForSphereCast,
				rayDirectionNormalized, // down
				out var raycastHit,
				rayDistance,
				this.FootCollisionLayers,
				QueryTriggerInteraction.Ignore
			);
			if (!feetHitSomething) yield break;

            if (this._debugFootstep) {
                Debug.DrawRay(rayOrigin, rayDirectionNormalized * rayDistance, Color.red, 3f, true);                
            }

            this._lastValidHitPoint = raycastHit.point;
			
			if(this._debugFootstep) Debug.Log($"Footstep {feet} on {raycastHit.collider.name}");

			// check for smashable object
			var smashableObj = raycastHit.collider.GetComponent<CICanBeSmashedWhenStepping>();
			if (smashableObj != null) {
				smashableObj.Smash(originTransform, feet);
				yield break;
			}

            CFootstepInfo footstepInfo = this.GetFootstepInfoFromRaycastHit(raycastHit);
            if(footstepInfo == null) yield break;

			this._onFootstep?.Invoke(footstepInfo, feet, raycastHit.collider);

			// new footstep particle effect.
			var particlePrefab = footstepInfo.GetRandomParticleSystem();
			if (particlePrefab != null) {
				var createdPartSystem = this.GetOrCreateParticleInstance(particlePrefab);
				if (createdPartSystem != null) {
					createdPartSystem.transform.position = raycastHit.point;
					
					var rot = Quaternion.FromToRotation(originTransform.up, raycastHit.normal);
					var euler = rot.eulerAngles;
					euler.y = originTransform.rotation.eulerAngles.y;
					createdPartSystem.transform.rotation = Quaternion.Euler(euler);
					createdPartSystem.Play(true);
				}
			}

			#if FMOD
			// play random audio
            if (footstepInfo.Audio.IsNull) yield break;

			FMODUnity.RuntimeManager.PlayOneShot(footstepInfo.Audio, raycastHit.point);
			#endif
		}

        public void FindFootTransformsIfNeeded() {
            if(this.FootL == null) this.FootL = this.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name.ToLower().Contains("foot") && t.name.ToLower().Contains('l'));
            if(this.FootR == null) this.FootR = this.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name.ToLower().Contains("foot") && t.name.ToLower().Contains('r'));
        }

        private CFootstepInfo GetFootstepInfoFromRaycastHit(RaycastHit hit) {
            var t = hit.transform;
            if (t == null || t.parent == null) return null;
            var surface = t.GetComponent<CIFootstepSurfaceBase>();
            if (surface == null) {
                surface = t.parent.GetComponent<CFootstepSurfaceParent>();
            }
            return surface != null ? surface.GetFootstepInfoFromRaycastHit(hit) : null;
        }
		
		#endregion <<---------- General ---------->>


		

		#region <<---------- Pooling ---------->>

		private ParticleSystem GetOrCreateParticleInstance(ParticleSystem key) {
			if (key == null) return null;

            // if particle is already on scene dont need to spawn
            if (key.gameObject.scene != default) return key;
            
			if (this._spawnedParticleInstances.TryGetValue(key, out var particleSystem) && particleSystem != null) {
				return particleSystem;
			}
			var spawnedParticle = Instantiate(key);
            this._spawnedParticleInstances[key] = spawnedParticle;
			return spawnedParticle;
		}
		
		private void DestroyAllParticleInstances() {
			foreach (var spawnedParticleInstance in this._spawnedParticleInstances.Values) {
				if (spawnedParticleInstance == null) continue;
                spawnedParticleInstance.gameObject.CDestroy();
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

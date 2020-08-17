using System;
using System.Linq;
using CDK.Audio;
using UnityEngine;

namespace CDK {
	public class CFootstepsManager : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private CRandomAudioPlayer _randomAudioPlayer;
		[SerializeField] private Transform footL;
		[SerializeField] private Transform footR;
		[SerializeField] private LayerMask footColisionLayers = 1;
		[SerializeField] private Transform _defaultRayOriginTransform;
		
		[NonSerialized] private RaycastHit _raycastHit;
		[NonSerialized] private float _feetSizeForSphereCast = 0.1f;
		

		public enum FootstepFeet {
			left, right
		}
		
		#endregion <<---------- Properties and Fields ---------->>

		#region <<---------- Cache Vars ---------->>
		[NonSerialized] private Transform currentFeet;
		[NonSerialized] private CFootstepSource footstepSource;
		#endregion <<---------- Cache Vars ---------->>

		#region <<---------- MonoBehaviour ---------->>
		private void Awake() {
			if (!this._defaultRayOriginTransform) {
				Debug.Log($"[FootstepManager] {this.name} doest have an {nameof(this._defaultRayOriginTransform)} value. Setting as root transform.", this);
				this._defaultRayOriginTransform = this.transform.root;
			}
		}

		#if UNITY_EDITOR
		private void OnDrawGizmos() {
			Gizmos.color = Color.white;
			if(this.footL) Gizmos.DrawWireSphere(this.footL.position, this._feetSizeForSphereCast);
			if(this.footR) Gizmos.DrawWireSphere(this.footR.position, this._feetSizeForSphereCast);
		}

		private void OnValidate() {
			if (this.GetComponent<Animator>() == null) {
				Debug.LogError($"[{this.GetType().Name}] need to have also an Animator to receive Animation Trigger.");
			}
		}
		#endif
		#endregion <<---------- MonoBehaviour ---------->>

		/// <summary>
		/// Do a footstep
		/// </summary>
		public void Footstep(FootstepFeet feet) {
			var rayOrigin = feet == FootstepFeet.left ? (this.footL ? this.footL : this._defaultRayOriginTransform) : (this.footR ? this.footR : this._defaultRayOriginTransform);
				
			bool feetHitSomething = Physics.SphereCast(
				rayOrigin.position + Vector3.up,
				this._feetSizeForSphereCast,
				Vector3.down,
				out this._raycastHit,
				2f,
				this.footColisionLayers,
				QueryTriggerInteraction.Ignore
			);
			if (!feetHitSomething) return;

			// check for smashable object
			var smashableObj = this._raycastHit.collider.GetComponent<CICanBeSmashedWhenStepping>();
			if (smashableObj != null) {
				smashableObj.Smash();
				return;
			}

			// hit ground
			this.footstepSource = this._raycastHit.collider.GetComponent<CFootstepSource>();
			if (!this.footstepSource) return;
			if (!this.footstepSource.footstepDataHere) return;

			// new footstep particle effect.
			if (this.footstepSource.footstepDataHere.particleEffect != null) {
				var createdPartSystem = Instantiate(this.footstepSource.footstepDataHere.particleEffect);
				if (createdPartSystem != null) {
					createdPartSystem.transform.position = this._raycastHit.point;
					createdPartSystem.transform.rotation = Quaternion.FromToRotation(Vector3.up, this._raycastHit.normal);
					Destroy(createdPartSystem.gameObject, this.footstepSource.footstepDataHere.particleEffect.main.duration);
				}
			}

			// play random audio
			var audiosList = this.footstepSource.footstepDataHere.audiosHere;
			if (audiosList == null || audiosList.Count <= 0) return;
			this._randomAudioPlayer.SetAudioClips(audiosList);
			this._randomAudioPlayer.PlayAudio();
		}
	}
}

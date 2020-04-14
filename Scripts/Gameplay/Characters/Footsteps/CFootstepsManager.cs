using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CDK {
	public class CFootstepsManager : MonoBehaviour {
		#region <<---------- Properties and Fields ---------->>
		[SerializeField] private AudioSource audioSource;

		[SerializeField, FormerlySerializedAs("footstepSourcesLayers")]
		private LayerMask footColisionLayers = 1;

		[SerializeField] private Transform footL;
		[SerializeField] private Transform footR;
		[NonSerialized] private float _feetSizeForSphereCast = 0.1f;

		[NonSerialized] private Queue<int> _lastPlayedClipsIds = new Queue<int>();

		[NonSerialized] private RaycastHit _raycastHit;

		public enum FootstepFeet {
			left, right
		}
		#endregion <<---------- Properties and Fields ---------->>

		#region <<---------- Cache Vars ---------->>
		[NonSerialized] private Transform currentFeet;
		[NonSerialized] private CFootstepSource footstepSource;
		#endregion <<---------- Cache Vars ---------->>

		#region <<---------- MonoBehaviour ---------->>
		#if UNITY_EDITOR

		private void OnDrawGizmos() {
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(this.footL.position, this._feetSizeForSphereCast);
			Gizmos.DrawWireSphere(this.footR.position, this._feetSizeForSphereCast);
		}

		private void OnValidate() {
			if (this.transform.parent != null) {
				Debug.LogError($"[{this.GetType().Name}] need to be on root object to receive Animation Events!");
			}
		}
		#endif
		#endregion <<---------- MonoBehaviour ---------->>

		/// <summary>
		/// Do a footstep
		/// </summary>
		public void Footstep(FootstepFeet feet) {
			this.currentFeet = feet == FootstepFeet.left ? this.footL : this.footR;
			bool feetHitSomething = Physics.SphereCast(
				this.currentFeet.position + Vector3.up,
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

			if (this.footstepSource.footstepDataHere.audiosHere == null || this.footstepSource.footstepDataHere.audiosHere.Length <= 0) return;
			this.audioSource.Stop();
			this.audioSource.clip = this.SelectAudioFromList(this.footstepSource.footstepDataHere);
			this._lastPlayedClipsIds.Enqueue(this.audioSource.clip.GetInstanceID());
			this.audioSource.Play();
		}

		private AudioClip SelectAudioFromList(CFootstepData fData) {
			if (this._lastPlayedClipsIds.Count >= fData.audiosHere.Length * 0.5f) {
				while (this._lastPlayedClipsIds.Count >= (fData.audiosHere.Length * 0.5f)) {
					this._lastPlayedClipsIds.Dequeue();
				}
			}
			else {
				return fData.audiosHere.GetRandomElement();
			}

			var selected = fData.audiosHere.GetRandomElement();
			if (this._lastPlayedClipsIds.Contains(selected.GetInstanceID())) {
				selected = this.SelectAudioFromList(fData);
			}

			return selected;
		}
	}
}

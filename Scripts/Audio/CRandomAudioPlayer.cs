using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CDK.Audio {
	[RequireComponent(typeof(AudioSource))]
	public class CRandomAudioPlayer : MonoBehaviour {

		[SerializeField] private List<AudioClip> _audioClips = new List<AudioClip>();
		[NonSerialized] private readonly Queue<AudioClip> _lastPlayedClips = new Queue<AudioClip>();
		[NonSerialized] private AudioSource _audioSource;

		
		
		
		private void Awake() {
			this._audioSource = this.GetComponent<AudioSource>();
		}




		public void SetAudioClips(List<AudioClip> clips) {
			if (clips == this._audioClips) return;
			this._lastPlayedClips.Clear();
			this._audioClips = clips;
		}

		public void PlayAudio() {
			var audioClip = this.GetRandomFromListNotRepeating();
			if (!audioClip) return;

			this._lastPlayedClips.Enqueue(audioClip);
			
			this._audioSource.Stop();
			this._audioSource.clip = audioClip;
			this._audioSource.Play();
		}

		public AudioClip GetRandomFromListNotRepeating() {
			if (this._audioClips == null) return null;
			int count = this._audioClips.Count;
			if (count <= 0) return null;
			if (count == 1) return this._audioClips[0];
			
			if (this._lastPlayedClips.Count >= count * 0.5f) {
				this._lastPlayedClips.Dequeue();
			}
			
			return this._audioClips.Except(this._lastPlayedClips).RandomElement();
		}
	}
}

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if FMOD
using FMODUnity;
#endif

namespace CDK.Audio {
	#if FMOD
	[RequireComponent(typeof(StudioEventEmitter))]
	#endif
	public class CRandomAudioPlayer : MonoBehaviour {

		#if FMOD
		[SerializeField] private StudioEventEmitter _audioEmitter;
		#endif
		[NonSerialized] private string[] _audios;
		[NonSerialized] private readonly Queue<string> _lastPlayedAudios = new Queue<string>();
		
		


		public void SetAudioEvents(string[] clips) {
			if (Equals(clips, this._audios)) return;
			this._lastPlayedAudios.Clear();
			this._audios = clips;
		}

		public void PlayAudio() {
			var audioEvent = this.GetRandomFromListNotRepeating();
			if (audioEvent.CIsNullOrEmpty()) return;

			this._lastPlayedAudios.Enqueue(audioEvent);

			#if FMOD
			this._audioEmitter.Event = audioEvent;
			this._audioEmitter.Play();
			#endif

		}

		private string GetRandomFromListNotRepeating() {
			if (this._audios == null) return null;
			int count = this._audios.Length;
			if (count == 1) return this._audios[0];
			
			if (this._lastPlayedAudios.Count >= count * 0.5f) {
				this._lastPlayedAudios.Dequeue();
			}
			
			return this._audios.Except(this._lastPlayedAudios).CRandomElement();
		}
	}
}

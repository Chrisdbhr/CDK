using System.Collections.Generic;
using UnityEngine;

namespace CDK {
	[CreateAssetMenu(fileName = "AudioClipArray", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "AudioClipArray_", order = 201)]
	public class CAudioClipListScriptableObject : ScriptableObject {
	
		public List<AudioClip> AudioClipList {
			get { return this._audioClips; }
		}
		[SerializeField] private List<AudioClip> _audioClips = new List<AudioClip>();

		
		
		
		public AudioClip GetRandomAudioClip() {
			return this._audioClips.RandomElement();
		}
	}
}

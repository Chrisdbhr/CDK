using System.Linq;
using UnityEngine;

#if FMOD
using FMODUnity;
#endif

namespace CDK {
	[CreateAssetMenu(fileName = "AudioClipArray", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "AudioClipArray", order = 201)]
	public class CAudioClipListScriptableObject : ScriptableObject {

		[System.Serializable]
		class AudiosList {
			#if FMOD
			public EventReference audio;
			#else
			public string audio;
			#endif
		}
		
		
		#if FMOD
		public EventReference[] Audios {
			get {
				return (from audioList in this._audios where audioList != null && !audioList.audio.IsNull select audioList.audio).ToArray();
			}
		}
		#endif
		[SerializeField] private AudiosList[] _audios = new AudiosList[1];
	}
}

using System.Linq;
using FMODUnity;
using UnityEngine;

namespace CDK {
	[CreateAssetMenu(fileName = "AudioClipArray", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "AudioClipArray", order = 201)]
	public class CAudioClipListScriptableObject : ScriptableObject {

		[System.Serializable]
		class AudiosList {
			[EventRef]
			public string audio;
		}
		
		
		public string[] Audios {
			get {
				return (from audioList in this._audios where audioList != null && !audioList.audio.CIsNullOrEmpty() select audioList.audio).ToArray();
			}
		}
		[SerializeField] private AudiosList[] _audios = new AudiosList[1];
	}
}

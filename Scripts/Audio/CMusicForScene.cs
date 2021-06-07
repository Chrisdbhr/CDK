using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CDK.Audio {
	public class CMusicForScene : MonoBehaviour {
	
		[SerializeField] [EventRef] private string _music;
		[NonSerialized] private Scene _sceneToCheck;
		[NonSerialized] private FMOD.Studio.EventInstance _musicState;
		

		
		
		private void OnEnable() {
			if (this._music.CIsNullOrEmpty()) {
				Debug.LogWarning($"{this.name} is not referencing a valid Music (EventRef)!");
				return;
			}
			this._musicState = FMODUnity.RuntimeManager.CreateInstance(this._music);
			if(this._musicState.isValid()) this._musicState.start();
		}
		
		private void OnDisable() {
			this._musicState.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		}
		
	}
}

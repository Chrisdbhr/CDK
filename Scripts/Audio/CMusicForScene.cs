using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CDK.Audio {
	public class CMusicForScene : MonoBehaviour {
		[SerializeField] [EventRef] private string _music;
		[NonSerialized] private Scene _sceneToCheck;
		[NonSerialized] private FMOD.Studio.EventInstance _musicState;
		
		
		private void Awake() {
			this._sceneToCheck = this.gameObject.scene;
			SceneManager.sceneUnloaded += unloadedScene => {
				if (unloadedScene != this._sceneToCheck) return;
				if (!this._musicState.isValid()) return;
				this._musicState.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				this.gameObject.CDestroy(15f);
			};
			DontDestroyOnLoad(this);
		}

		private void Start() {
			this._musicState = FMODUnity.RuntimeManager.CreateInstance(this._music);
			if(this._musicState.isValid()) this._musicState.start();
		}
	}
}

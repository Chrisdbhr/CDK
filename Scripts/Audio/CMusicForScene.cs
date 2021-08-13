using System;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace CDK.Audio {
	public class CMusicForScene : MonoBehaviour {
	
		[SerializeField] private bool _is3d;
		[SerializeField] [EventRef] private string _music;
		
		[NonSerialized] private FMOD.Studio.EventInstance _musicState;





		#region <<---------- MonoBehaviour ---------->>

		private void OnEnable() {
			if (this._music.CIsNullOrEmpty()) {
				Debug.LogWarning($"{this.name} is not referencing a valid Music (EventRef)!");
				return;
			}
			this._musicState = RuntimeManager.CreateInstance(this._music);

			if (this._is3d) {
				this._musicState.set3DAttributes(this.transform.position.To3DAttributes());
			}
			
			if(this._musicState.isValid()) this._musicState.start();
		}
		
		private void OnDisable() {
			this.Stop();

		}
		
		#if UNITY_EDITOR
		private void Reset() {
			this.name = "BGM";
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>



		#region <<---------- Music ---------->>

		public void Stop() {
			this._musicState.stop(STOP_MODE.ALLOWFADEOUT);
		}
		
		#endregion <<---------- Music ---------->>
	}
}

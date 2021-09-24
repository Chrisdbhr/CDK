using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if FMOD
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
#endif

namespace CDK.Audio {
	public class CMusicForScene : MonoBehaviour {
	
		[SerializeField] private bool _is3d;
		
#if FMOD
		[EventRef] 
#endif
		[SerializeField] private string _music;
		
#if FMOD
		[NonSerialized] private FMOD.Studio.EventInstance _musicState;
#endif





		#region <<---------- MonoBehaviour ---------->>

		private void OnEnable() {
			if (this._music.CIsNullOrEmpty()) {
				Debug.LogWarning($"{this.name} is not referencing a valid Music (EventRef)!");
				return;
			}
				
			#if FMOD
			this._musicState = RuntimeManager.CreateInstance(this._music);

			if (this._is3d) {
				this._musicState.set3DAttributes(this.transform.position.To3DAttributes());
			}
			
			if(this._musicState.isValid()) this._musicState.start();
			#endif

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
			#if FMOD
			this._musicState.stop(STOP_MODE.ALLOWFADEOUT);
			#else
			throw new NotImplementedException();
			#endif
		}
		
		#endregion <<---------- Music ---------->>
	}
}

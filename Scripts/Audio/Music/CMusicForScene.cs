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
		[SerializeField] private EventReference _music;
		[NonSerialized] private FMOD.Studio.EventInstance _musicState;
		#endif


        

		#region <<---------- MonoBehaviour ---------->>

        private void OnEnable() {
			#if FMOD
			if (this._music.IsNull) {
				Debug.LogWarning($"{this.name} is not referencing a valid Music (EventRef)!");
				return;
			}
            this.Play();
			#endif
        }
		
		private void OnDisable() {
			this.Stop(true);
		}
		
		#if UNITY_EDITOR
		private void Reset() {
			this.name = "BGM";
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>


        

		#region <<---------- Music ---------->>

        public void Play() {
            #if FMOD
            this._musicState = RuntimeManager.CreateInstance(this._music);

            if (this._is3d) {
                this._musicState.set3DAttributes(this.transform.position.To3DAttributes());
            }
			
            if(this._musicState.isValid()) this._musicState.start();
            #endif
        }
        
        public void Stop(bool allowFadeout) {
			#if FMOD
			this._musicState.stop(allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
			#else
			throw new NotImplementedException();
			#endif
		}
		
		#endregion <<---------- Music ---------->>
	}
}
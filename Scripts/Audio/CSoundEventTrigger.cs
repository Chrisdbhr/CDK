
using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if FMOD
using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
#endif

namespace CDK {
	public class CSoundEventTrigger : CAutoTriggerCompBase {
		
		#region <<---------- Enum ---------->>
		
		enum MonoStopTime {
			OnDisable,
			OnDestroy
		}
		
		#endregion <<---------- Enum ---------->>
		
		
		
		
		#region <<---------- Properties ---------->>

		[SerializeField] private MonoStopTime _eventStopTime;
		[SerializeField] private bool _is3d;
		
		#if FMOD
		[EventRef]
		#endif
		[SerializeField] private string _soundEvent;

		#if FMOD
		private EventInstance _soundState;
		#endif

		#endregion <<---------- Properties ---------->>

		
		

		#region <<---------- MonoBehaviour ---------->>
	
		private void OnDestroy() {
			if (this._eventStopTime != MonoStopTime.OnDestroy) return;
			this.StopEvent();
		}

		private void OnDisable() {
			if (this._eventStopTime != MonoStopTime.OnDisable) return;
			this.StopEvent();
		}

		#if UNITY_EDITOR
		private void Reset() {
			this.name = "Sound Event Trigger";
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>


		

		#region <<---------- CAutoTriggerCompBase ---------->>
		
		protected override void TriggerEvent() {
			#if FMOD
			if (this._soundEvent.CIsNullOrEmpty()) {
				Debug.LogWarning($"{this.name} is not referencing a valid Music (EventRef)!");
				return;
			}

			if (this._soundState.isValid()) {
				this._soundState.start();
			}
			else {
				this.CreateSoundInstance();
			}
			#else
			throw new NotImplementedException();
			#endif
		}
		
		#endregion <<---------- CAutoTriggerCompBase ---------->>

		
		
		
		#region <<---------- General ---------->>

		private void CreateSoundInstance() {
			#if FMOD
			this._soundState = RuntimeManager.CreateInstance(this._soundEvent);
			if (this._is3d) {
				this._soundState.set3DAttributes(this.transform.position.To3DAttributes());
			}
			#else
			throw new NotImplementedException();
			#endif
		}
		
		private void StopEvent() {
			#if FMOD
			this._soundState.stop(STOP_MODE.ALLOWFADEOUT);
			#else
			throw new NotImplementedException();
			#endif
		}
		
		#endregion <<---------- General ---------->>
		
	}
}

using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;


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
		[SerializeField] [EventRef] private string _soundEvent;
		private EventInstance _soundState;

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
		}
		
		#endregion <<---------- CAutoTriggerCompBase ---------->>

		
		
		
		#region <<---------- General ---------->>

		private void CreateSoundInstance() {
			this._soundState = RuntimeManager.CreateInstance(this._soundEvent);
			if (this._is3d) {
				this._soundState.set3DAttributes(this.transform.position.To3DAttributes());
			}
		}
		
		private void StopEvent() {
			this._soundState.stop(STOP_MODE.ALLOWFADEOUT);
		}
		
		#endregion <<---------- General ---------->>
		
	}
}

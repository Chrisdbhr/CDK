using System;
using UnityEngine;

#if FMOD
using FMODUnity;
#endif

namespace CDK.Audio {
	public class CSoundAreaGlobalParamTrigger : MonoBehaviour {

		#if FMOD
		[ParamRef]
		#endif
		[SerializeField] private string _parameter;
		

		public void TriggerParameter() {
			#if FMOD
			FMODUnity.RuntimeManager.StudioSystem.setParameterByName(_parameter, 1f);
			#else
			throw new NotImplementedException();
			#endif
		}
		
		public void UntriggerParameter() {
			#if FMOD
			FMODUnity.RuntimeManager.StudioSystem.setParameterByName(_parameter, 0f);
			#else
			throw new NotImplementedException();
			#endif
		}
		
	}
}

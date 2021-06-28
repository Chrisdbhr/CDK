using FMODUnity;
using UnityEngine;

namespace CDK.Audio {
	public class CSoundAreaGlobalParamTrigger : MonoBehaviour {

		[SerializeField] [ParamRef] private string _parameter;
		

		public void TriggerParameter() {
			FMODUnity.RuntimeManager.StudioSystem.setParameterByName(_parameter, 1f);
		}
		
		public void UntriggerParameter() {
			FMODUnity.RuntimeManager.StudioSystem.setParameterByName(_parameter, 0f);
		}
		
	}
}

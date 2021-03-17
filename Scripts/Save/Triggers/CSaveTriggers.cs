using UnityEngine;

namespace CDK {
	public class CSaveTriggers : MonoBehaviour {
		
		public void SaveCameraSensitivityValue(float value) {
			CSave.get.CameraSensitivity = value;
		}
	}
}

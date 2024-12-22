using UnityEngine;

namespace CDK {
	public class COnEditorTrigger : CAutoTriggerCompBase {

		[SerializeField] CUnityEventBool _isOnEditorEvent;
		[SerializeField] CUnityEventBool _isNotOnEditorEvent;
		
		
		
		
		protected override void TriggerEvent() {
			var isEditor = Application.isEditor;
			_isOnEditorEvent?.Invoke(isEditor);
			_isNotOnEditorEvent?.Invoke(!isEditor);
		}
	}
}

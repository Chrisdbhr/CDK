using UnityEngine;

namespace CDK {
	public class COnEditorTrigger : CAutoTriggerCompBase {

		[SerializeField] private CUnityEventBool _isOnEditorEvent;
		[SerializeField] private CUnityEventBool _isNotOnEditorEvent;
		
		
		
		
		protected override void TriggerEvent() {
			var isEditor = Application.isEditor;
			this._isOnEditorEvent?.Invoke(isEditor);
			this._isNotOnEditorEvent?.Invoke(!isEditor);
		}
	}
}

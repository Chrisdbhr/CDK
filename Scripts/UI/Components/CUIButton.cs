using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK.UI {
	[RequireComponent(typeof(Button))]
	public class CUIButton : CUIInteractable {
		public Button Button;


        #if UNITY_EDITOR
		private void Reset() {
            if (Button == null) {
                Button = this.GetComponent<Button>();
                EditorUtility.SetDirty(Button);
            }
		}
        #endif
	}
	
}
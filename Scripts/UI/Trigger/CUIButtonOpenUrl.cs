using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK.UI.Trigger {
    [RequireComponent(typeof(Button))]
    public class CUIButtonOpenUrl : MonoBehaviour {

        #if UNITY_EDITOR
        [MenuItem("CONTEXT/Button/Add open url component")]
        static void EditorAddOpenUrl(MenuCommand command) {
            var target = command.context as Button;
            Undo.AddComponent(target.gameObject, typeof(CUIButtonOpenUrl));
        }
        #endif

        private Button button;
        [SerializeField] private string urlToOpen = "https://chrisjogos.com";


        private void Awake() {
            button = this.GetComponent<Button>();
            button.onClick.AddListener(() => {
                CApplication.OpenURL(urlToOpen);
            });
        }



    }
}
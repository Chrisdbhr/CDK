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

        [SerializeField] string urlToOpen = "https://chrisjogos.com";
        Button button;


        void Awake()
        {
            if (!TryGetComponent(out button)) {
                Debug.LogError($"Error getting Button component on '{name}'");
            }
            button.onClick.AddListener(() => {
                CApplication.OpenURL(urlToOpen);
            });
        }

    }
}
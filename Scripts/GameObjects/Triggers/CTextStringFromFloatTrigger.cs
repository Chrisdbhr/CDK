using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK
{
    [RequireComponent(typeof(TMP_Text))]
    public class CTextStringFromFloatTrigger : MonoBehaviour
    {
        [SerializeField] string _stringFormatFormula = "00";
        [SerializeField] TMP_Text _tmpText;
        [SerializeField] CUnityEventString _textEvent;


        void Awake()
        {
            OnValidate();
        }

        void OnValidate()
        {
            if(!_tmpText) TryGetComponent(out _tmpText);
        }

        public void Set(float value)
        {
            var text = value.ToString(_stringFormatFormula);
            if(_tmpText) _tmpText.text = text;
            _textEvent?.Invoke(text);
        }

        #if UNITY_EDITOR
        [MenuItem("CONTEXT/TextMeshProUGUI/Add float to string trigger")]
        static void ConvertTextUGUI(MenuCommand data) {
            ConvertFrom(data);
        }
        [MenuItem("CONTEXT/TextMeshPro/Add float to string trigger")]
        static void ConvertText(MenuCommand data) {
           ConvertFrom(data);
        }
        static void ConvertFrom(MenuCommand data)
        {
            if (data?.context is not (TMP_Text tmpText)) return;

            var go = tmpText.gameObject;
            if (go == null) return;

            var meshRenderer = Undo.AddComponent<CTextStringFromFloatTrigger>(go);
        }
        #endif
    }
}
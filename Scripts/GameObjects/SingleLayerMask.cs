using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK
{
     
    [System.Serializable]
    public class SingleLayerMask
    {
        [SerializeField]
        private int m_LayerIndex = 0;
        public int LayerIndex
        {
            get { return this.m_LayerIndex; }
            set
            {
                if (value > 0 && value < 32)
                {
                    this.m_LayerIndex = value;
                }
            }
        }
 
        public static implicit operator int(SingleLayerMask layerMask)
        {
            return 1 << layerMask.m_LayerIndex;
        }
        
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SingleLayerMask))]
    public class SingleUnityLayerPropertyDrawer : PropertyDrawer 
    {
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            EditorGUI.BeginProperty(_position, GUIContent.none, _property);
            SerializedProperty layerIndex = _property.FindPropertyRelative("m_LayerIndex");
            _position = EditorGUI.PrefixLabel(_position, GUIUtility.GetControlID(FocusType.Passive), _label);
            if (layerIndex != null)
            {
                layerIndex.intValue = EditorGUI.LayerField(_position, layerIndex.intValue);
            }
            EditorGUI.EndProperty( );
        }
    }
#endif
    
}
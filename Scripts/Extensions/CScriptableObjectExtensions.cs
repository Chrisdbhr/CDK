using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK
{
    public static class CScriptableObjectExtensions
    {
        #if UNITY_EDITOR
        public static T EditorCreateInResourcesFolder<T>() where T : ScriptableObject
        {
            var so = ScriptableObject.CreateInstance<UISoundsBankSO>();
            var path = "Assets/Resources";
            if(!AssetDatabase.AssetPathExists(path)) AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.CreateAsset(so, $"{path}/{nameof(UISoundsBankSO)}.asset");
			return so as T;
        }
        #endif
    }
}
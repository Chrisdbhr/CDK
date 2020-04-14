using UnityEditor;
using UnityEngine;

namespace CDK
{
    public class PrintSelectedObjectLayer
    {
        [MenuItem("CONTEXT/Component/Print this object layer int value")]
        private static void RenameGameObjectWithThisComponentName(MenuCommand data)
        {
            Object context = data.context;
            if (context == null) return;
            
            var comp = context as Component;
            if (comp == null) return;
            
            GameObject go = comp.gameObject;
            if (go == null) return;

            Debug.Log($"{go.name} layer int value is {go.layer} and toString is {go.layer.ToString()}");
        }
    }
}
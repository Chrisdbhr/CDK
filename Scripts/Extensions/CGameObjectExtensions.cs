using UnityEngine;

namespace CDK {
    public static class CGameObjectExtensions {
        
        public static T CGetOrAddComponent<T>(this GameObject go) where T : Component {
            var comp = go.GetComponent<T>();
            return comp != null ? comp : go.AddComponent<T>();
        }
        
    }
}
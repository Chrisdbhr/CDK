using UnityEngine;

namespace CDK {
    public static class CObjectExtensions {

        public static string CGetNameSafe(this Object o, string fallback = "null") {
            if (o != null) return o.name;
            Debug.Log("Tried to get name of null object, returning 'null' as object name");
            return fallback;
        }
        
    }
}
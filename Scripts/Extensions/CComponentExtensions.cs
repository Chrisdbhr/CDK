using UnityEngine;

namespace CDK {
    public static class CComponentExtensions {

        public static T CGetComponentInChildrenOrInParent<T>(this Component comp, bool includeInactive = false) {
            return comp.gameObject.CGetComponentInChildrenOrInParent<T>(includeInactive);
        }
        
        public static T CGetComponentInParentOrInChildren<T>(this Component comp, bool includeInactive = false) {
            return comp.gameObject.CGetComponentInParentOrInChildren<T>(includeInactive);

        }
        
    }
}
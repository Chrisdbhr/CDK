using UnityEngine;

namespace CDK {
    public static class CColliderExtensions {

        public static void CCloneColliderAsChild(this Collider col, LayerMask newLayer) {
            if (col == null) return;
            var go = new GameObject("Char Collider");
            go.layer = newLayer;
            go.transform.parent = col.transform;

            switch (col.GetComponent<Collider>()) {
                case BoxCollider original:
                    var b = go.AddComponent<BoxCollider>();
                    b.center = original.center;
                    b.size = original.size;
                    break;
                case SphereCollider original:
                    var s = go.AddComponent<SphereCollider>();
                    s.center = original.center;
                    s.radius = original.radius;
                    break;
                case CapsuleCollider original:
                    var c = go.AddComponent<CapsuleCollider>();
                    c.center = original.center;
                    c.radius = original.radius;
                    c.height = original.height;
                    break;
            }
            
            go.transform.CResetTransform();

        }
        
    }
}
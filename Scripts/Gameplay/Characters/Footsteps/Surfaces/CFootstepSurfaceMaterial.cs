using UnityEngine;

namespace CDK {
    [RequireComponent(typeof(Collider))]
    public class CFootstepSurfaceMaterial : MonoBehaviour, CIFootstepSurfaceBase {
       
        [Header("List order matters, both list must have same count")]
        public Material[] MaterialList;
        public CFootstepInfo[] FootstepInfoData;

        
        
        
        public CFootstepInfo GetFootstepInfoFromRaycastHit(RaycastHit hit) {
            var selected = GetMaterialAtRaycastHitPoint(hit);
            if (selected == null) return null;
            for (int i = 0; i < this.MaterialList.Length; i++) {
                if (this.MaterialList[i] == selected) return this.FootstepInfoData[i];
            }
            return null;
        }
        
        
        
        
        private Material GetMaterialAtRaycastHitPoint(RaycastHit hit) {
            // check for mesh
            var meshFilter = hit.collider.GetComponent<MeshFilter>();
            if (meshFilter == null) return null;
            var sharedMesh = meshFilter.sharedMesh;
            if (sharedMesh == null) return null;
            var rend = meshFilter.GetComponent<Renderer>();
            if (!rend) return null;

            var materialId = -1;

            if (!sharedMesh.isReadable || !(hit.collider is MeshCollider)) {
                materialId = 0;
            }
            else {
                var triangleIndex = hit.triangleIndex;
                if (triangleIndex <= -1) return null;
                var triangles = sharedMesh.triangles;
                int lookupIndex1 = triangles[triangleIndex * 3];
                int lookupIndex2 = triangles[triangleIndex * 3 + 1];
                int lookupIndex3 = triangles[triangleIndex * 3 + 2];
                var subMeshCount = sharedMesh.subMeshCount;

                // get material index
                for (int i = 0; i < subMeshCount; i++) {
                    var tr = sharedMesh.GetTriangles(i);
                    for (int j = 0; j < tr.Length; j++) {
                        if (tr[j] != lookupIndex1 || 
                            tr[j + 1] != lookupIndex2 || 
                            tr[j + 2] != lookupIndex3) continue;
                        materialId = i;
                        break;
                    }
                    if (materialId != -1) break;
                }

                if (materialId == -1) return null;
            }

            return rend.sharedMaterials[materialId];
        }
        
    }
}
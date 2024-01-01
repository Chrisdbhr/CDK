using UnityEditor;
using UnityEngine;

namespace CDK.Editor {
    public static class CSkinnedMeshToMesh {
        [MenuItem("CONTEXT/SkinnedMeshRenderer/Convert to Static Mesh Renderer")]
        private static void ConvertToStaticMeshRenderer(MenuCommand data) {
            if (!(data?.context is Component comp) || !(comp is SkinnedMeshRenderer skinnedMesh)) return;

            var go = skinnedMesh.gameObject;
            if (go == null) return;

            var meshRenderer = Undo.AddComponent<MeshRenderer>(go);
            meshRenderer.sharedMaterials = skinnedMesh.sharedMaterials;
            var meshFilter = Undo.AddComponent<MeshFilter>(go);
            meshFilter.sharedMesh = new Mesh();
            skinnedMesh.BakeMesh(meshFilter.sharedMesh, true);
            Undo.DestroyObjectImmediate(skinnedMesh);

            Debug.Log($"Converted {go.name} to static mesh renderer");
        }
    }
}
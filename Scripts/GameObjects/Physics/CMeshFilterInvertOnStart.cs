using System;
using System.Linq;
using UnityEngine;

namespace CDK {
    public class CMeshFilterInvertOnStart : MonoBehaviour {

        [SerializeField] private MeshFilter[] _meshesToInvert;
        
        
        
        
        private void Reset() {
            this._meshesToInvert = this.GetComponentsInChildren<MeshFilter>();
        }

        void Start() {
            foreach (var meshFilter in this._meshesToInvert) {
                for (int i = 0; i < meshFilter.mesh.subMeshCount; i++) {
                    meshFilter.mesh.SetIndices(meshFilter.mesh.GetIndices(i)
                    .Concat(meshFilter.mesh.GetIndices(0).Reverse()).ToArray(), meshFilter.mesh.GetTopology(i), i);
                }
                if(meshFilter.TryGetComponent<MeshCollider>(out var mc)) {
                    mc.sharedMesh = meshFilter.mesh;
                }
            }
        }
    }
}
using UnityEngine;

namespace CDK {
    [RequireComponent(typeof(Collider), typeof(CTerrainTextureDetector))]
    public class CFootstepSurfaceTerrain : MonoBehaviour, CIFootstepSurfaceBase {
        
        [Header("List order matters, both list must have same count")]
        public TerrainLayer[] TerrainLayers;
        public CFootstepInfo[] FootstepInfoData;

        private CTerrainTextureDetector _terrainTextureDetector;

        
        
        
        private void Awake() {
            this._terrainTextureDetector = this.gameObject.CGetOrAddComponent<CTerrainTextureDetector>();
        }
        
        
        
        
        public CFootstepInfo GetFootstepInfoFromRaycastHit(RaycastHit hit) {
            var selected = this._terrainTextureDetector.GetTerrainLayerAtPosition(hit.point);
            if (selected == null) return null;
            for (int i = 0; i < this.TerrainLayers.Length; i++) {
                if (this.TerrainLayers[i] == selected) return this.FootstepInfoData[i];
            }
            return null;
        }
        
    }
}
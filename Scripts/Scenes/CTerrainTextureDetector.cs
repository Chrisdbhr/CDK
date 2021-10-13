using CDK;
using UnityEngine;

namespace CDK {
    /// <summary>
    /// Utility for getting the dominant texture from a splat blend at a given point on a terrain.
    /// From https://github.com/JimmyCushnie/JimmysUnityUtilities
    /// </summary>
    [RequireComponent(typeof(Terrain))]
    public class CTerrainTextureDetector : MonoBehaviour {
        Terrain ThisTerrain;
        TerrainData ThisTerrainData => ThisTerrain.terrainData;

        float[,,] CachedTerrainAlphamapData;

        
        void Start() {
            ThisTerrain = GetComponent<Terrain>();
            CachedTerrainAlphamapData = ThisTerrainData.GetAlphamaps(0, 0, ThisTerrainData.alphamapWidth, ThisTerrainData.alphamapHeight);
        }

        
        public TerrainLayer GetFirstTextureAt(Vector3 worldPosition) {
            int index = this.GetDominantTextureIndexAt(worldPosition);
            if (index < 0) return null;
            return this.ThisTerrainData.terrainLayers.CGetAtIndexSafe(index);
        }

        /// <summary>
        /// Gets the index of the most visible texture on the terrain at the specified point in world space.
        /// These texture indexes are assigned in the "paint textures" tab of the terrain inspector.
        /// If the supplied position is outside the bounds of the terrain, this function will return -1.
        /// </summary>
        private int GetDominantTextureIndexAt(Vector3 worldPosition) {
            Vector3Int alphamapCoordinates = ConvertToAlphamapCoordinates(worldPosition);

            if (!this.CachedTerrainAlphamapData.ContainsIndex(alphamapCoordinates.x, 1)) return -1;

            if (!this.CachedTerrainAlphamapData.ContainsIndex(alphamapCoordinates.z, 0)) return -1;


            int mostDominantTextureIndex = 0;
            float greatestTextureWeight = float.MinValue;

            int textureCount = CachedTerrainAlphamapData.GetLength(2);
            for (int textureIndex = 0; textureIndex < textureCount; textureIndex++)
            {
                // I am really not sure why the x and z coordinates are out of order here, I think it's just Unity being lame and weird
                float textureWeight = CachedTerrainAlphamapData[alphamapCoordinates.z, alphamapCoordinates.x, textureIndex];

                if (textureWeight > greatestTextureWeight)
                {
                    greatestTextureWeight = textureWeight;
                    mostDominantTextureIndex = textureIndex;
                }
            }

            return mostDominantTextureIndex;


            Vector3Int ConvertToAlphamapCoordinates(Vector3 _worldPosition)
            {
                Vector3 relativePosition = _worldPosition - transform.position;
                // Important note: terrains cannot be rotated, so we don't have to worry about rotation

                return new Vector3Int
                (
                    x: Mathf.RoundToInt((relativePosition.x / ThisTerrainData.size.x) * ThisTerrainData.alphamapWidth),
                    y: 0,
                    z: Mathf.RoundToInt((relativePosition.z / ThisTerrainData.size.z) * ThisTerrainData.alphamapHeight)
                );
            }
        }
    }
}

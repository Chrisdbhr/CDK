using System;
using UnityEngine;

namespace CDK {
    [Obsolete]
	public class CFootstepDatabase : ScriptableObject {
		
		public CFootstepInfo[] FootstepInfos;
		

        
        
		public CFootstepInfo GetFootstepInfoByMaterial(Material targetMaterial) {
			foreach (var footstepInfo in this.FootstepInfos) {
				if (footstepInfo == null) continue;
				foreach (var mat in footstepInfo.Materials) {
					if (mat == null) continue;
                    if (mat.renderQueue != targetMaterial.renderQueue) continue;
                    if (mat.shader != targetMaterial.shader) continue;
					if (mat.name != targetMaterial.name.Replace(" (Instance)", string.Empty)) continue;
					return footstepInfo;
				}
			}
			return null;
		}

		public CFootstepInfo GetFootstepInfoByTerrainLayer(TerrainLayer targetTerrainLayer) {
            foreach (var footstepInfo in this.FootstepInfos) {
                if (footstepInfo == null) continue;
                foreach (var terLayer in footstepInfo.TerrainLayers) {
                    if (terLayer == null) continue;
                    if (terLayer == targetTerrainLayer) return footstepInfo;
                }
            }
            return null;
        }
		
	}

}

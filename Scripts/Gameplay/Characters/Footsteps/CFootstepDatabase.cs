using System.Linq;
using UnityEngine;

namespace CDK {
	[CreateAssetMenu(fileName = "FootstepDatabase", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "Footstep database", order = 101)]
	public class CFootstepDatabase : ScriptableObject {
		
		public CFootstepInfo[] FootstepInfos;
		

		public CFootstepInfo GetFootstepInfoByMaterial(Material targetMaterial) {
			foreach (var footstepInfo in this.FootstepInfos) {
				if (footstepInfo == null) continue;
				foreach (var mat in footstepInfo.Materials) {
					if (mat.renderQueue != targetMaterial.renderQueue) continue;
					if (mat.name != targetMaterial.name) continue;
					return footstepInfo;
				}
			}
			return null;
		}

		public CFootstepInfo GetFootstepInfoByTerrainLayer(TerrainLayer targetTerrainLayer) {
			return (from footstepInfo in this.FootstepInfos where footstepInfo != null from terLayer in footstepInfo.TerrainLayers where terLayer == targetTerrainLayer select footstepInfo).FirstOrDefault();
		}
		
	}

}

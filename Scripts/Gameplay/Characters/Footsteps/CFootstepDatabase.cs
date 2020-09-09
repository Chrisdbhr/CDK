﻿using System.Linq;
using UnityEngine;

namespace CDK {
	[CreateAssetMenu(fileName = "FootstepDatabase", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "Footstep database", order = 101)]
	public class CFootstepDatabase : ScriptableObject {
		
		public CFootstepInfo[] FootstepInfos;

		
		
		
		public CFootstepInfo GetFootstepInfo(Material targetMaterial) {
			return (from footstepInfo in this.FootstepInfos where footstepInfo != null from mat in footstepInfo.Materials where mat == targetMaterial select footstepInfo).FirstOrDefault();
		}
		
	}

}
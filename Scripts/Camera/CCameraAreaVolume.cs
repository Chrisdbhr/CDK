using System;
using UnityEngine;

namespace CDK {
	
	[RequireComponent(typeof(Collider))][Obsolete]
	public class CCameraAreaVolume : MonoBehaviour {
		
		public CCameraAreaProfileData CameraAreaProfileData {
			get { return this.cameraAreaProfileData; }
		}
		[SerializeField] private CCameraAreaProfileData cameraAreaProfileData;
		
	}
}
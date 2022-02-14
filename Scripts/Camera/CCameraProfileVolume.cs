using UnityEditor;
using UnityEngine;

namespace CDK {
	public class CCameraProfileVolume : MonoBehaviour {

		public bool IsGlobal => this._isGlobal;
		[SerializeField] private bool _isGlobal;
		
		public CPlayerCamera.CameraType CameraType => _cameraType;
		[SerializeField] private CPlayerCamera.CameraType _cameraType;

		
		

		#if UNITY_EDITOR
		void Reset() {
			Undo.RecordObject(this.gameObject, "Renamed object");
			this.name = "Camera Profile";
		}
		#endif
	}
}
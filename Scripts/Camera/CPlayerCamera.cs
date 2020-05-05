using System;
using UnityEngine;

namespace CDK {
	
	[RequireComponent(typeof(Camera))]
	public class CPlayerCamera : MonoBehaviour {

		[NonSerialized] private Camera _camera;
		
		
		
		
		private void Awake() {
			this._camera = this.GetComponent<Camera>();
		}


		public void SetCameraEnabled(bool enabled) {
			this._camera.enabled = enabled;
		}


		public void SetRotation(Quaternion newRotation) {
			this._camera.transform.rotation = newRotation;
		}
		public void SetPositionAndRotation(Transform transformToCopy) {
			this.SetPositionAndRotation(transformToCopy.position, transformToCopy.rotation);
		}
		public void SetPositionAndRotation(Vector3 position, Quaternion rotation) {
			this._camera.transform.position = position;
			this._camera.transform.rotation = rotation;
		}
	}
}

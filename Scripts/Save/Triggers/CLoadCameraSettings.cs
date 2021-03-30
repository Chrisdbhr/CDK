using System;
using Cinemachine;
using UnityEngine;

namespace CDK {
	public class CLoadCameraSettings : MonoBehaviour {

		[SerializeField] private CinemachineVirtualCamera _cinemachineCamera;
		[NonSerialized] private CinemachinePOV _pov;

		private void Awake() {
			if(this._cinemachineCamera != null) this._pov = this._cinemachineCamera.GetCinemachineComponent<CinemachinePOV>();
		}

		private void OnEnable() {
			CSave.get.CameraSensitivity_Changed += this.OnCameraSensitivity;
			this.OnCameraSensitivity(CSave.get.CameraSensitivity);
		}

		private void OnDisable() {
			CSave.get.CameraSensitivity_Changed -= this.OnCameraSensitivity;
		}

		
		
		
		private void OnCameraSensitivity(float value) {
			if (this._pov != null) {
				this._pov.m_HorizontalAxis.m_MaxSpeed = value;
				this._pov.m_VerticalAxis.m_MaxSpeed = value;
			}
		}
		
	}
}

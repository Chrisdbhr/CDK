using System;
using System.Collections;
using UnityEngine;

namespace CDK {
	public class CAim : MonoBehaviour {

		private const float MAX_RAYCAST_DISTANCE = 10000f;
		
		[NonSerialized] private float _defaultFov = 50f;
		[NonSerialized] private float _minAimDistance = 1f;

		public bool IsAiming {
			get { return this._isAiming; }
			set {
				if (this._isAiming == value) return;
				this._isAiming = value;
				if(this._fovSetCoroutine != null) this.StopCoroutine(this._fovSetCoroutine);
				if (this._isAiming) {
					this._fovSetCoroutine = this.StartCoroutine(this.SmoothSetFov(this._defaultFov * 0.5f, 0.5f));
				}
				else {
					this._fovSetCoroutine = this.StartCoroutine(this.SmoothSetFov(this._defaultFov, 1f));
				}
			}
		}
		[NonSerialized] private bool _isAiming;
		
		[NonSerialized] private Coroutine _fovSetCoroutine;
		
		// Cache
		[SerializeField] private CCharacterBase ownerCiCharacter;
		[SerializeField] private Camera _camera;
		[NonSerialized] private RaycastHit _hitInfo; 
		[NonSerialized] private Transform _myTransform;
		[NonSerialized] private Vector3 _position;
		[NonSerialized] private Vector3 _forward;

		
		private void Awake() {
			if (this.ownerCiCharacter == null || this._camera == null) {
				Debug.LogError($"CameraAim script being destroyed because there is no Character set as owner to set target or there is no camera attached to it.");
				Destroy(this);
			}

			this._myTransform = this.transform;
			this._defaultFov = this._camera.fieldOfView;
		}

		private void Update() {
			// cache
			this._position = this._myTransform.position;
			this._forward = this._myTransform.forward;
			
			bool hit = Physics.Raycast(
				this._position,
				this._forward,
				out this._hitInfo,
				MAX_RAYCAST_DISTANCE,
				CGameSettings.get.AttackableLayers,
				QueryTriggerInteraction.Collide
			);

			this.ownerCiCharacter.SetAimDirection(this._forward);
		}

		private IEnumerator SmoothSetFov(float targetFov, float time) {
			float elapsedTime = 0f;
			while (elapsedTime < time) {
				float fov = this._camera.fieldOfView;
				this._camera.fieldOfView = fov.CLerp(targetFov, (elapsedTime / time) * Time.timeScale);
				elapsedTime += Time.deltaTime * Time.timeScale;
				yield return null;
			}
			this._camera.fieldOfView = targetFov;
		}
		
	}
}

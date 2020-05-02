using System;
using System.Collections;
using CDK.Inventory;
using UnityEngine;

namespace CDK {
	public class CAim : MonoBehaviour {

		private const float MAX_RAYCAST_DISTANCE = 10000f;
	
		[SerializeField] private Transform _aimTargetPosition;
		[SerializeField] private CUnityEventFloat _aimProgressPercentage;
		[SerializeField] private CInventory _ownerCharInventory;
		

		[NonSerialized] private float _defaultFov = 50f;
		[NonSerialized] private float _minAimDistance = 1f;

		public bool IsAiming {
			get { return this._isAiming; }
			set {
				if (this._isAiming == value) return;
				this._isAiming = value;
				
				// set fov
				if(this._fovSetCoroutine != null) this.StopCoroutine(this._fovSetCoroutine);
				this._fovSetCoroutine = this.StartCoroutine(this._isAiming ? this.SmoothSetFov(this._defaultFov * 0.5f, 0.5f) : this.SmoothSetFov(this._defaultFov, 1f));
				
				// set aim progress
				if(this._aimProgressCoroutine != null) this.StopCoroutine(this._aimProgressCoroutine);
				this._aimProgressCoroutine = this.StartCoroutine(
					this._isAiming && (this._ownerCharInventory != null && this._ownerCharInventory.EquippedWeapon is CGunData)? 
							this.SmoothSetAimProgress(1f) : 
							this.SmoothSetAimProgress(0f));
			}
		}
		[NonSerialized] private bool _isAiming;

		private float AimProgress {
			get { return this._aimProgress; }
			set {
				this._aimProgress = value;
				this._aimProgressPercentage?.Invoke(value);
			}
		}
		[NonSerialized] private float _aimProgress;
		[NonSerialized] private float _aimSpeed = 0.5f;
		[NonSerialized] private Vector3 _aimTargetStartPosition; // also used when cannot see any target
		

		[NonSerialized] private Coroutine _fovSetCoroutine;
		[NonSerialized] private Coroutine _aimProgressCoroutine;

		

		// Cache
		[SerializeField] private CCharacterBase _ownerCharacter;
		[SerializeField] private Camera _camera;
		[NonSerialized] private RaycastHit _hitInfo; 
		[NonSerialized] private Transform _myTransform;
		[NonSerialized] private Vector3 _position;
		[NonSerialized] private Vector3 _forward;

		
		
		
		#region <<---------- MonoBehaviour ---------->>
		
		private void Awake() {
			if (this._ownerCharacter == null || this._camera == null) {
				Debug.LogError($"CameraAim script being destroyed because there is no Character set as owner to set target or there is no camera attached to it.");
				Destroy(this);
			}
			this._myTransform = this.transform;
			this._aimTargetStartPosition = this._myTransform.position;
			this.AimProgress = 0f;
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

			this._aimTargetPosition.position = hit ? 
					this._hitInfo.point : 
					Vector3.Lerp(this._aimTargetPosition.position, this._aimTargetStartPosition, this._aimSpeed * Time.deltaTime * Time.timeScale);
			
			this._ownerCharacter.SetAimDirection(this._forward);
		}

		#endregion <<---------- MonoBehaviour ---------->>


		

		#region <<---------- Coroutines ---------->>

		private IEnumerator SmoothSetAimProgress(float targetValue) {
			float elapsedTime = 0f;
			while (elapsedTime < this._aimSpeed) {
				this.AimProgress = this._aimProgress.CLerp(targetValue, (elapsedTime / this._aimSpeed));
				elapsedTime += Time.deltaTime * Time.timeScale;
				yield return null;
			}
		}
		
		private IEnumerator SmoothSetFov(float targetFov, float time) {
			float elapsedTime = 0f;
			while (elapsedTime < time) {
				float fov = this._camera.fieldOfView;
				float lerpTime = (elapsedTime / time);
				this._camera.fieldOfView = fov.CLerp(targetFov, lerpTime);
				elapsedTime += Time.deltaTime * Time.timeScale;
				yield return null;
			}
			this._camera.fieldOfView = targetFov;
		}
		
		#endregion <<---------- Coroutines ---------->>

	}
}

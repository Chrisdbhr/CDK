using System;
using System.Collections;
using UnityEngine;

namespace CDK {
	public class CPlayerHead : MonoBehaviour {

		[SerializeField] private CCharacter_Base _characterBase;
		[NonSerialized] private Transform _transform;
		[NonSerialized] private float _initialY;
		[NonSerialized] private Coroutine _lerpCoroutine;
		[NonSerialized] private float _crouchTime = 0.25f;
		
	
		
		
		
		private void Awake() {
			this._transform = this.transform;
			this._initialY = this._transform.localPosition.y;
			if (this._crouchTime <= 0f) this._crouchTime = 0.1f;
		}

		private void OnEnable() {
			// this._characterBase.OnCrouchedStateChanged += this.CrouchedStateChanged;
		}

		private void OnDisable() {
			// this._characterBase.OnCrouchedStateChanged -= this.CrouchedStateChanged;
		}

		private void CrouchedStateChanged(bool crouched) {
			var pos = this._transform.localPosition;
			pos.y  = this._initialY * (crouched ? 0.5f : 1f);
			this.CStopCoroutine(this._lerpCoroutine);
			this._lerpCoroutine = this.CStartCoroutine(this.CrouchRoutine(pos));
		}

		IEnumerator CrouchRoutine(Vector3 targetPos) {
			float time = 0f;
			var initialPos = this._transform.localPosition;
			while (time < this._crouchTime) {
				this._transform.localPosition = Vector3.Lerp(initialPos, targetPos, time / this._crouchTime);
				time += CTime.DeltaTimeScaled;
				yield return null;
			}
			this._transform.localPosition = targetPos;
		}
		
	}
}

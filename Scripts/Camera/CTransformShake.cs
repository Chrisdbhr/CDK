using System;
using System.Collections;
using UnityEngine;

namespace CDK {
	public class CTransformShake : MonoBehaviour {

		[NonSerialized] private Coroutine _shakeRoutine;
		[NonSerialized] private Transform _transform;
		
		
		private void Awake() {
			this._transform = this.transform;
			if (this._transform.localPosition != Vector3.zero) {
				Debug.LogWarning($"{this.name} has the script CTransformShake but it doesnt started at Vector3.zero position. Fixing it.");
				this._transform.localPosition = Vector3.zero;
			}
		}

		
		
		
		public void RequestShake(Vector3 direction, AnimationCurve animationCurve) {
			this._transform.localPosition = Vector3.zero;
			this.CStopCoroutine(this._shakeRoutine);
			this._shakeRoutine = this.CStartCoroutine(this.ShakeRoutine(direction, animationCurve));
		}

		private IEnumerator ShakeRoutine(Vector3 direction, AnimationCurve animationCurve) {
			if (animationCurve.length <= 0) {
				Debug.LogError($"Tried to Shake transform with a invalid Animation Curve (animationCurve.length invalid).");
				yield break;
			}

			if (direction == Vector3.zero) {
				Debug.Log($"Will not shake transform because requested direction was zero.");
				yield break;
			}

			float elapsedTime = 0f;
			while (elapsedTime <= animationCurve[animationCurve.length - 1].time) {
				elapsedTime += CTime.DeltaTimeScaled;
				this._transform.localPosition = direction * animationCurve.Evaluate(elapsedTime);
				yield return null;
			}
			yield return null;

			this._transform.localPosition = Vector3.zero;
		}
		
	}
}

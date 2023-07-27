using System.Collections;
using UnityEngine;

namespace CDK {
	public class CTransformShake : MonoBehaviour {

		private Coroutine _shakeRoutine;
		private Vector3 _startLocalPosition = Vector3.zero;
		
		private Transform _transform;
		
		
		private void Awake() {
			this._transform = this.transform;
			this._startLocalPosition = this._transform.localPosition;
		}

		
		
		
		public void RequestShake(Vector3 direction, AnimationCurve animationCurve, float shakeMultiplier) {
			this.CStopCoroutine(this._shakeRoutine);
			this._shakeRoutine = this.CStartCoroutine(this.ShakeRoutine(direction, animationCurve, shakeMultiplier));
		}

		private IEnumerator ShakeRoutine(Vector3 direction, AnimationCurve animationCurve, float shakeMultiplier) {
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
				this._transform.localPosition = this._startLocalPosition + (direction * (animationCurve.Evaluate(elapsedTime) * shakeMultiplier));
				yield return null;
			}
			yield return null;

			this._transform.localPosition = this._startLocalPosition;
		}
		
	}
}

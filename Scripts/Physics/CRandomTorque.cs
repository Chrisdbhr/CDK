using UnityEngine;

namespace CDK {
	public class CRandomTorque : CAutoTriggerCompBase {
		
		[SerializeField] private Vector3 _torqueRangeMin = new Vector3(0f,-1f,0f);
		[SerializeField] private Vector3 _torqueRangeMax = new Vector3(0f,1f,0f);
		[SerializeField] private Rigidbody _rb;
		
		
		
		
		protected override void TriggerEvent() {
			if (_rb == null) return;
			_rb.AddTorque(
				UnityEngine.Random.Range(this._torqueRangeMin.x, this._torqueRangeMax.x),
				UnityEngine.Random.Range(this._torqueRangeMin.y, this._torqueRangeMax.y),
				UnityEngine.Random.Range(this._torqueRangeMin.z, this._torqueRangeMax.z)
			);
		}

		private void Reset() {
			if(_rb == null) _rb = this.GetComponent<Rigidbody>();
		}
	}
}

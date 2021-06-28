using System;
using UnityEngine;

namespace CDK {
	[RequireComponent(typeof(Rigidbody))]
	public class CRigidbodySetVelocityTrigger : CAutoTriggerCompBase {
		[SerializeField] private Vector3 _setVelocityAmount = Vector3.forward;
		[NonSerialized] private Rigidbody _rb;
		
		
		
		
		protected override void Awake() {
			this._rb = this.GetComponent<Rigidbody>();
			base.Awake();
		}

		protected override void TriggerEvent() {
			this._rb.velocity = this.transform.TransformDirection(this._setVelocityAmount);
		}
		
	}
}

using System;
using UnityEngine;

namespace CDK {
	[RequireComponent(typeof(Rigidbody))]
	public class CRigidbodySetVelocityTrigger : CAutoTriggerCompBase {
		[SerializeField] Vector3 _setVelocityAmount = Vector3.forward;
		[NonSerialized] Rigidbody _rb;
		
		
		
		
		protected override void Awake() {
			_rb = GetComponent<Rigidbody>();
			base.Awake();
		}

		protected override void TriggerEvent() {
			_rb.linearVelocity = transform.TransformDirection(_setVelocityAmount);
		}
		
	}
}

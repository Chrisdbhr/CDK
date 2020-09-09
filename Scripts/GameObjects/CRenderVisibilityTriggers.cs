using System;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CRenderVisibilityTriggers : MonoBehaviour {

		[SerializeField] private UnityEvent _becameVisibleEvent;
		[SerializeField] private UnityEvent _becameInvisibleEvent;

		public Action BecameVisibleAction;
		public Action BecameInvisibleAction;
		
		private void OnEnable() {
			// enable checkbox
		}

		private void OnBecameVisible() {
			if (!this.enabled) return;
			this._becameVisibleEvent?.Invoke();
			this.BecameVisibleAction?.Invoke();
		}

		private void OnBecameInvisible() {
			if (!this.enabled) return;
			this._becameInvisibleEvent?.Invoke();
			this.BecameInvisibleAction?.Invoke();
		}
		
	}
}

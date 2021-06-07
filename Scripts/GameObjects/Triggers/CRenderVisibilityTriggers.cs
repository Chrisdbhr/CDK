using System;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CRenderVisibilityTriggers : MonoBehaviour {

		[SerializeField] private UnityEvent _becameVisibleEvent;
		[SerializeField] private UnityEvent _becameInvisibleEvent;
		[NonSerialized] private bool _isVisible;
		
		public Action BecameVisibleAction;
		public Action BecameInvisibleAction;
		
		
		
		
		#region <<---------- MonoBehaviour ---------->>
		
		private void OnEnable() {
			if (this._isVisible) {
				this.BecameVisibleInvoke();
			}
			else {
				this.BecameInvisibleInvoke();
			}
		}
		
		private void OnBecameVisible() {
			this._isVisible = true;
			if (!this.enabled) return;
			this.BecameVisibleInvoke();
		}

		private void OnBecameInvisible() {
			this._isVisible = false;
			if (!this.enabled) return;
			this.BecameInvisibleInvoke();
		}

		#endregion <<---------- MonoBehaviour ---------->>
		
		
		
		
		#region <<---------- Events Invoke ---------->>
		
		private void BecameVisibleInvoke() {
			this._becameVisibleEvent?.Invoke();
			this.BecameVisibleAction?.Invoke();
		}
		
		private void BecameInvisibleInvoke() {
			this._becameInvisibleEvent?.Invoke();
			this.BecameInvisibleAction?.Invoke();
		}
		
		#endregion <<---------- Events Invoke ---------->>
		
	}
}

using System;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CRenderVisibilityTriggers : MonoBehaviour {

		[SerializeField] UnityEvent _becameVisibleEvent;
		[SerializeField] UnityEvent _becameInvisibleEvent;
		[NonSerialized] bool _isVisible;
		
		public Action BecameVisibleAction;
		public Action BecameInvisibleAction;
		
		
		
		
		#region <<---------- MonoBehaviour ---------->>

		void OnEnable() {
			if (_isVisible) {
				BecameVisibleInvoke();
			}
			else {
				BecameInvisibleInvoke();
			}
		}

		void OnBecameVisible() {
			_isVisible = true;
			if (!enabled) return;
			BecameVisibleInvoke();
		}

		void OnBecameInvisible() {
			_isVisible = false;
			if (!enabled) return;
			BecameInvisibleInvoke();
		}

		#endregion <<---------- MonoBehaviour ---------->>
		
		
		
		
		#region <<---------- Events Invoke ---------->>

		void BecameVisibleInvoke() {
			_becameVisibleEvent?.Invoke();
			BecameVisibleAction?.Invoke();
		}

		void BecameInvisibleInvoke() {
			_becameInvisibleEvent?.Invoke();
			BecameInvisibleAction?.Invoke();
		}
		
		#endregion <<---------- Events Invoke ---------->>
		
	}
}

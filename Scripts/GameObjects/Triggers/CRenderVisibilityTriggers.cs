using System;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CRenderVisibilityTriggers : MonoBehaviour
	{
		[SerializeField] bool _debug;
		[SerializeField] UnityEvent _becameVisibleEvent;
		[SerializeField] UnityEvent _becameInvisibleEvent;
		[SerializeField] CUnityEventBool _visibleEvent;
		[SerializeField] CUnityEventBool _invisibleEvent;
		[NonSerialized] bool _isVisible;
		
		[NonSerialized] public Action BecameVisibleAction;
		[NonSerialized] public Action BecameInvisibleAction;
		
		
		
		
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
			_visibleEvent?.Invoke(true);
			_invisibleEvent?.Invoke(false);
			if(_debug) Debug.Log($"{this.name} became visible by some camera.");
		}

		void BecameInvisibleInvoke() {
			_becameInvisibleEvent?.Invoke();
			BecameInvisibleAction?.Invoke();
			_visibleEvent?.Invoke(false);
			_invisibleEvent?.Invoke(true);
			if(_debug) Debug.Log($"{this.name} became invisible by all cameras.");
		}
		
		#endregion <<---------- Events Invoke ---------->>
		
	}
}

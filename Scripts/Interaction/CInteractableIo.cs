using System;
using UnityEngine;
using UnityEngine.Events;

namespace CDK.Interaction {
    [Obsolete("This script needs revision")]
	public class CInteractableIo : CInteractable {

		#region <<---------- Properties and Fields ---------->>
		
		public bool On {
			get { return _on; }
			set {
				if (_on == value) return;
				_on = value;
				NewStateEvent?.Invoke(_on);
				if (_on) {
					StateOnEvent?.Invoke();
				}
				else {
					StateOffEvent?.Invoke();
				}
				Debug.Log($"Interact {_on}");
			}
		}
		[SerializeField] private bool _on;

		[SerializeField] private bool triggerOnStart = true;

		private bool Locked {
			get { return _locked; }
			set {
				if (_locked == value) return;
				_locked = value;
				LockStateChangedEvent?.Invoke(_locked);
				if (_locked) {
					LockEvent?.Invoke();
				}
				else {
					UnlockEvent?.Invoke();
				}
			}
		}
		[SerializeField] private bool _locked;

		[SerializeField] private CUnityEventBool NewStateEvent;
		[SerializeField] private CUnityEvent StateOnEvent;
		[SerializeField] private CUnityEvent StateOffEvent;
		[Header("Lock/Unlock")]
		[SerializeField] private CUnityEventBool LockStateChangedEvent;
		[SerializeField] private UnityEvent LockEvent;
		[SerializeField] private UnityEvent UnlockEvent;
		[SerializeField] private CUnityEventTransform InteractedWhenLocked;

		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- MonoBehaviour ---------->>

		private void Start() {
			if(triggerOnStart) NewStateEvent?.Invoke(On);
		}

		#if UNITY_EDITOR
		private void Reset() {
			if (gameObject.layer != 15) {
				Debug.Log($"Settings {name} layer to {LayerMask.LayerToName(15)}");
				gameObject.layer = 15;
			}
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>



		
		#region <<---------- CInteractable ---------->>

        public override bool CanBeInteractedWith() {
            return base.CanBeInteractedWith() && !_locked;
        }

		public override void OnBecameInteractionTarget(Transform lookingTransform) {
			
		}

		public override bool OnInteract(Transform interactingTransform) {
            if (!base.OnInteract(interactingTransform)) return false; 
			if (Locked) {
				TryToOpenWhenLocked(interactingTransform);
				return true;
			}
			SwitchState();
            return true;
        }

		#endregion <<---------- CInteractable ---------->>
		
		

		public void SwitchState() {
			if (Locked) return;
			On = !On;
		}
		
		public void SetOnOffState(bool newState) {
			if (Locked) return;
			On = newState;
		}

		public void TryToOpenWhenLocked(Transform interactingTransform) {
			InteractedWhenLocked?.Invoke(interactingTransform);
		}
		
		public void Lock() {
			Locked = true;
		}

		public void Unlock() {
			Locked = false;
		}

		public void SwitchLockState() {
			Locked = !Locked;
		}

		public void SetLockState(bool newState) {
			Locked = newState;
		}

	}
}
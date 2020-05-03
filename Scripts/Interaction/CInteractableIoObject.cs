using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CInteractableIoObject : MonoBehaviour, CIInteractable {
		
		public bool On {
			get { return this._on; }
			set {
				if (this._on == value) return;
				this._on = value;
				this.NewStateEvent?.Invoke(this._on);
				if (this._on) {
					this.StateOnEvent?.Invoke();
				}
				else {
					this.StateOffEvent?.Invoke();
				}
				Debug.Log($"Interact {this._on}");
			}
		}
		[SerializeField] private bool _on;

		[SerializeField] private bool triggerOnStart = true;

		private bool Locked {
			get { return this._locked; }
			set {
				if (this._locked == value) return;
				this._locked = value;
				this.LockStateChangedEvent?.Invoke(this._locked);
				if (this._locked) {
					this.LockEvent?.Invoke();
				}
				else {
					this.UnlockEvent?.Invoke();
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
		[SerializeField] private UnityEvent InteractedWhenLocked;




		#region <<---------- MonoBehaviour ---------->>
		
		private void Start() {
			if(this.triggerOnStart) this.NewStateEvent?.Invoke(this.On);
		}

		#if UNITY_EDITOR
		private void Reset() {
			if (this.gameObject.layer != 15) {
				Debug.Log($"Settings {this.name} layer to {LayerMask.LayerToName(15)}");
				this.gameObject.layer = 15;
			}
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>
		
		
		public void OnInteract(Transform interactingTransform) {
			if (!this.enabled || !this.gameObject.activeInHierarchy) return;

			if (this.Locked) {
				this.TryToOpenWhenLocked();
				return;
			}
			this.SwitchState();
		}

		public void SwitchState() {
			if (this.Locked) return;
			this.On = !this.On;
		}
		
		public void SetOnOffState(bool newState) {
			if (this.Locked) return;
			this.On = newState;
		}

		public void TryToOpenWhenLocked() {
			this.InteractedWhenLocked?.Invoke();
		}
		
		public void Lock() {
			this.Locked = true;
		}

		public void Unlock() {
			this.Locked = false;
		}

		public void SwitchLockState() {
			this.Locked = !this.Locked;
		}

		public void SetLockState(bool newState) {
			this.Locked = newState;
		}

	}
}
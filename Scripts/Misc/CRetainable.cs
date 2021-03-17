using System;
using UniRx;
using UnityEngine;

namespace CDK {
	public class CRetainable {

		#region <<---------- Properties and Fields ---------->>

		private int RetainCount {
			get { return this._retainCount; }
			set {
				if (this._retainCount == 0) {
					if (value > 0) {
						this._onRetain?.Invoke();
						this._onIsRetained?.Invoke(true);
						this._isRetainedRx.Value = true;
					}else if (value < 0) {
						Debug.LogError("Retain count reached a < 0 value! This should not happen! Will Release retainable to reduce errors.");
						Release();
						this._retainCount = 0;
						return;
					}
				}else if (this._retainCount > 0 && value == 0) {
					Release();
				}

				void Release() {
					this._onRelease?.Invoke();
					this._onIsRetained?.Invoke(false);
					this._isRetainedRx.Value = false;
				}

				this._retainCount = value;
			}
		}
		private int _retainCount = 0;

		public event Action OnRetain{
			add {
				this._onRetain -= value;
				this._onRetain += value;
			}
			remove {
				this._onRetain -= value;
			}
		}
		private event Action _onRetain;

		public event Action OnRelease{
			add {
				this._onRelease -= value;
				this._onRelease += value;
			}
			remove {
				this._onRelease -= value;
			}
		}
		private event Action _onRelease;
		
		public event Action<bool> OnIsRetained{
			add {
				this._onIsRetained -= value;
				this._onIsRetained += value;
			}
			remove {
				this._onIsRetained -= value;
			}
		}
		private event Action<bool> _onIsRetained;

		public ReadOnlyReactiveProperty<bool> IsRetainedRx => this._isRetainedRx.ToReadOnlyReactiveProperty();
		private ReactiveProperty<bool> _isRetainedRx;

		#endregion <<---------- Properties and Fields ---------->>

		
		

		#region <<---------- Initializers ---------->>

		public CRetainable() {
			this._isRetainedRx = new ReactiveProperty<bool>();
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		
		
		#region <<---------- General ---------->>
		
		public void Retain() {
			this.RetainCount += 1;
		}

		public void Release() {
			this.RetainCount -= 1;
		}

		public bool IsRetained() {
			return this._retainCount > 0;
		}
		
		#endregion <<---------- General ---------->>

	}
}

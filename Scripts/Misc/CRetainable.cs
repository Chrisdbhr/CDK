using System;
using UnityEngine;

namespace CDK {
	public class CRetainable {

		#region <<---------- Properties and Fields ---------->>
		
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

		
		#endregion <<---------- Properties and Fields ---------->>

		
		#region <<---------- General ---------->>
		
		public void Retain() {
			this._retainCount += 1;
			this._onRetain?.Invoke();
			this._onIsRetained?.Invoke(true);
		}

		public void Release() {
			this._retainCount -= 1;
			this._onRelease?.Invoke();
			this._onIsRetained?.Invoke(false);

			if (this._retainCount < 0) {
				Debug.LogError("Retain count reached a < 0 value!");
			}
		}

		public bool IsRetained() {
			return this._retainCount > 0;
		}
		
		#endregion <<---------- General ---------->>

	}
}

using System;
using System.Threading.Tasks;
using FMODUnity;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CDK.UI {
	public abstract class CUIBase : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		[Header("Setup")]
		[SerializeField] protected CUIInteractable _firstSelected;
		public CUIInteractable FirstSelected => this._firstSelected;
		[SerializeField] private Canvas _canvas;
		[SerializeField] private bool _hideWhenBehind;
		
		[Header("Sounds")]
		[SerializeField] [EventRef] protected string _soundOpenMenu;
		[SerializeField] [EventRef] protected string _soundCloseMenu;
		
		[NonSerialized] private CUIBase _previousUI;
		[NonSerialized] private CUIButton _previousButton;

		public event Action OnOpen {
			add {
				this._onOpen -= value;
				this._onOpen += value;
			}
			remove {
				this._onOpen -= value;
			}
		}
		[NonSerialized] private Action _onOpen;
		
		public event Action OnClose {
			add {
				this._onClose -= value;
				this._onClose += value;
			}
			remove {
				this._onClose -= value;
			}
		}
		[NonSerialized] private Action _onClose;

		#endregion <<---------- Properties and Fields ---------->>

		
		
		
		#region <<---------- Open / Close ---------->>

		public async Task Open(int sortOrder) {
			this._onOpen?.Invoke();
			this._canvas.sortingOrder = sortOrder;
			RuntimeManager.PlayOneShot(this._soundOpenMenu);
			Debug.Log($"Opening menu {this.name}");
		}
		public async Task Close() {
			this._onClose?.Invoke();
			RuntimeManager.PlayOneShot(this._soundCloseMenu);
			Debug.Log($"Closing menu {this.name}");
			Addressables.ReleaseInstance(this.gameObject);
			Destroy(this.gameObject);
		}
		
		#endregion <<---------- Open / Close ---------->>




		#region <<---------- Visibility ---------->>
		
		public void HideIfSet() {
			if (!this._hideWhenBehind) return;
			this.gameObject.SetActive(false);
			//this._canvas.enabled = false;
		}

		public void ShowIfHidden() {
			if (!this._hideWhenBehind) return;
			this.gameObject.SetActive(true);
			//this._canvas.enabled = true;
		}
		
		#endregion <<---------- Visibility ---------->>

	}
}

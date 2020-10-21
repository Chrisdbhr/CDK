using System;
using UnityEngine;

namespace CDK {
	public class CUIInteractable : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private CPlayerInteractor _playerInteractor;

		[SerializeField] private CUnityEventBool HasInteractableEvent;
		
		#endregion <<---------- Properties and Fields ---------->>

		
		

		#region <<---------- MonoBehaviour ---------->>
		
		private void Awake() {
			this.HasInteractableEvent?.Invoke(false);
		}

		private void OnEnable() {
			this._playerInteractor.OnInteractableChanged += this.InteractableChanged;
		}

		private void OnDisable() {
			this._playerInteractor.OnInteractableChanged -= this.InteractableChanged;
		}
		
		#endregion <<---------- MonoBehaviour ---------->>


		

		#region <<---------- Callbacks ---------->>

		private void InteractableChanged(CInteractableObject interactable) {
			this.HasInteractableEvent?.Invoke(interactable != null);
		}

		#endregion <<---------- Callbacks ---------->>
		
	}
}

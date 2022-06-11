using CDK.Interaction;
using UnityEngine;

namespace CDK {
	public class CHasInteractableTrigger : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private CPlayerInteractor _playerInteractor;

		[SerializeField] private CUnityEventBool HasInteractableEvent;
		
		#endregion <<---------- Properties and Fields ---------->>

		
		

		#region <<---------- MonoBehaviour ---------->>
		
		private void OnEnable() {
			this._playerInteractor.OnInteractableChanged += this.InteractableChanged;
		}

		private void OnDisable() {
			this._playerInteractor.OnInteractableChanged -= this.InteractableChanged;
		}
		
		#endregion <<---------- MonoBehaviour ---------->>


		

		#region <<---------- Callbacks ---------->>

		private void InteractableChanged(CInteractable interactable) {
			this.HasInteractableEvent?.Invoke(interactable != null);
		}

		#endregion <<---------- Callbacks ---------->>
		
	}
}

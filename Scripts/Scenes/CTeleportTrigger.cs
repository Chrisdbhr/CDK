using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CTeleportTrigger : MonoBehaviour, CIInteractable {
		
		[SerializeField] private CSceneField targetScene;
		[SerializeField] private int _targetEntryPoint;
		[SerializeField] private bool onlyWorkOneTimePerSceneLoad = true;
		
		[SerializeField] private CPlayerCamera.CameraTransitionType cameraTransitionType;

		[SerializeField] private UnityEvent _onTeleport;
		



		[NonSerialized] private AsyncOperation asyncOp;
		[NonSerialized] private float nextTime;
		[NonSerialized] private bool interacted;


		

		#region <<---------- Teleport ---------->>
		
		private void Teleport(Transform objTriggeringTeleport) {
			var triggerCharacter = objTriggeringTeleport.GetComponent<CCharacterBase>();
			if (triggerCharacter == null) {
				Debug.LogWarning($"Transform {objTriggeringTeleport} triggered TeleportTrigger {this.name} but it has no Character attached to it!", objTriggeringTeleport);
				return;
			}

			var relatedGameObjects = CGamePlayerManager.get.GetAllGameObjectsRelatedToCharacter(triggerCharacter);
			if (relatedGameObjects.Count <= 0) {
				Debug.LogWarning($"Transform {objTriggeringTeleport} triggered TeleportTrigger {this.name} but can find any GamePlayer that controls this Character!", objTriggeringTeleport);
				return;
			}

			this._onTeleport?.Invoke();
			
			CSceneManager.Teleport(this.targetScene, this._targetEntryPoint, relatedGameObjects).CAwait();
		}
		
		#endregion <<---------- Teleport ---------->>

		
		#region <<---------- IInteractable ---------->>
		
		public void OnLookTo(Transform lookingTransform) {
			throw new NotImplementedException();
		}

		public void OnInteract(Transform interactingTransform) {
			if (this.onlyWorkOneTimePerSceneLoad && this.interacted) {
				Debug.LogWarning($"Already interacted with {this.name}. Will not trigger OnInteract().");
				return;
			}
			if (CBlockingEventsManager.IsAnyBlockingEventHappening) return;

			this.interacted = true;
			this.Teleport(interactingTransform);
		}
		
		#endregion <<---------- IInteractable ---------->>
		
		
	}
	
}
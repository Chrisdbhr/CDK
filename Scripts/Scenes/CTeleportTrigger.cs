using System;
using System.Collections.Generic;
using CDK;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CDK {
	public class CTeleportTrigger : MonoBehaviour, CIInteractable {

		[SerializeField] private Vector3 targetPosition;
		[SerializeField] private CSceneField targetScene;
		[SerializeField] private int _targetEntryPoint;
		[SerializeField] private bool onlyWorkOneTimePerSceneLoad = true;
		
		[SerializeField] private CPlayerCamera.CameraTransitionType cameraTransitionType;



		[NonSerialized] private AsyncOperation asyncOp;
		[NonSerialized] private float nextTime;
		[NonSerialized] private bool interacted;




		#region <<---------- MonoBehaviour ---------->>
		
		#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			Handles.color = Color.cyan;
			Handles.Label(this.targetPosition + (Vector3.up * 1f), "Teleport target");
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(this.targetPosition, 0.1f);
			Gizmos.DrawLine(this.transform.position, this.targetPosition);
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>



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
			if (CBlockingEventsManager.IsBlockingEventHappening) return;

			this.interacted = true;
			this.Teleport(interactingTransform);
		}
		
		#endregion <<---------- IInteractable ---------->>
		
		
		
		#region <<---------- Editor ---------->>
		
		#if UNITY_EDITOR

		public void EditorFocusTeleportPosition() {
			SceneView.lastActiveSceneView.Frame(new Bounds(this.targetPosition, Vector3.one*3f), false);
		}
		#endif
		
		#endregion <<---------- Editor ---------->>
		
	}


#if UNITY_EDITOR
	[CustomEditor(typeof(CTeleportTrigger))]
	public class TeleportTriggerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			this.DrawDefaultInspector();
			if (!(this.target is CTeleportTrigger myScript)) return;
			if(GUILayout.Button("Focus Teleport Position")) {
				myScript.EditorFocusTeleportPosition();
			}
		}
	}
#endif
	
	
}
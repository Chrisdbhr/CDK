using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;		
#endif

namespace CDK.DataPersistence {
	public class CSave_Component : MonoBehaviour {

		[SerializeField] private CSaveGameObjectData saveGameObjectData;
		[NonSerialized] private bool _loadedData;
		
		
		[SerializeField] private bool SavePosition = true;
		[SerializeField] private bool SaveRotation = true;
		
		

		#region <<---------- MonoBehaviour ---------->>
		private void Awake() {
			// load and cache data
			var loadedData = CSaveManager.get.LoadData_GameObject(this.saveGameObjectData.MyUid);
			if (loadedData != null) {
				this.saveGameObjectData = loadedData;
				this.transform.position = this.saveGameObjectData.Saved_Pos;
				this.transform.rotation = this.saveGameObjectData.Saved_Rotation;
			}
		}

		private void Start() {
			if (!this._loadedData) return;

			// restore position
			if (this.SavePosition) {
				this.transform.position = this.saveGameObjectData.Saved_Pos;
			}
			
			// restore rotation
			if (this.SaveRotation) {
				this.transform.rotation = this.saveGameObjectData.Saved_Rotation;
			}
		}

		private void OnDestroy() {
			this.saveGameObjectData.Saved_Pos = this.transform.position;
			this.saveGameObjectData.Saved_Rotation = this.transform.rotation;
			CSaveManager.get.SaveData_GameObject(this.saveGameObjectData);
		}
		
		#endregion <<---------- MonoBehaviour ---------->>



		#if UNITY_EDITOR
		public bool GetNewGuidIfNeeded() {
			return this.saveGameObjectData.GenerateNewUidIfNeeded();
		}
		#endif
		
	}
	
	#if UNITY_EDITOR
	[CustomEditor(typeof(CSave_Component))]
	public class CSaveGameObjectEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			this.DrawDefaultInspector();
        
			var myScript = (CSave_Component) this.target;
			if(GUILayout.Button("Get new GUID")) {
				var allSaveComponents = FindObjectsOfType<CSave_Component>();
				if (allSaveComponents == null) return;
				int numberOfIdsCreated = 0;
				for (int i = 0; i < allSaveComponents.Length; i++) {
					if (allSaveComponents[i].GetNewGuidIfNeeded()) {
						EditorUtility.SetDirty(allSaveComponents[i]);
						Debug.Log($"Assign id to object {allSaveComponents[i].gameObject.name}.", allSaveComponents[i]);
						numberOfIdsCreated+=1;
					}
				}
				if (numberOfIdsCreated > 0) {
					Debug.Log($"Assigned {numberOfIdsCreated} new ids.");
				}
				else {
					Debug.Log($"No new ids assigned.");
				}
				if(!Application.isPlaying) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(myScript.gameObject.scene);
			}
			if(Application.isPlaying && GUILayout.Button("Save to disk")) {
				CSaveManager.get.SaveToDisk().Wait();
			}
		}
	}
	#endif

}
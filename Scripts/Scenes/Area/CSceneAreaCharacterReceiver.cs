using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CDK {
	public class CSceneAreaCharacterReceiver : MonoBehaviour {

		[NonSerialized] private CCharacterBase _characterBase;
		[NonSerialized] private List<CSceneAreaData> _activeAreas = new List<CSceneAreaData>();

		
		
		
		private void Awake() {
			this._characterBase = this.GetComponent<CCharacterBase>();
		}

		
		
		
		public void AddArea(CSceneAreaData data) {
			if (data == null) {
				Debug.LogError($"Tried to Add null {nameof(CSceneAreaData)}");
				return;
			}
			Debug.Log($"Entering Scene Area {data.name}");
			this._activeAreas.Insert(data.Priority, data);
			this.CheckIfNeedToApplyValues();
		}
		
		public void RemoveArea(CSceneAreaData data) {
			if (data == null) return;
			this._activeAreas.Remove(data);
			Debug.Log($"Removing Scene Area {data.name}");
			this.CheckIfNeedToApplyValues();
		}


		private void CheckIfNeedToApplyValues() {
			if (this._activeAreas.Count <= 0) {
				this._characterBase.BlockRunFromEnvironment = false;
				return;
			}
			var activeData = this._activeAreas.Count == 1 ? this._activeAreas[0] : this._activeAreas.Last();
			this._characterBase.BlockRunFromEnvironment = !activeData.CanRun;
		}
	}
}

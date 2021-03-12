using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDK {
	public class CSceneArea : MonoBehaviour {
		[SerializeField] private CSceneAreaData _data;
		[NonSerialized] private HashSet<CSceneAreaCharacterReceiver> _affectedReceivers = new HashSet<CSceneAreaCharacterReceiver>();
		
		
		private void OnDestroy() {
			foreach (var receiver in this._affectedReceivers) {
				if (receiver == null) continue;
				receiver.RemoveArea(this._data);
			}
		}

		private void OnTriggerEnter(Collider other) {
			var receiver = other.GetComponent<CSceneAreaCharacterReceiver>();
			if (receiver == null) return;
			this._affectedReceivers.Add(receiver);
			receiver.AddArea(this._data);
		}

		private void OnTriggerExit(Collider other) {
			var receiver = other.GetComponent<CSceneAreaCharacterReceiver>();
			if (receiver == null) return;
			this._affectedReceivers.Remove(receiver);
			receiver.RemoveArea(this._data);
		}

		
	}
}
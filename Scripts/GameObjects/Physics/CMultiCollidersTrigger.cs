using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDK {
	public class CMultiCollidersTrigger : CPhysicsTrigger {

		[NonSerialized] private List<Transform> _transformsInside;

		private void OnEnable() {
			this._transformsInside = new List<Transform>();
		}

		private void OnDisable() {
			this._transformsInside.Clear();
			this._transformsInside = null;
		}

		protected override void StartedCollisionOrTrigger(Transform transf) {
			if (!this._transformsInside.Contains(transf)) {
				base.StartedCollisionOrTrigger(transf);
			}
			this._transformsInside.Add(transf);
		}

		protected override void ExitedCollisionOrTrigger(Transform transf) {
			if (!this._transformsInside.Remove(transf)) {
				Debug.LogError($"Tried to remove a transform from '{nameof(this._transformsInside)}' but it wasnt on list! This can lead to unpredicable behaviour.", this);
			}
			if (!this._transformsInside.Contains(transf)) {
				base.ExitedCollisionOrTrigger(transf);
			}
		}
		
	}
}

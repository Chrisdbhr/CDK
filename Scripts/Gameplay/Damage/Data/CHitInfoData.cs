using System;
using UnityEngine;

namespace CDK.Data {
	[System.Serializable]
	public class CHitInfoData {

		public CHitInfoScriptableObject ScriptableObject;

		public Transform AttackerRootTransform;
		public Vector3 HitPointPosition;



		public CHitInfoData(CHitInfoScriptableObject scriptableObject, Vector3 hitPointPosition, Transform attackerRootTransform) {
			this.ScriptableObject = scriptableObject;
			this.AttackerRootTransform = attackerRootTransform;
			this.HitPointPosition = hitPointPosition;
		}
		
	}
}

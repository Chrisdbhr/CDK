using UnityEngine;

namespace CDK.Data {
	[System.Serializable]
	public class CHitInfoData {

		public CHitInfoScriptableObject ScriptableObject;
		public Transform AttackerTransform;
		public Vector3 HitPointPosition;

		public CHitInfoData(CHitInfoScriptableObject scriptableObject, Vector3 hitPointPosition, Transform attackerTransform) {
			this.ScriptableObject = scriptableObject;
			this.AttackerTransform = attackerTransform;
			this.HitPointPosition = hitPointPosition;
		}
		
	}
}
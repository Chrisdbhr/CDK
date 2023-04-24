using UnityEngine;

namespace CDK.Data {
	[System.Serializable]
	public class CHitInfoData {

		public CHitInfoScriptableObject ScriptableObject;
		public Transform AttackerTransform;
		public Vector3 HitPointPosition;

		public CHitInfoData(CHitInfoScriptableObject scriptableObject, Vector3 hitPointPosition, Transform attackerRootTransform) {
			this.ScriptableObject = scriptableObject;
			this.AttackerTransform = attackerRootTransform;
			this.HitPointPosition = hitPointPosition;
		}
		
	}
}

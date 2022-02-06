using System;
using UnityEngine;

namespace CDK.Data {
	[System.Serializable]
	public class CHitInfoData {

		public CHitInfoScriptableObject ScriptableObject;

		[NonSerialized] public Transform AttackerRootTransform;
		[NonSerialized] public Vector3 HitPointPosition;
		[NonSerialized] public float DamageMultiplier = 1f;



		public CHitInfoData(CHitInfoScriptableObject scriptableObject, Vector3 hitPointPosition, Transform attackerRootTransform) {
			this.ScriptableObject = scriptableObject;
			this.AttackerRootTransform = attackerRootTransform;
			this.HitPointPosition = hitPointPosition;
		}
		
	}
}

using System;
using UnityEngine;

namespace CDK.Data {
	[System.Serializable]
	public class CHitInfoData {

		public CHitInfoScriptableObject ScriptableObject;
		[NonSerialized] public CCharacterBase Attacker;
		[NonSerialized] public Vector3 HitPointPosition;


		public CHitInfoData(CHitInfoScriptableObject scriptableObject, Vector3 hitPointPosition, CCharacterBase attacker) {
			this.ScriptableObject = scriptableObject;
			this.Attacker = attacker;
			this.HitPointPosition = hitPointPosition;
		}
		
	}
}

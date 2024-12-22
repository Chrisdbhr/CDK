using UnityEngine;
using UnityEngine.Serialization;

namespace CDK.Data {
	[System.Serializable]
	public class CAttackData {

		[FormerlySerializedAs("ScriptableObject")] public CHitInfoData data;
		public Transform AttackerTransform;
		public Vector3 HitPointPosition;
		public float Damage => data.RawDamage;

		public CAttackData(CHitInfoData data, Vector3 hitPointPosition, Transform attackerTransform) {
			if (data == null) {
				// log error:
				Debug.LogError("CAttackData: data is null");
			}
			this.data = data;
			AttackerTransform = attackerTransform;
			HitPointPosition = hitPointPosition;
		}
		
	}
}
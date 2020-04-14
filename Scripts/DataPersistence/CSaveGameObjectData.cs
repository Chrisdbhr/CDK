using UnityEngine;

namespace CDK.DataPersistence {
	[System.Serializable]
	public class CSaveGameObjectData : CSaveData {

		#if UNITY_EDITOR
		public bool GenerateNewUidIfNeeded() {
			bool needToGenerate = this.MyUid.CIsNullOrEmpty();
			if(needToGenerate) this.MyUid = System.Guid.NewGuid().ToString();
			return needToGenerate;
		}
		#endif
		
		[CReadOnly] public Vector3 Saved_Pos;
		[CReadOnly] public Quaternion Saved_Rotation;

	}
}
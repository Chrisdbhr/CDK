
using UnityEngine;

namespace CDK {
	public class CCollectableAmmoGameObject : CCollectableItemGameObject{
	
		[SerializeField] CAmmoData AmmoHere;
		
		
		
		
		#region <<---------- MonoBehaviour ---------->>
		
		protected override void Awake() {
            base.Awake();
			if (AmmoHere == null) {
				Debug.LogError($"{GetType()} has no data set, destroying game object.");
				Destroy(gameObject);
			}
		}

		#endregion <<---------- MonoBehaviour ---------->>

		


		#region <<---------- CCollectableItemGameObject ---------->>

		public override void SetItemHere(CIItemBase itemData) {
			AmmoHere = itemData as CAmmoData;
		}
		
		public override CIItemBase GetItemHere() {
			return AmmoHere;
		}
		
		#endregion <<---------- CCollectableItemGameObject ---------->>
	}
}

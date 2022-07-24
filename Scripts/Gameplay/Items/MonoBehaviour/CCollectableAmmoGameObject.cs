
using UnityEngine;

namespace CDK {
	public class CCollectableAmmoGameObject : CCollectableItemGameObject{
	
		[SerializeField] private CAmmoData AmmoHere;
		
		
		
		
		#region <<---------- MonoBehaviour ---------->>
		
		protected override void Awake() {
            base.Awake();
			if (this.AmmoHere == null) {
				Debug.LogError($"{this.GetType()} has no data set, destroying game object.");
				Destroy(this.gameObject);
			}
		}

		#endregion <<---------- MonoBehaviour ---------->>

		


		#region <<---------- CCollectableItemGameObject ---------->>

		public override void SetItemHere(CIItemBase itemData) {
			this.AmmoHere = itemData as CAmmoData;
		}
		
		public override CIItemBase GetItemHere() {
			return this.AmmoHere;
		}
		
		#endregion <<---------- CCollectableItemGameObject ---------->>
	}
}

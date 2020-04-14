using CDK.Inventory;
using UnityEngine;

namespace CDK {
	public class CCollectableAmmoGameObject : CCollectableItemGameObject{
	
		[SerializeField] private CAmmoData AmmoHere;
		
		
		
		
		#region <<---------- MonoBehaviour ---------->>
		
		protected void Awake() {
			if (this.AmmoHere == null) {
				Debug.LogError($"Collectable item has no data set, destroying game object.");
				Destroy(this.gameObject);
			}
		}

		#endregion <<---------- MonoBehaviour ---------->>

		


		#region <<---------- CCollectableItemGameObject ---------->>

		public override void SetItemHere(CItemBaseData itemData) {
			this.AmmoHere = itemData as CAmmoData;
		}
		
		public override CItemBaseData GetItemHere() {
			return this.AmmoHere;
		}
		
		#endregion <<---------- CCollectableItemGameObject ---------->>
	}
}

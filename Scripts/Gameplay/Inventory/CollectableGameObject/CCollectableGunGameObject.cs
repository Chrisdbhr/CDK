using CDK.Inventory;
using UnityEngine;

namespace CDK {
	public class CCollectableGunGameObject : CCollectableItemGameObject {

		[SerializeField] private CGunData GunHere;
		
		
		
		
		#region <<---------- MonoBehaviour ---------->>
		
		protected void Awake() {
			if (this.GunHere == null) {
				Debug.LogError($"Collectable item has no data set, destroying game object.");
				Destroy(this.gameObject);
			}
		}

		#endregion <<---------- MonoBehaviour ---------->>



		
		#region <<---------- CCollectableItemGameObject ---------->>
		
		public override void SetItemHere(CItemBaseData itemData) {
			this.GunHere = itemData as CGunData;
		}
		
		public override CItemBaseData GetItemHere() {
			return this.GunHere;
		}

		#endregion <<---------- CCollectableItemGameObject ---------->>
		
	}
}

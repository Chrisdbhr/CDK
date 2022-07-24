using UnityEngine;

namespace CDK {
	public class CWeaponGameObject : CCollectableItemGameObject {
		[SerializeField] private CWeaponData WeaponHere;
		
		
		
		
		#region <<---------- MonoBehaviour ---------->>
		
		protected override void Awake() {
            base.Awake();
			if (this.WeaponHere == null) {
				Debug.LogError($"{this.GetType()} has no data set, destroying game object.");
				Destroy(this.gameObject);
			}
		}

		#endregion <<---------- MonoBehaviour ---------->>



		
		#region <<---------- CCollectableItemGameObject ---------->>
		
		public override void SetItemHere(CIItemBase itemData) {
			this.WeaponHere = itemData as CWeaponData;
		}
		
		public override CIItemBase GetItemHere() {
			return this.WeaponHere;
		}

		#endregion <<---------- CCollectableItemGameObject ---------->>
	}
}

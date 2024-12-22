using UnityEngine;

namespace CDK {
	public class CWeaponGameObject : CCollectableItemGameObject {
		[SerializeField] CWeaponData WeaponHere;
		
		
		
		
		#region <<---------- MonoBehaviour ---------->>
		
		protected override void Awake() {
            base.Awake();
			if (WeaponHere == null) {
				Debug.LogError($"{GetType()} has no data set, destroying game object.");
				Destroy(gameObject);
			}
		}

		#endregion <<---------- MonoBehaviour ---------->>



		
		#region <<---------- CCollectableItemGameObject ---------->>
		
		public override void SetItemHere(CIItemBase itemData) {
			WeaponHere = itemData as CWeaponData;
		}
		
		public override CIItemBase GetItemHere() {
			return WeaponHere;
		}

		#endregion <<---------- CCollectableItemGameObject ---------->>
	}
}

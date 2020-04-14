using CDK.Weapons;

namespace CDK.Inventory {
	[System.Serializable]
	public class CAmmoData : CItemBaseData {
		public CAmmoData(CAmmoScriptableObject item, int quantity) {
			this.ScriptableObject = item;
			this.Count = quantity;
		}
		
		public void ConsumeAmmo() {
			this.Count -= 1;
		}
	}
}

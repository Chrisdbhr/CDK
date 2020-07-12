
using UnityEngine;

namespace CDK {
	[System.Serializable]
	public class CAmmoData : CIItemBase {

		#region <<---------- Initializers ---------->>
		
		public CAmmoData(CAmmoScriptableObject item, int quantity) {
			this._ammoScriptableObject = item;
			this.Count = quantity;
		}
		
		#endregion <<---------- Initializers ---------->>

		
		
		
		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] private CAmmoScriptableObject _ammoScriptableObject;
		
		public int Count {
			get { return this._count; }
			private set {
				this._count = value < 0 ? 0 : value;
			}
		}
		private int _count;

		#endregion <<---------- Properties and Fields ---------->>

		
		

		#region <<---------- CIItemBase ---------->>
		
		public CItemBaseScriptableObject GetScriptableObject() {
			return this._ammoScriptableObject;
		}
		public int Add(int quantity) {
			return this.Count += quantity;
		}

		public int Remove(int quantity) {
			return this.Count -= quantity;
		}
		#endregion <<---------- CIItemBase ---------->>
		
		
		
		
		public void ConsumeAmmo() {
			this.Count -= 1;
		}

	}
}

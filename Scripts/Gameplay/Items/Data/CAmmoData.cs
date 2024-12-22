
using UnityEngine;

namespace CDK {
	[System.Serializable]
	public class CAmmoData : CIItemBase {

		#region <<---------- Initializers ---------->>
		
		public CAmmoData(CAmmoScriptableObject item, int quantity) {
			_ammoScriptableObject = item;
			Count = quantity;
		}
		
		#endregion <<---------- Initializers ---------->>

		
		
		
		#region <<---------- Properties and Fields ---------->>
		
		[SerializeField] CAmmoScriptableObject _ammoScriptableObject;
		
		public int Count {
			get { return _count; }
			private set {
				_count = value < 0 ? 0 : value;
			}
		}
		int _count;

		#endregion <<---------- Properties and Fields ---------->>

		
		

		#region <<---------- CIItemBase ---------->>
		
		public CItemBaseScriptableObject GetScriptableObject() {
			return _ammoScriptableObject;
		}
		public int Add(int quantity) {
			return Count += quantity;
		}

		public int Remove(int quantity) {
			return Count -= quantity;
		}
		#endregion <<---------- CIItemBase ---------->>
		
		
		
		
		public void ConsumeAmmo() {
			Count -= 1;
		}

	}
}

using UniRx;
using UnityEngine;

namespace CDK {
	public partial class CSave {

		#region <<---------- Singleton ---------->>

		public static CSave get {
			get {
				if (_instance != null) return _instance;
				Debug.Log($"Creating new instance of {nameof(CSave)}");
				CApplication.IsQuitting += () => {
					_instance = null;
				};
				return _instance = new CSave();
			}
		}
		private static CSave _instance;
		
		#endregion <<---------- Singleton ---------->>
		



		public readonly ReactiveProperty<float> CameraSensitivityMultiplierRx = new ReactiveProperty<float>(1f);
	}
}

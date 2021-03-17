using System;
using UnityEngine;
using UnityJSON;

namespace CDK {
	[JSONObject(ObjectOptions.IgnoreUnknownKey | ObjectOptions.IgnoreProperties)]
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




		public float CameraSensitivity {
			get { return this._cameraSensitivity; }
			set {
				if (this._cameraSensitivity == value) return;
				this.CameraSensitivity_Changed?.Invoke(this._cameraSensitivity = value);
			}
		}
		[JSONNode(NodeOptions.SerializeNull | NodeOptions.ReplaceDeserialized | NodeOptions.IgnoreDeserializationTypeErrors, key = "cameraSensitivity")]
		private float _cameraSensitivity = 1f;
		public event Action<float> CameraSensitivity_Changed;


	}
}

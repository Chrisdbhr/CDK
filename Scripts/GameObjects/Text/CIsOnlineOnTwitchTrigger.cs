using UnityEngine;
using UnityEngine.Networking;

namespace CDK.Text {
	/// <summary>
	/// Dunno if this works in another languages like Japanese.
	/// </summary>
	public class CIsOnlineOnTwitchTrigger : CAutoTriggerCompBase {

		[SerializeField] private string _userName = "chrisdbhr";
		[SerializeField] private CUnityEventBool _isOnline;
		
		
		
		
		protected override void TriggerEvent() {
			var get = UnityWebRequest.Get($"m.twitch.tv/{this._userName}");
			get.SendWebRequest().completed += asyncOp => {
				this._isOnline?.Invoke(get.downloadHandler.text.Contains("Stream Chat"));
				get.Dispose();
			};
		}
	}
}

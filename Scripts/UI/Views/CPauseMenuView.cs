using UnityEngine;

namespace CDK.UI {
	public class CPauseMenuView : MonoBehaviour {
		
		#region <<---------- Menu ---------->>

		public void OpenMenu() {
			CTime.SetTimeScale(0f);
			Debug.Log("TODO musics volume lower on unpause");
			Debug.Log("TODO play unpause animation");
		}
		public void CloseMenu() {
			CTime.SetTimeScale(1f);
			CBlockingEventsManager.IsOnMenu = false;
			Destroy(this.gameObject);
		}
		
		#endregion <<---------- Menu ---------->>

	}
}

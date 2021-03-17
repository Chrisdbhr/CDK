using UnityEngine;

namespace CDK {
	public static class CCursorManager {
		
		/// <summary>
		/// ANTES da scene load.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize() {
			SetCursorState(!CGameSettings.CursorStartsHidden);
			CBlockingEventsManager.OnMenu += SetCursorState;
		}


		private static void SetCursorState(bool visible) {
			Cursor.visible = visible;
			
			if (visible) {
				Cursor.lockState = CursorLockMode.None;
			}
			else {
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

	}
}

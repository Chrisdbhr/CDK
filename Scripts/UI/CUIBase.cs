using System.Threading.Tasks;
using UnityEngine;

namespace CDK.UI {
	public abstract class CUIBase : MonoBehaviour {
		public virtual async Task OpenMenu() {
			Debug.Log($"{this.name} received a OpenMenu request");
			if (CBlockingEventsManager.IsOnMenu) {
				Debug.LogWarning("Tried to open a menu when already on some Menu! Will not open.");
				return;
			}
		}

		public virtual async Task CloseMenu() {			
			Debug.Log($"{this.name} received a CloseMenu request");

		}
	}
}

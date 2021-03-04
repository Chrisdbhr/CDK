using System.Threading.Tasks;
using UnityEngine;

namespace CDK.UI {
	public abstract class CUIBase : MonoBehaviour {
		public abstract Task OpenMenu();
		public abstract Task CloseMenu();
	}
}

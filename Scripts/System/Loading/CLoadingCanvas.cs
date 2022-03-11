using System;
using UnityEngine;

namespace CDK {
	public class CLoadingCanvas : MonoBehaviour {

		[SerializeField] private CanvasGroup _loadingUI;

		private void Awake() {
			DontDestroyOnLoad(this.gameObject);
			HideLoadingUI();
		}

		public void ShowLoadingUI() {
			_loadingUI.alpha = 1f;
		}
		
		public void HideLoadingUI() {
			_loadingUI.alpha = 0f;
		}
	}
}

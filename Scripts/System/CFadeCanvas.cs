using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CDK {
	public static class CFadeCanvas {

		#region <<---------- Properties ---------->>
		
		private static CanvasGroup _fadeCanvasGroup; 
		
		#endregion <<---------- Properties ---------->>


		#region <<---------- General ---------->>
		
		public static async Task FadeToBlack(float fadeTime, bool ignoreTimeScale = true) {
			if (_fadeCanvasGroup == null) {
				await CreateFadeCanvas();
			}

			_fadeCanvasGroup.alpha = 0f;

			while (_fadeCanvasGroup.alpha < 1f) {
				await Observable.NextFrame();

				float delta = ignoreTimeScale ? Time.unscaledDeltaTime : CTime.DeltaTimeScaled;
				_fadeCanvasGroup.alpha += delta / fadeTime;
			}
			
		}

		public static async Task FadeToTransparent(float fadeTime, bool ignoreTimeScale = true) {
			if (_fadeCanvasGroup == null) {
				await CreateFadeCanvas();
			}

			_fadeCanvasGroup.alpha = 1f;

			while (_fadeCanvasGroup.alpha > 0f) {
				await Observable.NextFrame();
				
				float delta = ignoreTimeScale ? Time.unscaledDeltaTime : CTime.DeltaTimeScaled;
				_fadeCanvasGroup.alpha -= delta / fadeTime;
			}
		}
		
		private static async Task CreateFadeCanvas() {
			var parentGo = new GameObject("FadeCanvas");
			parentGo.layer = 5; // UI
			Object.DontDestroyOnLoad(parentGo);

			// canvas
			var goCanvas = parentGo.AddComponent<Canvas>();
			goCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			goCanvas.sortingOrder = 9999;
			try {
				goCanvas.sortingLayerID++;
			} catch (Exception e) {
				Console.WriteLine(e);
			}			
			
			// canvas group
			_fadeCanvasGroup = parentGo.AddComponent<CanvasGroup>();
			_fadeCanvasGroup.blocksRaycasts = false;
			_fadeCanvasGroup.interactable = false;
			
			var goImg = new GameObject("Black Image");
			goImg.layer = 5; // UI
			goImg.transform.SetParent(goCanvas.transform);
			var imgComp = goImg.AddComponent<Image>();
			imgComp.color = Color.black;
			imgComp.maskable = false;
			imgComp.raycastTarget = false;
			var imgRect = goImg.GetComponent<RectTransform>();
			imgRect.anchorMin = Vector2.zero;
			imgRect.anchorMax = Vector2.one;
			imgRect.sizeDelta = Vector2.zero;
			imgRect.anchoredPosition = Vector2.zero;
		}
		
		#endregion <<---------- General ---------->>

		
	}
}

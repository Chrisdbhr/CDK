using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CDK {
	public class CFader {

		#region <<---------- Properties ---------->>
		
		private CanvasGroup _fadeCanvasGroup;
		private float TargetAlpha;
		private float TargetFadeTime;
		private bool IgnoreTimeScale;
		
		#endregion <<---------- Properties ---------->>

		
		
		
		#region <<---------- Initializers ---------->>
		
		public CFader() {
			var parentGo = new GameObject("FadeCanvas");
			parentGo.layer = 5; // UI
			Object.DontDestroyOnLoad(parentGo);

			// canvas
			var goCanvas = parentGo.AddComponent<Canvas>();
			goCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			goCanvas.sortingOrder = -1;
			try {
				goCanvas.sortingLayerID++;
			} catch (Exception e) {
				Console.WriteLine(e);
			}
			
			// canvas group
			this._fadeCanvasGroup = parentGo.AddComponent<CanvasGroup>();
			this._fadeCanvasGroup.blocksRaycasts = false;
			this._fadeCanvasGroup.interactable = false;
			
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

			Observable.EveryUpdate().TakeUntilDestroy(parentGo).Subscribe(_ => {
				this.UpdateOpacity();
			});
		}
		
		#endregion <<---------- Initializers ---------->>
		
		
		

		#region <<---------- General ---------->>
		
		public void FadeToBlack(float fadeTime, bool ignoreTimeScale = true) {
            Debug.Log($"Requesting fade to black, time '{fadeTime}' seconds with ignoreTimeScale set to '{ignoreTimeScale}'");
			this.TargetAlpha = 1f;
			this.TargetFadeTime = fadeTime;
			this.IgnoreTimeScale = ignoreTimeScale;
            this.UpdateOpacity();
        }

		public void FadeToTransparent(float fadeTime, bool ignoreTimeScale = true) {
            Debug.Log($"Requesting fade to transparent, time '{fadeTime}' seconds with ignoreTimeScale set to '{ignoreTimeScale}'");
			this.TargetAlpha = 0f;
			this.TargetFadeTime = fadeTime;
			this.IgnoreTimeScale = ignoreTimeScale;
            this.UpdateOpacity();
		}

		private void UpdateOpacity() {
            if (this._fadeCanvasGroup == null) return;
			float currentAlpha = this._fadeCanvasGroup.alpha.CImprecise();
			if (this.TargetAlpha == currentAlpha) return;
			float delta = this.IgnoreTimeScale ? Time.unscaledDeltaTime : CTime.DeltaTimeScaled;
			float step = delta / this.TargetFadeTime;
			if (currentAlpha > this.TargetAlpha) step *= -1f;
			this._fadeCanvasGroup.alpha = currentAlpha + step;
		}
        
        #endregion <<---------- General ---------->>




        #region <<---------- Debug ---------->>

        public void DebugEnableFader() {
            this._fadeCanvasGroup.enabled = true;
        }
        
        public void DebugDisableFader() {
            this._fadeCanvasGroup.enabled = false;
        }

        #endregion <<---------- Debug ---------->>

		
	}
}

using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CDK {
	[Obsolete("Use Fader on timeline instead")]
	public class CFader : MonoBehaviour {

        #region <<---------- Singleton ---------->>

        public static CFader Instance {
            get {
                if (CApplication.IsQuitting || _instance != null) return _instance;
                var go = new GameObject("Fader");
                #if UNITY_EDITOR
                go.hideFlags = HideFlags.DontSaveInEditor;
                #endif
                return _instance = go.AddComponent<CFader>();
            }
        }
        static CFader _instance;

        #endregion <<---------- Singleton ---------->>




		#region <<---------- Properties ---------->>
		
		CanvasGroup _fadeCanvasGroup;
		float TargetAlpha;
		float TargetFadeTime;
		bool IgnoreTimeScale;
		
		#endregion <<---------- Properties ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>

		void Awake() {
			if (_instance != null && _instance != this) {
				gameObject.CDestroy(true);
				return;
			}
			gameObject.layer = 5; // UI
			gameObject.CDontDestroyOnLoad();

			// canvas
			var goCanvas = gameObject.AddComponent<Canvas>();
			goCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			goCanvas.sortingOrder = -1;
			try {
				goCanvas.sortingLayerID++;
			} catch (Exception e) {
				Debug.LogException(e);
			}

			// canvas group
			this._fadeCanvasGroup = gameObject.AddComponent<CanvasGroup>();
			this._fadeCanvasGroup.blocksRaycasts = false;
			this._fadeCanvasGroup.interactable = false;

			// image black
			var imgComp = gameObject.AddComponent<Image>();
			imgComp.color = Color.black;
			imgComp.maskable = false;
			imgComp.raycastTarget = false;
		}

		void Update() => UpdateOpacity();

		#endregion <<---------- MonoBehaviour ---------->>
		
		
		

		#region <<---------- General ---------->>
		
		public void FadeToBlack(float fadeTime, bool ignoreTimeScale = true) {
            //Debug.Log($"Requesting fade to black, time '{fadeTime}' seconds with ignoreTimeScale set to '{ignoreTimeScale}'");
			this.TargetAlpha = 1f;
			this.TargetFadeTime = fadeTime;
			this.IgnoreTimeScale = ignoreTimeScale;
            this.UpdateOpacity();
        }

		public void FadeToTransparent(float fadeTime, bool ignoreTimeScale = true) {
            //Debug.Log($"Requesting fade to transparent, time '{fadeTime}' seconds with ignoreTimeScale set to '{ignoreTimeScale}'");
			this.TargetAlpha = 0f;
			this.TargetFadeTime = fadeTime;
			this.IgnoreTimeScale = ignoreTimeScale;
            this.UpdateOpacity();
		}

		private void UpdateOpacity() {
            if (this.TargetFadeTime <= 0f) {
                this._fadeCanvasGroup.alpha = this.TargetAlpha;
                return;
            }
            float currentAlpha = this._fadeCanvasGroup.alpha.CImprecise();
			if (Mathf.Approximately(this.TargetAlpha, currentAlpha)) return;
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

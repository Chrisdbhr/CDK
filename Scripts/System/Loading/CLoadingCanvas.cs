using System;
using UniRx;
using UnityEngine;

namespace CDK {
	public class CLoadingCanvas : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>
        
        [SerializeField] private Canvas _loadingUI;
        private IDisposable _timerDisposable;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>
        
        private void Awake() {
            DontDestroyOnLoad(this.gameObject);
            HideLoadingUI();
        }

        private void OnDestroy() {
            this._timerDisposable?.Dispose();
        }
        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- General ---------->>
        
        public void ShowLoadingUI() {
            this._timerDisposable?.Dispose();
            this._timerDisposable = Observable.Timer(TimeSpan.FromSeconds(1f), TimeSpan.Zero).Subscribe(_ => {
                if (this == null) return;
                if(this._loadingUI) this._loadingUI.enabled = true;
            });
        }
		
        public void HideLoadingUI() {
            this._timerDisposable?.Dispose();
            if(this._loadingUI) this._loadingUI.enabled = false;
        }
        
        #endregion <<---------- General ---------->>
	}
}

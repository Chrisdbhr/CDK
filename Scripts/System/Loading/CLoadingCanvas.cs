using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CDK {
	public class CLoadingCanvas : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>
        
        [SerializeField] private Canvas _loadingUI;
        [SerializeField] private Image _imageProgress;
        private List<AsyncOperation> _activeAsyncOperations = new List<AsyncOperation>();
        private IDisposable _timerDisposable;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>
        
        private void Awake() {
            DontDestroyOnLoad(this.gameObject);
            HideLoadingUI();
        }

        private void LateUpdate() {
            foreach (var op in this._activeAsyncOperations) {
                if (op == null || op.isDone) continue;
                this._imageProgress.fillAmount = (op.progress / 0.9f).CClamp01();
                break;
            }
        }

        private void OnDestroy() {
            this._timerDisposable?.Dispose();
        }
        
        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- General ---------->>

        /// <summary>
        /// Returns the same async operation for easier chained operations.
        /// </summary>
        public AsyncOperation MonitorAsyncOperation(AsyncOperation asyncOperation) {
            if (this._activeAsyncOperations.Count <= 0) {
                this.ShowLoadingUI();
            }
            this._activeAsyncOperations.Add(asyncOperation);
            asyncOperation.completed += ActiveAsyncOperationCompleted;
            return asyncOperation;
        }

        private void ActiveAsyncOperationCompleted(AsyncOperation op) {
            this._activeAsyncOperations.Remove(op);
            if (this._activeAsyncOperations.Count <= 0) {
                HideLoadingUI();
            }
        }

        private void ShowLoadingUI() {
            this._timerDisposable?.Dispose();
            this._timerDisposable = Observable.Timer(TimeSpan.FromSeconds(1f), Scheduler.MainThreadIgnoreTimeScale)
            .Subscribe(_ => {
                if (this == null) return;
                if(this._loadingUI) this._loadingUI.enabled = true;
            });
        }
		
        private void HideLoadingUI() {
            this._timerDisposable?.Dispose();
            if(this._loadingUI) this._loadingUI.enabled = false;
        }
        
        #endregion <<---------- General ---------->>
	}
}

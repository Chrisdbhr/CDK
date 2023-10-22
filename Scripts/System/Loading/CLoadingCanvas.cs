using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CDK {
	public class CLoadingCanvas : MonoBehaviour {
        
        #region <<---------- Singleton ---------->>

        public static CLoadingCanvas get {
            get {
                if (CSingletonHelper.CannotCreateAnyInstance() || _instance != null) {
                    return _instance;
                }
                return (_instance = CAssets.LoadResourceAndInstantiate<CLoadingCanvas>("System/Loading Canvas"));
            }
        }
        private static CLoadingCanvas _instance;

        #endregion <<---------- Singleton ---------->>

        
        
        
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
                this._timerDisposable?.Dispose();
                this._timerDisposable = Observable.Timer(TimeSpan.FromSeconds(1f), Scheduler.MainThreadIgnoreTimeScale)
                .Subscribe(_ => {
                    if (this == null) return;
                    this.ShowLoadingUI();
                });
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

        public bool IsLoading => this._loadingUI != null && this._loadingUI.enabled;

        public void ShowLoadingUI() {
            if(this._loadingUI) this._loadingUI.enabled = true;
        }
		
        public void HideLoadingUI() {
            this._timerDisposable?.Dispose();
            if(this._loadingUI) this._loadingUI.enabled = false;
        }
        
        #endregion <<---------- General ---------->>
	}
}
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace CDK {
	public class CLoadingCanvas : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>
        
        [SerializeField] Canvas _loadingUI;
        [SerializeField] Image _imageProgress;
        readonly List<AsyncOperation> _activeAsyncOperations = new ();
        [NonSerialized] IDisposable _timerDisposable;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>

        void Awake() {
            DontDestroyOnLoad(gameObject);
            HideLoadingUI();
        }

        void LateUpdate() {
            foreach (var op in _activeAsyncOperations) {
                if (op == null || op.isDone) continue;
                _imageProgress.fillAmount = (op.progress / 0.9f).CClamp01();
                break;
            }
        }

        void OnDestroy() {
            _timerDisposable?.Dispose();
        }
        
        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- General ---------->>

        /// <summary>
        /// Returns the same async operation for easier chained operations.
        /// </summary>
        public AsyncOperation MonitorAsyncOperation(AsyncOperation asyncOperation) {
            if (_activeAsyncOperations.Count <= 0) {
                _timerDisposable?.Dispose();
                _timerDisposable = Observable.Timer(TimeSpan.FromSeconds(1f))
                .Subscribe(_ => {
                    if (this == null) return;
                    ShowLoadingUI();
                });
            }
            _activeAsyncOperations.Add(asyncOperation);
            asyncOperation.completed += ActiveAsyncOperationCompleted;
            return asyncOperation;
        }

        void ActiveAsyncOperationCompleted(AsyncOperation op) {
            _activeAsyncOperations.Remove(op);
            if (_activeAsyncOperations.Count <= 0) {
                HideLoadingUI();
            }
        }

        public bool IsLoading => _loadingUI != null && _loadingUI.enabled;

        public void ShowLoadingUI() {
            if(_loadingUI) _loadingUI.enabled = true;
        }
		
        public void HideLoadingUI() {
            _timerDisposable?.Dispose();
            if(_loadingUI) _loadingUI.enabled = false;
        }
        
        #endregion <<---------- General ---------->>
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CDK {
	public class CLoadingCanvas : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>
        
        [SerializeField] Canvas _loadingUI;
        [SerializeField] Image _imageProgress;
        readonly List<AsyncOperation> _activeAsyncOperations = new ();
        [NonSerialized] float _delaySecondsToShowLoading = 1f;
        [NonSerialized] float _lastTimeAsyncOpCountWasZero;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>

        void Awake()
        {
            _loadingUI.CAssertIfNull("Loading Canvas has a null reference to the loading UI.");
            DontDestroyOnLoad(gameObject);
        }

        void LateUpdate()
        {
            bool shouldShow = (Time.time >= _lastTimeAsyncOpCountWasZero + _delaySecondsToShowLoading) && _activeAsyncOperations.Count > 0;
            if(shouldShow) {
                float acumulatedProgress = _activeAsyncOperations.Sum(op => op.progress);
                _imageProgress.fillAmount = (acumulatedProgress / (_activeAsyncOperations.Count * 0.9f)).CClamp01();
            }
            _loadingUI.enabled = shouldShow;
        }

        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- General ---------->>

        /// <summary>
        /// Returns the same async operation for easier chained operations.
        /// </summary>
        public AsyncOperation MonitorAsyncOperation(AsyncOperation asyncOperation) {
            if(asyncOperation == null) {
                Debug.LogError("Skipping null AsyncOperation to monitor.");
                return null;
            }
            if(_activeAsyncOperations.Count <= 0) {
                _lastTimeAsyncOpCountWasZero = Time.time;
            }
            _activeAsyncOperations.Add(asyncOperation);
            asyncOperation.completed += ActiveAsyncOperationCompleted;
            return asyncOperation;
        }

        void ActiveAsyncOperationCompleted(AsyncOperation op) {
            _activeAsyncOperations.Remove(op);
        }

        #endregion <<---------- General ---------->>
	}
}